using System;
using System.Linq;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Languages;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Languages;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Web.JsContext;
using static ToSic.Eav.Configuration.BuiltInFeatures;

namespace ToSic.Sxc.WebApi.Context
{
    public class UiContextBuilderBase: HasLog, IUiContextBuilder
    {

        #region Dependencies 

        public class Dependencies
        {
            public IContextOfSite SiteCtx { get; }
            public Apps.IApp AppToLaterInitialize { get; }
            public IAppStates AppStates { get; }
            public Lazy<AppUserLanguageCheck> AppUserLanguageCheck { get; }
            public Lazy<LanguagesBackend> LanguagesBackend { get; }
            public Lazy<IFeaturesInternal> Features { get; }

            public Dependencies(
                IContextOfSite siteCtx, 
                Apps.App appToLaterInitialize, 
                IAppStates appStates, 
                Lazy<AppUserLanguageCheck> appUserLanguageCheck, 
                Lazy<LanguagesBackend> languagesBackend,
                Lazy<IFeaturesInternal> features
            )
            {
                SiteCtx = siteCtx;
                AppToLaterInitialize = appToLaterInitialize;
                AppStates = appStates;
                AppUserLanguageCheck = appUserLanguageCheck;
                LanguagesBackend = languagesBackend;
                Features = features;
            }
        }

        #endregion

        #region Constructor / DI

        protected UiContextBuilderBase(Dependencies dependencies): base(Constants.SxcLogName + ".UiCtx")
        {
            Deps = dependencies;
            _appToLaterInitialize = dependencies.AppToLaterInitialize;
        }
        protected Dependencies Deps;

        protected int ZoneId => Deps.SiteCtx.Site.ZoneId;
        protected Sxc.Apps.IApp App;
        private readonly Apps.IApp _appToLaterInitialize;
        protected AppState AppState;

        #endregion

        public IUiContextBuilder InitApp(AppState appState, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AppState = appState;
            App = appState != null ? (_appToLaterInitialize as Apps.App)?.Init(appState, null, Log) : null;
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
            //if (flags.HasFlag(Ctx.Enable)) 
            if(enableFlags != CtxEnable.None) ctx.Enable = GetEnable(enableFlags);
            if (flags.HasFlag(Ctx.Language)) ctx.Language = GetLanguage();
            if (flags.HasFlag(Ctx.Page)) ctx.Page = GetPage();
            if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite(flags);
            if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem(flags);
            return ctx;
        }

        protected virtual ContextLanguageDto GetLanguage()
        {
            if (ZoneId == 0) return null;
            var site = Deps.SiteCtx.Site;

            var converted = Deps.LanguagesBackend.Value.Init(Log).GetLanguagesOfApp(AppState);

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
                Id = Deps.SiteCtx.Site.Id,
                Url = "//" + Deps.SiteCtx.Site.UrlRoot + "/",
            };
            // Stop now if we don't need advanced infos
            if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

            // Otherwise also add the global appId
            var zoneId = Deps.SiteCtx.Site.ZoneId;
            result.DefaultApp = Deps.AppStates.IdentityOfDefault(zoneId);
            result.PrimaryApp = Deps.AppStates.IdentityOfPrimary(zoneId);
            return result;
        }

        protected virtual WebResourceDto GetPage() =>
            new WebResourceDto
            {
                Id = Eav.Constants.NullId,
            };

        protected virtual ContextEnableDto GetEnable(CtxEnable ctx)
        {
            var isRealApp = AppState != null && !AppState.IsContentApp();// App.NameId != Eav.Constants.DefaultAppGuid; // #SiteApp v13 - Site-Apps should also have permissions
            var tmp = new JsContextUser(Deps.SiteCtx.User);
            var dto = new ContextEnableDto
            {
                DebugMode = tmp.CanDevelop ||
                            Deps.Features.Value.IsEnabled(EditUiAllowDebugModeForEditors)
            };
            if (ctx.HasFlag(CtxEnable.AppPermissions)) dto.AppPermissions = isRealApp;
            if (ctx.HasFlag(CtxEnable.CodeEditor)) dto.CodeEditor = tmp.CanDevelop;
            if(ctx.HasFlag(CtxEnable.Query)) dto.Query = isRealApp && tmp.CanDevelop;
            if (ctx.HasFlag(CtxEnable.FormulaSave)) dto.FormulaSave = tmp.CanDevelop;
            if (ctx.HasFlag(CtxEnable.OverrideEditRestrictions)) dto.OverrideEditRestrictions = tmp.CanDevelop;
            return dto;
        }

        protected virtual string GetGettingStartedUrl() => Eav.Constants.UrlNotInitialized;

        protected virtual ContextAppDto GetApp(Ctx flags)
        {
            if (AppState == null || App == null) return null;
            var result = new ContextAppDto
            {
                Id = AppState.AppId,
                Name = AppState.Name,
                Folder = AppState.Folder,
                Url = App?.Path,
                SharedUrl = App?.PathShared
            };

            // Stop now if we don't need edit or advanced
            if (!flags.HasFlag(Ctx.AppEdit) && !flags.HasFlag(Ctx.AppAdvanced)) return result;

            result.IsGlobalApp = AppState.IsGlobalSettingsApp();
            result.IsSiteApp = AppState.IsSiteSettingsApp();
            // only check content if not global, as that has the same id
            if (!result.IsGlobalApp) result.IsContentApp = AppState.IsContentApp();

            // Stop now if we don't need advanced infos
            if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

            result.GettingStartedUrl = GetGettingStartedUrl();
            result.Identifier = AppState.NameId;
            
            // #SiteApp v13
            result.SettingsScope = AppState.IsGlobalSettingsApp()
                ? "Global" 
                : AppState.IsSiteSettingsApp()
                    ? "Site" 
                    : "App";

            result.Permissions = new HasPermissionsDto { Count = AppState.Metadata.Permissions.Count() };

            result.IsGlobal = AppState.IsShared();
            result.IsShared = result.IsGlobal;
            result.IsInherited = AppState.IsInherited();
            return result;
        }
    }
}
