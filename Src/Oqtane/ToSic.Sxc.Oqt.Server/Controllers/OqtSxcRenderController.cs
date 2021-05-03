using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Repository;
using Oqtane.Shared;
using System.Linq;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route("{alias:int}/api/[controller]")]
    public class OqtSxcRenderController : Controller
    {
        public OqtSxcRenderController(IHttpContextAccessor accessor,
            ISxcOqtane sxcOqtane,
            IAliasRepository aliases,
            ISiteRepository sites,
            IPageRepository pages,
            IModuleRepository modules,
            IModuleDefinitionRepository moduleDefinitions,
            ISettingRepository settings,
            SiteState siteState)
        {
            _accessor = accessor;
            _sxcOqtane = sxcOqtane;
            _aliases = aliases;
            _sites = sites;
            _pages = pages;
            _modules = modules;
            _moduleDefinitions = moduleDefinitions;
            _settings = settings;
            _siteState = siteState;

            if (accessor.HttpContext.Request.Query.ContainsKey(OqtConstants.EntityIdParam))
            {
                _entityId = int.Parse(accessor.HttpContext.Request.Query[OqtConstants.EntityIdParam]);
            }
        }

        private readonly IHttpContextAccessor _accessor;
        private readonly ISxcOqtane _sxcOqtane;
        private readonly IAliasRepository _aliases;
        private readonly ISiteRepository _sites;
        private readonly IPageRepository _pages;
        private readonly IModuleRepository _modules;
        private readonly IModuleDefinitionRepository _moduleDefinitions;
        private readonly ISettingRepository _settings;
        private readonly SiteState _siteState;
        protected int _entityId = -1;


        [HttpGet("{aliasId:int}/{pageId:int}/{moduleId:int}/{culture}/Prepare")]
        //[Authorize(Policy = "ViewModule")]
        public OqtViewResultsDto Prepare([FromRoute] int aliasId, [FromRoute] int pageId, [FromRoute] int moduleId, [FromRoute] string culture, [FromQuery] string originalParameters)
        {
            var alias = _aliases.GetAlias(aliasId);

            // Store Alias in SiteState for background processing.
            if (_siteState != null) _siteState.Alias = alias;

            // Set User culture
            if (culture != CultureInfo.CurrentUICulture.Name) OqtCulture.SetCulture(culture);

            var site = _sites.GetSite(alias.SiteId);
            var page = _pages.GetPage(pageId); // TODO: probably need to add security related to user
            var module = _modules.GetModule(moduleId);
            var moduleDefinitions = _moduleDefinitions.GetModuleDefinitions(module.SiteId).ToList();
            module.ModuleDefinition = moduleDefinitions.Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);
            module.Settings = _settings.GetSettings(EntityNames.Module, moduleId).ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);

            // HACKS: STV POC - indirectly share information
            if (alias != null) _accessor.HttpContext.Items.TryAdd("AliasFor2sxc", alias);

            return _sxcOqtane.Prepare(alias, site, page, module);
        }
    }
}
