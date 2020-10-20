using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using ToSic.Sxc.Models;
using ToSic.Sxc.Repository;

namespace ToSic.Sxc.Controllers
{
    [Route("{alias}/api/[controller]")]
    public class SxcController : Controller
    {
        private readonly ISxcRepository _SxcRepository;
        private readonly ILogManager _logger;
        protected int _entityId = -1;

        public SxcController(ISxcRepository SxcRepository, ILogManager logger, IHttpContextAccessor accessor)
        {
            _SxcRepository = SxcRepository;
            _logger = logger;

            if (accessor.HttpContext.Request.Query.ContainsKey("entityid"))
            {
                _entityId = int.Parse(accessor.HttpContext.Request.Query["entityid"]);
            }
        }

        // GET: 1/api/sxc/ping
        [HttpGet("ping")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants", Justification = "<Pending>")]
        public string Ping()
        {
            return "pong";
        }



        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<Models.Sxc> Get(string moduleid)
        {
            return _SxcRepository.GetSxcs(int.Parse(moduleid));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ViewModule")]
        public Models.Sxc Get(int id)
        {
            Models.Sxc Sxc = _SxcRepository.GetSxc(id);
            if (Sxc != null && Sxc.ModuleId != _entityId)
            {
                Sxc = null;
            }
            return Sxc;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public Models.Sxc Post([FromBody] Models.Sxc Sxc)
        {
            if (ModelState.IsValid && Sxc.ModuleId == _entityId)
            {
                Sxc = _SxcRepository.AddSxc(Sxc);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Sxc Added {Sxc}", Sxc);
            }
            return Sxc;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public Models.Sxc Put(int id, [FromBody] Models.Sxc Sxc)
        {
            if (ModelState.IsValid && Sxc.ModuleId == _entityId)
            {
                Sxc = _SxcRepository.UpdateSxc(Sxc);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Sxc Updated {Sxc}", Sxc);
            }
            return Sxc;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int id)
        {
            Models.Sxc Sxc = _SxcRepository.GetSxc(id);
            if (Sxc != null && Sxc.ModuleId == _entityId)
            {
                _SxcRepository.DeleteSxc(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Sxc Deleted {SxcId}", id);
            }
        }
    }
}
