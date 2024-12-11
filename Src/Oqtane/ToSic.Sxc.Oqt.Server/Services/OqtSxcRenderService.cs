using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Globalization;
using System.Threading.Tasks;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared.Helpers;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Oqt.Server.Services;

public class OqtSxcRenderService(
    IHttpContextAccessor accessor,
    Generator<IOqtSxcViewBuilder> oqtSxcViewBuilder,
    AliasResolver aliasResolver,
    ISiteRepository sites,
    IPageRepository pages,
    IModuleRepository modules,
    IModuleDefinitionRepository definitions,
    ISettingRepository settings,
    IUserPermissions userPermissions,
    ILogManager logger,
    SiteState siteState) : IOqtSxcRenderService/*, ITransientService*/
{
    public Task<OqtViewResultsDto> RenderAsync(RenderParameters @params) => Task.FromResult(Render(@params));

    public OqtViewResultsDto Render(RenderParameters @params)
    {
        try
        {
            StoreParamsInHttpContext(@params);

            var alias = aliasResolver.GetAndStoreAlias(@params.AliasId);
            if (alias == null)
                return Forbidden("Unauthorized Alias Get Attempt {AliasId}", @params.AliasId);

            // Set User culture
            if (@params.Culture != CultureInfo.CurrentUICulture.Name) OqtCulture.SetCulture(@params.Culture);

            var site = sites.GetSite(alias.SiteId);
            if (site == null)
                return Forbidden("Unauthorized Site Get Attempt {SiteId}", alias.SiteId);

            var page = pages.GetPage(@params.PageId);
            if (page == null || page.SiteId != alias.SiteId || !userPermissions.IsAuthorized(accessor?.HttpContext?.User, alias.SiteId, EntityNames.Page, @params.PageId, PermissionNames.View))
                return Forbidden("Unauthorized Page Get Attempt {pageId}", @params.PageId);

            var module = modules.GetModule(@params.ModuleId);
            if (module == null || module.SiteId != alias.SiteId || !userPermissions.IsAuthorized(accessor?.HttpContext?.User, "View", module.Permissions)) 
                return Forbidden("Unauthorized Module Get Attempt {ModuleId}", @params.ModuleId);

            var moduleDefinitions = definitions.GetModuleDefinitions(module.SiteId).ToList();
            module.ModuleDefinition = moduleDefinitions.Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);

            module.Settings = settings.GetSettings(EntityNames.Module, @params.ModuleId).ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);

            return oqtSxcViewBuilder.New().Render(alias, site, page, module, @params.PreRender);
        }
        catch (Exception ex)
        {
            return Error(ex);
        }
    }

    /// <summary>
    /// Stores the rendering parameters in the current HTTP context for later retrieval during the request lifecycle.
    /// ModuleId is necessary for ModuleService to scope its functionality to each module rendering in Oqtane Interactive Server.
    /// </summary>
    /// <param name="params">
    /// RenderParameters containing module, page, and site information.
    /// </param>
    private void StoreParamsInHttpContext(RenderParameters @params)
    {
        if (accessor.HttpContext != null)
        {
            // Store the render parameters in HttpContext.Items using a specific key.
            // This allows other components to access these parameters throughout the current HTTP request.
            accessor.HttpContext.Items[ModuleService.OqtaneSxcRenderParameters] = @params;

            //accessor.HttpContext.Items.TryGetValue("Oqtane.Sxc.RenderParametersCounter", out var counter);
            //accessor.HttpContext.Items["Oqtane.Sxc.RenderParametersCounter"] = (counter as int? ?? 0) + 1;
        }
    }

    private OqtViewResultsDto Forbidden(string message, params object[] args)
    {
        logger.Log(LogLevel.Error, this, LogFunction.Security, message, args);
        //if (accessor?.HttpContext != null) 
        //    accessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return null;
    }

    private OqtViewResultsDto Error(Exception ex)
    {
        logger.Log(LogLevel.Error, this, LogFunction.Read, ex, $"exception in {nameof(Render)}");
        return new() { 
            ErrorMessage = ErrorHelper.ErrorMessage(ex, IsSuperUser)
        };
    }

    private bool IsSuperUser => 
        (accessor?.HttpContext?.User.IsInRole(RoleNames.Host) ?? false)
        || (accessor?.HttpContext?.User.IsInRole(RoleNames.Admin) ?? false);
}