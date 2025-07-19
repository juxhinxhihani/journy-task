using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Journey.Domain.Identity.Interface;
using Journey.Domain.Users;

namespace Journey.Application.Users.ConfirmEmail;

    public class ConfirmEmailCommandHandler(
        IApplicationDbContext _context,
        IIdentityService _identityService,
        IUserRepository _userRepository) : ICommandHandler<ConfirmEmailCommand>
    {
       
        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(request.userId);
            if (user is null)
            {
                return Result.Failure(UserErrors.NotFound);
            }
            
            using var transaction = _context.BeginTransaction();
            try
            {
                
                var confirmResult = await _identityService.ConfirmEmail(user, request.token);
                var updatedUser = User.ConfirmEmail(user);

                if (confirmResult.IsFailure)
                {
                    var token = await _identityService.GenerateEmailConfirmationTokenAsync(user);
                    updatedUser = User.ResendConfirmEmail(user, token.Value);
                }
                
                _userRepository.Update(updatedUser);
                await _context.SaveChangesAsync(cancellationToken);

                transaction.Commit();

                if (confirmResult.IsFailure)
                    return Result.Failure(UserErrors.NewConfirmEmail);
                else
                    return confirmResult;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result.Failure(ex.Message);
            }
        }
    }
