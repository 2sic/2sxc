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