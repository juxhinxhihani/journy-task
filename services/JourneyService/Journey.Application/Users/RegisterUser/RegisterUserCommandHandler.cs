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
                                                     IIdentityService _identityService) 
        : ICommandHandler<RegisterUserCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _userRepository.IsEmailUniqueAsync(request.dto.Email))
            {
                var user = _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefault(x => x.Email == request.dto.Email);

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
                    request.dto.FirstName,
                    request.dto.LastName,
                    request.dto.DateOfBirth,
                    request.dto.Email,
                    request.dto.Password,
                    UserRoles.GetRole(request.dto.Role).Value);

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
                
                User.SendConfirmEmail(identityUserResult.Value, token.Value);
                await _context.SaveChangesAsync(cancellationToken);        
                
                transaction.Commit();

                return identityUserResult.Value.Id;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

        }
    }
}
