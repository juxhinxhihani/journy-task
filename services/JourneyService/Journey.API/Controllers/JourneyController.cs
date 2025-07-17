using Journey.API.Extensions;
using Journey.Application.DTOs;
using Journey.Application.Journys.AddJourny;
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
    }
}
