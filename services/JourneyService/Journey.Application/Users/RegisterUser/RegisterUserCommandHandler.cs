using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Identity.Interface;
using Journey.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journey.Application.Users.RegisterUser
{
    internal sealed class RegisterUserCommandHandler(IUserRepository _userRepository,
                                                     IApplicationDbContext _context,
                                                     IIdentityService _identityService,
                                                     IActualUser _loggedUser) 
        : ICommandHandler<RegisterUserCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _userRepository.IsEmailUniqueAsync(request.dto.Email))
            {
                var user = _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefault(x => x.Email == request.dto.Email));

                if (user.IsDeleted)
                {
                    user.UnDelete();

                    _userRepository.Update(user);

                    await _context.SaveChangesAsync(cancellationToken);

                    return Result.Failure<Guid>(UserErrors.FailedCreating);
                }
                return Result.Failure<Guid>(UserErrors.EmailNotUnique);
            }
        

            using var transaction = _context.BeginTransaction();
            try
            {

                var identityUserResult = await _identityService.CreateIdentityUser(
                    request.dto.Email, UserRoles.GetRole(request.dto.Role).Value);

                if (identityUserResult.IsFailure)
                {
                    transaction.Rollback();
                    return Result.Failure<Guid>(UserErrors.FailedCreating);
                }

                var token = await _identityService.GenerateEmailConfirmationTokenAsync(identityUserResult.Value);

                if (token.IsFailure)
                {
                    transaction.Rollback();
                    return Result.Failure<Guid>(UserErrors.FailedCreating);
                }
                bool isAdmin = false;
                if (_loggedUser != null && (_loggedUser.Role == UserRoles.Admin || _loggedUser.Role == UserRoles.RegularUser))
                {
                    isAdmin = true;
                }
                else
                {
                    isAdmin = false;
                }

                var webPortalUserID = await _userRepository.GetUserId();

                var user = User.Create(
                    webPortalUserID,
                    request.dto.FirstName,
                    request.dto.LastName,
                    email,
                    new PhoneNumber(request.CreateUserDto?.PhoneNumber?.PhoneNumber ?? ""),
                    request.CreateUserDto?.PhoneNumber?.PhonePrefix ?? "",
                    false,
                    false,
                    identityUserResult.Value.Id,
                    UserRoles.RegularUser,
                    isAdmin,
                    token.Value,
                    UserStatuses.Submitted.ToString());

                _userRepository.Add(user);

                await _context.SaveChangesAsync(cancellationToken);

                User.SendConfirmEmail(user, token.Value);
                await _context.SaveChangesAsync(cancellationToken);        
        
                await _context.SaveChangesAsync(cancellationToken);

                transaction.Commit();

                return user.Id;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

        }
    }
}
