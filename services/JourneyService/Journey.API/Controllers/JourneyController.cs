using Journey.API.Extensions;
using Journey.Application.DTOs;
using Journey.Application.Journys.AddJourny;
using Journey.Application.Journys.DeleteJourney;
using Journey.Application.Journys.GetJourneyByPublicLinkToken;
using Journey.Application.Journys.GetJourneys;
using Journey.Application.Journys.GetMonthlyRouteJourney;
using Journey.Application.Journys.GetPublicJourneys;
using Journey.Application.Journys.GetSharedJourneys;
using Journey.Application.Journys.GetUserJourneys;
using Journey.Application.Journys.RevokePublicLink;
using Journey.Application.Journys.ShareJourney;
using Journey.Application.Journys.ShareJourneyPublicLink;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JourneyController(ISender _sender) : ControllerBase
    {
        [HttpPost(Name = "CreateJourney")]
        public async Task<IResult> CreateJourney([FromBody] CreateJourneyDTO request)
        {
            var command = new CreateJourneyCommand(request);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpDelete("{journeyId}", Name = "DeleteJourney")]
        public async Task<IResult> DeleteJourney(Guid journeyId)
        {
            var command = new DeleteJourneyCommand(journeyId);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpGet(Name = "GetJourneys")]
        public async Task<IResult> GetJourneys([FromQuery] GetJourneysQuery query)
        {
            var result = await _sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpGet("public", Name = "GetPublicJourneys")]
        public async Task<IResult> GetPublicJourneys([FromQuery] GetPublicJourneysQuery query)
        {
            var result = await _sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpGet("{token}", Name = "GetJourneyByPublicLinkToken")]
        [AllowAnonymous]
        public async Task<IResult> GetJourneyByPublicLinkToken(string token)
        {
            var query = new GetPublicJourneysByTokenQuery(token);
            var result = await _sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        
        [HttpGet("shared", Name = "GetSharedJourneys")]
        public async Task<IResult> GetSharedJourneys([FromQuery] GetSharedJourneysQuery query)
        {
            var result = await _sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        
        [HttpGet("monthly-route", Name = "GetMonthlyRouteJourney")]
        public async Task<IResult> GetMonthlyRouteJourney([FromQuery] GetMonthlyRouteJourneyQuery query)
        {
            var result = await _sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }

        [HttpGet("user/{userId}", Name = "GetJourneysByUserId")]
        public async Task<IResult> GetJourneysByUserId([FromQuery] GetJourneysByUserQuery query)
        {
            var result = await _sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpPut("link/revoke/{journeyId}", Name = "RevokePublicLink")]
        public async Task<IResult> RevokePublicLink(Guid journeyId)
        {
            var command = new RevokeJourneyPublicLinkCommand(journeyId);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpPost("share/{journeyId}", Name = "ShareJourney")]
        public async Task<IResult> ShareJourney(Guid journeyId, List<Guid> UsersIds)
        {
            var command = new ShareJourneyCommand(journeyId, UsersIds);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
        [HttpPost("share/public/{journeyId}", Name = "ShareJourneyPublicLink")]
        public async Task<IResult> ShareJourneyPublicLink(Guid journeyId)
        {
            var command = new ShareJourneyPublicLinkCommand(journeyId);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return result.ToProblemDetails();
            }
        }
    }
}