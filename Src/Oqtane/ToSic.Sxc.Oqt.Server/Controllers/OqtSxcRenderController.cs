using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared.Helpers;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Controllers;

//[Route("{alias:int}/api/[controller]")]
[Route(ControllerRoutes.ApiRoute)]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtSxcRenderController(
    IHttpContextAccessor accessor,
    IOqtSxcViewBuilder sxcOqtane,
    IAliasRepository aliases,
    ISiteRepository sites,
    IPageRepository pages,
    IModuleRepository modules,
    IModuleDefinitionRepository definitions,
    ISettingRepository settings,
    IUserPermissions userPermissions,
    ILogManager logger,
    Oqtane.Shared.SiteState siteState)
    : ModuleControllerBase(logger, accessor)
{
    private readonly IHttpContextAccessor _accessor = accessor;

    [HttpGet("{aliasId:int}/{pageId:int}/{moduleId:int}/{culture}/{preRender:bool}/Prepare")]
    //[Authorize(Policy = PolicyNames.ViewModule)]
    public OqtViewResultsDto Prepare([FromRoute] int aliasId, [FromRoute] int pageId, [FromRoute] int moduleId, [FromRoute] string culture, [FromRoute] bool preRender, [FromQuery] string originalParameters)
    {
        try
        {
            if (moduleId != AuthEntityId(EntityNames.Module))
                return Forbidden("Unauthorized OqtSxcRenderController Get Attempt {ModuleId}", moduleId);

            var alias = aliases.GetAlias(aliasId);
            if (alias == null)
                return Forbidden("Unauthorized Alias Get Attempt {AliasId}", aliasId);

            // HACKS: STV POC - indirectly share information
            _accessor.HttpContext.Items.TryAdd("AliasFor2sxc", alias);

            // Store Alias in SiteState for background processing.
            if (siteState != null) siteState.Alias = alias;

            // Set User culture
            if (culture != CultureInfo.CurrentUICulture.Name) OqtCulture.SetCulture(culture);

            var site = sites.GetSite(alias.SiteId);
            if (site == null)
                return Forbidden("Unauthorized Site Get Attempt {SiteId}", alias.SiteId);

            var page = pages.GetPage(pageId);
            if (page == null || page.SiteId != alias.SiteId || !userPermissions.IsAuthorized(User, EntityNames.Page, pageId, PermissionNames.View))
                return Forbidden("Unauthorized Page Get Attempt {pageId}", pageId);

            var module = modules.GetModule(moduleId);
            if (module == null || module.SiteId != alias.SiteId || !userPermissions.IsAuthorized(User, "View", module.Permissions))
                return Forbidden("Unauthorized Module Get Attempt {ModuleId}", moduleId);

            var moduleDefinitions = definitions.GetModuleDefinitions(module.SiteId).ToList();
            module.ModuleDefinition = moduleDefinitions.Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);

            module.Settings = settings.GetSettings(EntityNames.Module, moduleId).ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);

            return sxcOqtane.Prepare(alias, site, page, module, preRender);
        }
        catch (Exception ex)
        {
            return Error(ex);
        }
    }

    private OqtViewResultsDto Forbidden(string message, params object[] args)
    {
        _logger.Log(LogLevel.Error, this, LogFunction.Security, message, args);
        HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return null;
    }

    private OqtViewResultsDto Error(Exception ex)
    {
        _logger.Log(LogLevel.Error, this, LogFunction.Read, ex, $"exception in {nameof(Prepare)}");
        return new() { ErrorMessage = ErrorHelper.ErrorMessage(ex, User.IsInRole(RoleNames.Host) || User.IsInRole(RoleNames.Admin)) };
    }
}