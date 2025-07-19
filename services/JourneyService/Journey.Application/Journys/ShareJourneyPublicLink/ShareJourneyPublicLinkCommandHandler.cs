using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Journeys.Interface;
using Journey.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Journys.ShareJourneyPublicLink;

public class ShareJourneyPublicLinkCommandHandler(
    IJourneyRepository _journeyRepository,
    IApplicationDbContext _context,
    IActualUser _loggedUser,
    IHttpContextAccessor _httpContextAccessor)
    : ICommandHandler<ShareJourneyPublicLinkCommand, string>
{
    public async Task<Result<string>> Handle(ShareJourneyPublicLinkCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_loggedUser.Id, out var userId))
        {
            return Result.Failure<string>(UserErrors.NotLoggedIn);
        }

        var journey = await _context.Journeys.FirstOrDefaultAsync(j => j.Id == request.JourneyId, cancellationToken);

        if (journey == null)
            return Result.Failure<string>(Error.NotFound("Journey.NotFound", "Journey can not be found."));

        var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        var publicToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); 
        var publicUrl = $"{baseUrl}/api/public-journeys/{publicToken}";

        journey.GeneratePublicLink(publicUrl);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(publicUrl);
    }
}