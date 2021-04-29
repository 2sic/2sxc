using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// DELETE THIS - NOT IN USE.
    /// This is just example how to create typical Oqtane Controller.
    /// </summary>
    [Microsoft.AspNetCore.Mvc.Route("{alias:int}/api/[controller]")]
    public class SxcOqtaneController : Controller
    {
        private readonly ISxcRepository _sxcRepository;
        private readonly ILogManager _logger;
        private readonly Lazy<ISxcOqtane> _sxcOqtane;
        private readonly Lazy<IAliasRepository> _aliases;
        private readonly Lazy<ISiteRepository> _sites;
        private readonly Lazy<IPageRepository> _pages;
        private readonly Lazy<IModuleRepository> _modules;
        private readonly Lazy<IModuleDefinitionRepository> _moduleDefinitions;
        private readonly Lazy<ISettingRepository> _settings;
        protected int _entityId = -1;

        public SxcOqtaneController(ISxcRepository sxcRepository, ILogManager logger, IHttpContextAccessor accessor, Lazy<ISxcOqtane> sxcOqtane, Lazy<IAliasRepository> aliases, Lazy<ISiteRepository> sites, Lazy<IPageRepository> pages, Lazy<IModuleRepository> modules, Lazy<IModuleDefinitionRepository> moduleDefinitions, Lazy<ISettingRepository> settings)
        {
            _sxcRepository = sxcRepository;
            _logger = logger;
            _sxcOqtane = sxcOqtane;
            _aliases = aliases;
            _sites = sites;
            _pages = pages;
            _modules = modules;
            _moduleDefinitions = moduleDefinitions;
            _settings = settings;

            if (accessor.HttpContext.Request.Query.ContainsKey(OqtConstants.EntityIdParam))
            {
                _entityId = int.Parse(accessor.HttpContext.Request.Query[OqtConstants.EntityIdParam]);
            }
        }

        [HttpGet("Prepare")]
        //[Authorize(Policy = "ViewModule")]
        public SxcOqtaneDto Prepare([FromQuery] int aliasId, [FromQuery] int siteId, [FromQuery] int pageId, [FromQuery] int moduleId, [FromQuery] string originalParameters)
        {
            var alias = _aliases.Value.GetAlias(aliasId);
            var site = _sites.Value.GetSite(siteId);
            var page = _pages.Value.GetPage(pageId); // TODO: probably need to add security related to user
            var module = _modules.Value.GetModule(moduleId);
            var moduleDefinitions = _moduleDefinitions.Value.GetModuleDefinitions(module.SiteId).ToList();
            module.ModuleDefinition = moduleDefinitions.Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);
            module.Settings = _settings.Value.GetSettings(EntityNames.Module, moduleId).ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);

            var rez = _sxcOqtane.Value.Prepare(alias, site, page, module);

            // HACK: TODO...
            //rez.Resources = null;

            return rez;
        }
    }
}
