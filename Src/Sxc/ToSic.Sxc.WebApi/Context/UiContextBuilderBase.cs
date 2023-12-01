using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Languages;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Web.JsContext;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.WebApi.Context;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UiContextBuilderBase: ServiceBase<UiContextBuilderBase.MyServices>, IUiContextBuilder
{

    #region Dependencies 

    public class MyServices: MyServicesBase
    {
        public LazySvc<GlobalPaths> GlobalPaths { get; }
        public IAppPathsMicroSvc AppPaths { get; }
        public IContextOfSite SiteCtx { get; }
        public IAppStates AppStates { get; }
        public LazySvc<LanguagesBackend> LanguagesBackend { get; }
        public LazySvc<IEavFeaturesService> Features { get; }
        public LazySvc<IUiData> UiDataLazy { get; }

        public MyServices(
            IContextOfSite siteCtx,
            IAppStates appStates,
            LazySvc<IEavFeaturesService> features,
            LazySvc<IUiData> uiDataLazy,
            LazySvc<LanguagesBackend> languagesBackend,
            IAppPathsMicroSvc appPaths,
            LazySvc<GlobalPaths> globalPaths
        )
        {
            ConnectServices(
                SiteCtx = siteCtx, AppStates = appStates,
                Features = features,
                UiDataLazy = uiDataLazy,
                AppPaths = appPaths,
                LanguagesBackend = languagesBackend,
                GlobalPaths = globalPaths
            );
        }
    }

    #endregion

    #region Constructor / DI

    protected UiContextBuilderBase(MyServices services): base(services, Constants.SxcLogName + ".UiCtx")
    {
    }

    protected int ZoneId => Services.SiteCtx.Site.ZoneId;
    protected IAppStateInternal AppStateOrNull;

    #endregion

    public IUiContextBuilder InitApp(IAppState appState)
    {
        AppStateOrNull = appState?.Internal();
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
        if (flags.HasFlag(Ctx.Features)) ctx.Features = GetFeatures();
        return ctx;
    }

    protected virtual ContextLanguageDto GetLanguage()
    {
        if (ZoneId == 0) return null;
        var site = Services.SiteCtx.Site;

        var converted = Services.LanguagesBackend.Value.GetLanguagesOfApp(AppStateOrNull);

        return new ContextLanguageDto
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
        result.PrimaryApp = result.DefaultApp = new AppIdentity(1, 1);
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
        result.DefaultApp = (AppIdentity)Services.AppStates.IdentityOfDefault(zoneId);
        result.PrimaryApp = (AppIdentity)Services.AppStates.IdentityOfPrimary(zoneId);
        return result;
    }

    protected virtual WebResourceDto GetPage() =>
        new()
        {
            Id = Eav.Constants.NullId,
        };

    protected virtual ContextEnableDto GetEnable(CtxEnable ctx)
    {
        var isRealApp = AppStateOrNull != null && !AppStateOrNull.IsContentApp();// App.NameId != Eav.Constants.DefaultAppGuid; // #SiteApp v13 - Site-Apps should also have permissions
        var tmp = new JsContextUser(Services.SiteCtx.User);
        var dto = new ContextEnableDto
        {
            DebugMode = tmp.CanDevelop ||
                        Services.Features.Value.IsEnabled(EditUiAllowDebugModeForEditors)
        };
        if (ctx.HasFlag(CtxEnable.AppPermissions)) dto.AppPermissions = isRealApp;
        if (ctx.HasFlag(CtxEnable.CodeEditor)) dto.CodeEditor = tmp.CanDevelop;
        if (ctx.HasFlag(CtxEnable.Query)) dto.Query = isRealApp && tmp.CanDevelop;
        if (ctx.HasFlag(CtxEnable.FormulaSave)) dto.FormulaSave = tmp.CanDevelop;
        if (ctx.HasFlag(CtxEnable.OverrideEditRestrictions)) dto.OverrideEditRestrictions = tmp.CanDevelop;
        return dto;
    }

    protected virtual string GetGettingStartedUrl() => Eav.Constants.UrlNotInitialized;

    protected virtual ContextAppDto GetApp(Ctx flags)
    {
        if (AppStateOrNull == null) return null;
        var paths = Services.AppPaths.Init(Services.SiteCtx.Site, AppStateOrNull);
        var result = new ContextAppDto
        {
            Id = AppStateOrNull.AppId,
            Name = AppStateOrNull.Name,
            Folder = AppStateOrNull.Folder,
            Url = paths?.Path,
            SharedUrl = paths?.PathShared
        };

        // Stop now if we don't need edit or advanced
        if (!flags.HasFlag(Ctx.AppEdit) && !flags.HasFlag(Ctx.AppAdvanced)) return result;

        result.IsGlobalApp = AppStateOrNull.IsGlobalSettingsApp();
        result.IsSiteApp = AppStateOrNull.IsSiteSettingsApp();
        // only check content if not global, as that has the same id
        if (!result.IsGlobalApp) result.IsContentApp = AppStateOrNull.IsContentApp();

        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        result.GettingStartedUrl = GetGettingStartedUrl();
        result.Identifier = AppStateOrNull.NameId;
            
        // #SiteApp v13
        result.SettingsScope = AppStateOrNull.IsGlobalSettingsApp()
            ? "Global" 
            : AppStateOrNull.IsSiteSettingsApp()
                ? "Site" 
                : "App";

        result.Permissions = new HasPermissionsDto { Count = AppStateOrNull.Metadata.Permissions.Count() };

        result.IsShared = AppStateOrNull.IsShared();
        result.IsInherited = AppStateOrNull.IsInherited();

        result.Icon = AppAssetThumbnail.GetUrl(AppStateOrNull, paths, Services.GlobalPaths); // App?.Thumbnail;
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

    protected virtual IList<FeatureDto> GetFeatures() => Services.UiDataLazy.Value.FeaturesDto(Services.SiteCtx.UserMayEdit);
}