using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PongController : Int01StatelessControllerBase
    {
        [HttpGet]
        public string Pong()
        {
            return "pong";
        }

        protected override string HistoryLogName => "Int.Pong";
    }
}
