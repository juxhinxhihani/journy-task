using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Journey.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JournyController(ISender _sender) : ControllerBase
    {

        [HttpGet(Name = "GetAllJournys")]
        public IResult<> Get()
        {
           
        }
    }
}
