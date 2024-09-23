using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.Apps.Internal.Specs;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Languages;
using ToSic.Sxc.Apps;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.Backend.Context;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UiContextBuilderBase(UiContextBuilderBase.MyServices services)
    : ServiceBase<UiContextBuilderBase.MyServices>(services, SxcLogName + ".UiCtx"), IUiContextBuilder
{
    #region Dependencies 

    public class MyServices(
        IContextOfSite siteCtx,
        LazySvc<IEavFeaturesService> features,
        LazySvc<IUiData> uiDataLazy,
        LazySvc<LanguagesBackend> languagesBackend,
        IAppPathsMicroSvc appPaths,
        LazySvc<GlobalPaths> globalPaths,
        IAppsCatalog appsCatalog
    ) : MyServicesBase(connect: [siteCtx, features, uiDataLazy, appPaths, languagesBackend, globalPaths, appsCatalog])
    {
        public LazySvc<GlobalPaths> GlobalPaths { get; } = globalPaths;
        public IAppPathsMicroSvc AppPaths { get; } = appPaths;
        public IContextOfSite SiteCtx { get; } = siteCtx;
        public LazySvc<LanguagesBackend> LanguagesBackend { get; } = languagesBackend;
        public LazySvc<IEavFeaturesService> Features { get; } = features;
        public LazySvc<IUiData> UiDataLazy { get; } = uiDataLazy;
        public IAppsCatalog AppsCatalog { get; } = appsCatalog;
    }

    #endregion

    #region Constructor / DI

    protected int ZoneId => Services.SiteCtx.Site.ZoneId;
    protected IAppSpecs AppSpecsOrNull;
    private IAppReader _appReaderOrNull;

    #endregion

    public IUiContextBuilder InitApp(IAppReader appState)
    {
        AppSpecsOrNull = appState?.Specs;
        _appReaderOrNull = appState;
        return this;
    }

    /// <inheritdoc />
    public ContextDto Get(Ctx flags, CtxEnable enableFlags)
    {
        var ctx = new ContextDto();
        // logic for activating each part
        // 1. either that switch is on
        // 2. or the null-check: all is on
        // 3. This also means if the switch is off, it's off
        if (flags.HasFlag(Ctx.AppBasic) | flags.HasFlag(Ctx.AppAdvanced))
            ctx.App = GetApp(flags);
        if (enableFlags != CtxEnable.None) ctx.Enable = GetEnable(enableFlags);
        if (flags.HasFlag(Ctx.Language)) ctx.Language = GetLanguage();
        if (flags.HasFlag(Ctx.Page)) ctx.Page = GetPage();
        if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite(flags);
        if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem(flags);
        if (flags.HasFlag(Ctx.User)) ctx.User = GetUser(flags);
        if (flags.HasFlag(Ctx.Features) || flags.HasFlag(Ctx.FeaturesForSystemTypes)) ctx.Features = GetFeatures(flags);
        return ctx;
    }

    protected virtual ContextLanguageDto GetLanguage()
    {
        if (ZoneId == 0) return null;
        var site = Services.SiteCtx.Site;

        var converted = Services.LanguagesBackend.Value.GetLanguagesOfApp(_appReaderOrNull);

        return new()
        {
            Current = site.CurrentCultureCode,
            Primary = converted.Any() ? site.DefaultCultureCode : site.CurrentCultureCode, // in special case when no languages are available, use the current culture to fix translation issue in UI
            List = converted, 
        };
    }

    protected virtual ContextResourceWithApp GetSystem(Ctx flags)
    {
        var result = new ContextResourceWithApp
        {
            Url = "/"
        };
        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        // Otherwise also add the global app id
        result.PrimaryApp = result.DefaultApp = new(1, 1);
        return result;
    }

    protected virtual ContextResourceWithApp GetSite(Ctx flags)
    {
        var result = new ContextResourceWithApp
        {
            Id = Services.SiteCtx.Site.Id,
            Url = "//" + Services.SiteCtx.Site.UrlRoot + "/",
        };
        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        // Otherwise also add the global appId
        var zoneId = Services.SiteCtx.Site.ZoneId;
        result.DefaultApp = (AppIdentity)Services.AppsCatalog.DefaultAppIdentity(zoneId);
        result.PrimaryApp = (AppIdentity)Services.AppsCatalog.PrimaryAppIdentity(zoneId);
        return result;
    }

    protected virtual WebResourceDto GetPage() =>
        new()
        {
            Id = Eav.Constants.NullId,
        };

    protected virtual ContextEnableDto GetEnable(CtxEnable ctx)
    {
        var isRealApp = AppSpecsOrNull != null && ! AppSpecsOrNull.IsContentApp();
        var user = Services.SiteCtx.User;
        var dto = new ContextEnableDto
        {
            DebugMode = user.IsSystemAdmin ||
                        Services.Features.Value.IsEnabled(EditUiAllowDebugModeForEditors)
        };
        if (ctx.HasFlag(CtxEnable.AppPermissions)) dto.AppPermissions = isRealApp;
        if (ctx.HasFlag(CtxEnable.CodeEditor)) dto.CodeEditor = user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.Query)) dto.Query = isRealApp && user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.FormulaSave)) dto.FormulaSave = user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.OverrideEditRestrictions)) dto.OverrideEditRestrictions = user.IsSystemAdmin;
        return dto;
    }

    protected virtual string GetGettingStartedUrl() => Eav.Constants.UrlNotInitialized;

    protected virtual ContextAppDto GetApp(Ctx flags)
    {
        if (_appReaderOrNull == null) return null;
        var appReader = _appReaderOrNull;
        var appSpecs = appReader.Specs;
        var paths = Services.AppPaths.Get(appReader, Services.SiteCtx.Site);
        var result = new ContextAppDto
        {
            Id = appSpecs.AppId,
            Name = appSpecs.Name,
            Folder = appSpecs.Folder,
            Url = paths?.Path,
            SharedUrl = paths?.PathShared
        };

        // Stop now if we don't need edit or advanced
        if (!flags.HasFlag(Ctx.AppEdit) && !flags.HasFlag(Ctx.AppAdvanced)) return result;

        result.IsGlobalApp = AppSpecsOrNull.IsGlobalSettingsApp();
        result.IsSiteApp = AppSpecsOrNull.IsSiteSettingsApp();
        // only check content if not global, as that has the same id
        if (!result.IsGlobalApp) result.IsContentApp = AppSpecsOrNull.IsContentApp();

        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        result.GettingStartedUrl = GetGettingStartedUrl();
        result.Identifier = AppSpecsOrNull.NameId;
            
        // #SiteApp v13
        result.SettingsScope = AppSpecsOrNull.IsGlobalSettingsApp()
            ? "Global" 
            : AppSpecsOrNull.IsSiteSettingsApp()
                ? "Site" 
                : "App";

        result.Permissions = new() { Count = appReader.Specs.Metadata.Permissions.Count() };

        result.IsShared = appReader.IsShared();
        result.IsInherited = appReader.IsInherited();

        result.Icon = AppAssetThumbnail.GetUrl(appReader, paths, Services.GlobalPaths);
        return result;
    }

    protected virtual ContextUserDto GetUser(Ctx flags)
    {
        var userDto = new ContextUserDto();
        var user = Services.SiteCtx.User;
        userDto.Email = user.Email;
        userDto.Id = user.Id;
        userDto.Guid = user.Guid;
        userDto.IsSystemAdmin = user.IsSystemAdmin;
        userDto.IsAnonymous = user.IsAnonymous;
        userDto.IsSiteAdmin = user.IsSiteAdmin;
        userDto.IsContentAdmin = user.IsContentAdmin;
        userDto.Name = user.Name;
        userDto.Username = user.Username;
        return userDto;
    }

    /// <summary>
    /// Features provided must be virtual, so that applications can override this.
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    protected virtual IList<FeatureDto> GetFeatures(Ctx flags)
        => Services.UiDataLazy.Value.FeaturesDto(Services.SiteCtx.Permissions.IsContentAdmin, flags.HasFlag(Ctx.FeaturesForSystemTypes));
}