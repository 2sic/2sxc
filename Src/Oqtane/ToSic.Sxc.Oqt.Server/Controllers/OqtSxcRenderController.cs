using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    //[Route("{alias:int}/api/[controller]")]
    [Route(ControllerRoutes.ApiRoute)]
    public class OqtSxcRenderController : ModuleControllerBase
    {
        public OqtSxcRenderController(IHttpContextAccessor accessor,
            ISxcOqtane sxcOqtane,
            IAliasRepository aliases,
            ISiteRepository sites,
            IPageRepository pages,
            IModuleRepository modules,
            IModuleDefinitionRepository moduleDefinitions,
            ISettingRepository settings,
            IUserPermissions userPermissions,
            ILogManager logger,
            SiteState siteState) : base(logger, accessor)
        {
            _accessor = accessor;
            _sxcOqtane = sxcOqtane;
            _aliases = aliases;
            _sites = sites;
            _pages = pages;
            _modules = modules;
            _moduleDefinitions = moduleDefinitions;
            _settings = settings;
            _userPermissions = userPermissions;
            _siteState = siteState;
        }

        private readonly IHttpContextAccessor _accessor;
        private readonly ISxcOqtane _sxcOqtane;
        private readonly IAliasRepository _aliases;
        private readonly ISiteRepository _sites;
        private readonly IPageRepository _pages;
        private readonly IModuleRepository _modules;
        private readonly IModuleDefinitionRepository _moduleDefinitions;
        private readonly ISettingRepository _settings;
        private readonly IUserPermissions _userPermissions;
        private readonly SiteState _siteState;

        [HttpGet("{aliasId:int}/{pageId:int}/{moduleId:int}/{culture}/Prepare")]
        //[Authorize(Policy = PolicyNames.ViewModule)]
        public OqtViewResultsDto Prepare([FromRoute] int aliasId, [FromRoute] int pageId, [FromRoute] int moduleId, [FromRoute] string culture, [FromQuery] string originalParameters)
        {
            if (moduleId != AuthEntityId(EntityNames.Module))
                return Forbidden("Unauthorized OqtSxcRenderController Get Attempt {ModuleId}", moduleId);

            var alias = _aliases.GetAlias(aliasId);
            if (alias == null)
                return Forbidden("Unauthorized Alias Get Attempt {AliasId}", aliasId);

            // HACKS: STV POC - indirectly share information
            _accessor.HttpContext.Items.TryAdd("AliasFor2sxc", alias);

            // Store Alias in SiteState for background processing.
            if (_siteState != null) _siteState.Alias = alias;

            // Set User culture
            if (culture != CultureInfo.CurrentUICulture.Name) OqtCulture.SetCulture(culture);

            var site = _sites.GetSite(alias.SiteId);
            if (site == null)
                return Forbidden("Unauthorized Site Get Attempt {SiteId}", alias.SiteId);

            var page = _pages.GetPage(pageId);
            if (page == null || page.SiteId != alias.SiteId || !_userPermissions.IsAuthorized(User, EntityNames.Page, pageId, PermissionNames.View))
                return Forbidden("Unauthorized Page Get Attempt {pageId}", pageId);

            var module = _modules.GetModule(moduleId);
            if (module == null || module.SiteId != alias.SiteId || !_userPermissions.IsAuthorized(User, "View", module.Permissions))
                return Forbidden("Unauthorized Module Get Attempt {ModuleId}", moduleId);

            var moduleDefinitions = _moduleDefinitions.GetModuleDefinitions(module.SiteId).ToList();
            module.ModuleDefinition = moduleDefinitions.Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);
            //if (module.ModuleDefinition == null || !_userPermissions.IsAuthorized(User, "Utilize", module.ModuleDefinition.Permissions))
            //    return Forbidden("Unauthorized ModuleDefinition Get Attempt {SiteId}", module.SiteId);

            module.Settings = _settings.GetSettings(EntityNames.Module, moduleId).ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);

            return _sxcOqtane.Prepare(alias, site, page, module);
        }

        private OqtViewResultsDto Forbidden(string message, params object[] args)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, message, args);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }
}
