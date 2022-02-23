using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PongController : IntControllerBase<DummyControllerReal>
    {
        public PongController() : base("Pong") { }

        [HttpGet]
        public string Pong() => "pong";
    }
}
