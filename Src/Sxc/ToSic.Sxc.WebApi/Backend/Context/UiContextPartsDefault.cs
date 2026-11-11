using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Services;
using ToSic.Sys.Capabilities.Features;
using ContextLanguageDto = ToSic.Eav.WebApi.Sys.Dto.ContextLanguageDto;

namespace ToSic.Sxc.Backend.Context;


public interface IUiContextParts : IServiceWithSetup<IAppReader?>
{
    ContextLanguageDto? GetLanguage(Ctx flags);
    ContextResourceWithApp? GetSystem(Ctx flags);
    ContextResourceWithApp? GetSite(Ctx flags);
    WebResourceDto GetPage(Ctx flags);
    ContextEnableDto GetEnable(CtxEnable ctx);
    ContextAppDto? GetApp(Ctx flags);
    ContextUserDto GetUser(Ctx flags);

    /// <summary>
    /// Features provided must be virtual, so that applications can override this.
    /// </summary>
    /// <returns></returns>
    IList<FeatureDto> GetFeatures(Ctx flags);
}

public class UiContextPartsDefault(
    IContextOfSiteService siteCtxSvc,
    LazySvc<ISysFeaturesService> features,
    LazySvc<IUiData> uiDataLazy,
    LazySvc<ContextLanguageDtoService> languagesBackend,
    IAppPathsMicroSvc appPaths,
    LazySvc<GlobalPaths> globalPaths,
    IAppsCatalog appsCatalog,
    LazySvc<IUserService> usersSvc)
    : ServiceWithSetup<IAppReader?>("Sxc.CtxPrt", connect: [siteCtxSvc, languagesBackend, features, uiDataLazy, appPaths, globalPaths, appsCatalog, usersSvc]), IUiContextParts
{
    private ISite Site => field ??= siteCtxSvc.Context.Site;
    protected int ZoneId => siteCtxSvc.Context.Site.ZoneId;

    public ContextLanguageDto? GetLanguage(Ctx flags)
    {
        if (ZoneId == 0)
            return null;
        var site = Site;

        var converted = languagesBackend.Value.GetLanguagesOfApp(MyOptions);

        return new()
        {
            Current = site.CurrentCultureCode,
            Primary = converted.Any() ? site.DefaultCultureCode : site.CurrentCultureCode, // in special case when no languages are available, use the current culture to fix translation issue in UI
            List = converted,
        };
    }

    public ContextResourceWithApp? GetSystem(Ctx flags)
    {
        var result = new ContextResourceWithApp
        {
            Url = "/"
        };
        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced))
            return result;

        // Otherwise also add the global app id
        result.PrimaryApp = result.DefaultApp = new(1, 1);
        return result;
    }

    public ContextResourceWithApp? GetSite(Ctx flags)
    {
        var result = new ContextResourceWithApp
        {
            Id = Site.Id,
            Url = "//" + Site.UrlRoot + "/",
        };
        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        // Otherwise also add the global appId
        var zoneId = Site.ZoneId;
        result.DefaultApp = appsCatalog.DefaultAppIdentity(zoneId).PureIdentityExplicit();
        result.PrimaryApp = appsCatalog.PrimaryAppIdentity(zoneId).PureIdentityExplicit();
        return result;
    }

    public WebResourceDto GetPage(Ctx flags) =>
        new()
        {
            Id = EavConstants.NullId,
        };

    public ContextEnableDto GetEnable(CtxEnable ctx)
    {
        var isRealApp = MyOptions?.Specs != null && !MyOptions.Specs.IsContentApp();
        var user = siteCtxSvc.Context.User;
        var dto = new ContextEnableDto
        {
            DebugMode = user.IsSystemAdmin ||
                        features.Value.IsEnabled(BuiltInFeatures.EditUiAllowDebugModeForEditors)
        };
        if (ctx.HasFlag(CtxEnable.AppPermissions))
            dto.AppPermissions = isRealApp;
        if (ctx.HasFlag(CtxEnable.CodeEditor))
            dto.CodeEditor = user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.Query))
            dto.Query = isRealApp && user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.FormulaSave))
            dto.FormulaSave = user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.OverrideEditRestrictions))
            dto.OverrideEditRestrictions = user.IsSystemAdmin;
        return dto;
    }

    public string GetGettingStartedUrl() => EavConstants.UrlNotInitialized;

    public ContextAppDto? GetApp(Ctx flags)
    {
        if (MyOptions == null)
            return null;
        var appReader = MyOptions;
        var appSpecs = appReader.Specs;
        var paths = appPaths.Get(appReader, Site);
        var result = new ContextAppDto
        {
            Id = appSpecs.AppId,
            Name = appSpecs.Name,
            Folder = appSpecs.Folder,
            Url = paths?.Path,
            SharedUrl = paths?.PathShared
        };

        // Stop now if we don't need edit or advanced
        if (!flags.HasFlag(Ctx.AppEdit) && !flags.HasFlag(Ctx.AppAdvanced))
            return result;

        result.IsGlobalApp = appSpecs.IsGlobalSettingsApp();
        result.IsSiteApp = appSpecs.IsSiteSettingsApp();
        // only check content if not global, as that has the same id
        if (!result.IsGlobalApp)
            result.IsContentApp = appSpecs.IsContentApp();

        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced))
            return result;

        result.GettingStartedUrl = GetGettingStartedUrl();
        result.Identifier = appSpecs.NameId;

        // #SiteApp v13
        result.SettingsScope = appSpecs.IsGlobalSettingsApp()
            ? "Global"
            : appSpecs.IsSiteSettingsApp()
                ? "Site"
                : "App";

        result.Permissions = new() { Count = appReader.Specs.Metadata.Permissions.Count() };

        result.IsShared = appReader.IsShared();
        result.IsInherited = appReader.IsInherited();

        result.Icon = AppAssetThumbnail.GetUrl(appReader, paths!, globalPaths);
        return result;
    }

    public ContextUserDto GetUser(Ctx flags)
    {
        var user = siteCtxSvc.Context.User;
        var userDto = new ContextUserDto
        {
            Email = user.Email,
            Id = user.Id,
            Guid = user.Guid,
            IsSystemAdmin = user.IsSystemAdmin,
            IsAnonymous = user.IsAnonymous,
            IsSiteAdmin = user.IsSiteAdmin,
            IsContentEditor = user.IsContentEditor,
            IsContentAdmin = user.IsContentAdmin,
            Name = user.Name,
            Username = user.Username,
            Roles = flags.HasFlag(Ctx.UserRoles) ? usersSvc.Value.GetCurrentUser().Roles.Select(r => r.Name).ToListOpt() : [],
        };
        return userDto;
    }

    /// <summary>
    /// Features provided must be virtual, so that applications can override this.
    /// </summary>
    /// <returns></returns>
    public IList<FeatureDto> GetFeatures(Ctx flags)
        => uiDataLazy.Value.FeaturesDto(siteCtxSvc.Context.Permissions.IsContentAdmin, flags.HasFlag(Ctx.FeaturesForSystemTypes));
}