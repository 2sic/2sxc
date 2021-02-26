using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// DELETE THIS - NOT IN USE.
    /// This is just example how to create Oqtane Controller with DynamicCode.
    /// </summary>
    [Route("{alias}/api/[controller]")]
    [AllowAnonymous]
    public class Poc01Controller : OqtStatefulControllerBase
    {
        protected override string HistoryLogName { get; }

        public Poc01Controller(StatefulControllerDependencies dependencies) : base(dependencies)
        {
        }

        [HttpGet("hello")]
        public string Hello()
        {
            var shared = CreateInstance("Content/Tenants/1/Sites/1/2sxc/Poc01/FunctionsBasic.cs");
            return shared.SayHello();
        }

        // GET: 1/api/adam/ping
        [HttpGet("ping")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants", Justification = "<Pending>")]
        public string Ping()
        {
            return $"pong";
        }
    }
}
