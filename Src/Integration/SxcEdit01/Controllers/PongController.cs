using Microsoft.AspNetCore.Mvc;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PongController : IntStatelessControllerBase
    {
        [HttpGet]
        public string Pong()
        {
            return "pong";
        }

        protected override string HistoryLogName => "Int.Pong";
    }
}
