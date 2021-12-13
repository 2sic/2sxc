using System.Linq;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.WebApi.Context
{
    public class UiContextBuilderBase: IUiContextBuilder
    {

        #region Dependencies 

        public class Dependencies
        {
            public IContextOfSite SiteCtx { get; }
            public JsContextLanguage JsCtx { get; }
            public Apps.IApp AppToLaterInitialize { get; }
            public IAppStates AppStates { get; }

            public Dependencies(IContextOfSite siteCtx, JsContextLanguage jsCtx, Apps.App appToLaterInitialize, IAppStates appStates)
            {
                SiteCtx = siteCtx;
                JsCtx = jsCtx;
                AppToLaterInitialize = appToLaterInitialize;
                AppStates = appStates;
            }
        }

        #endregion

        #region Constructor / DI

        protected UiContextBuilderBase(Dependencies dependencies)
        {
            Deps = dependencies;
            _appToLaterInitialize = dependencies.AppToLaterInitialize;
        }
        protected Dependencies Deps;

        protected int ZoneId;
        protected IApp App;
        private readonly Apps.IApp _appToLaterInitialize;

        #endregion

        public virtual IUiContextBuilder SetZoneAndApp(int zoneId, IAppIdentity app)
        {
            ZoneId = zoneId;
            App = app != null ? (_appToLaterInitialize as Apps.App)?.InitNoData(app, null) : null;
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
            var language = Deps.JsCtx.Init(Deps.SiteCtx.Site, ZoneId);
            return new ContextLanguageDto
            {
                Current = language.Current,
                Primary = language.Primary,
                All = language.All.ToDictionary(l => l.key.ToLowerInvariant(), l => l.name),
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
            result.DefaultApp = Deps.AppStates.IdentityOfDefault(zoneId); // Deps.AppStates.Identity(null, Deps.AppStates.DefaultAppId(Deps.SiteCtx.Site.ZoneId));
            result.PrimaryApp = Deps.AppStates.IdentityOfPrimary(zoneId); // Deps.AppStates.Identity(null, Deps.AppStates.PrimaryAppId(Deps.SiteCtx.Site.ZoneId));
            return result;
        }

        protected virtual WebResourceDto GetPage() =>
            new WebResourceDto
            {
                Id = Eav.Constants.NullId,
            };

        protected virtual ContextEnableDto GetEnable(CtxEnable ctx)
        {
            var isRealApp = App != null && (App.AppGuid != Eav.Constants.DefaultAppGuid /*&& App.AppGuid != Eav.Constants.PrimaryAppGuid*/); // #SiteApp v13 - Site-Apps should also have permissions
            var tmp = new JsContextUser(Deps.SiteCtx.User);
            var dto = new ContextEnableDto();
            if (ctx.HasFlag(CtxEnable.AppPermissions)) dto.AppPermissions = isRealApp;
            if (ctx.HasFlag(CtxEnable.CodeEditor)) dto.CodeEditor = tmp.CanDevelop;
            if(ctx.HasFlag(CtxEnable.Query)) dto.Query = isRealApp && tmp.CanDevelop;
            if (ctx.HasFlag(CtxEnable.FormulaSave)) dto.FormulaSave = tmp.CanDevelop;
            return dto;
        }

        protected virtual string GetGettingStartedUrl() => Eav.Constants.UrlNotInitialized;

        protected virtual ContextAppDto GetApp(Ctx flags)
        {
            if (App == null) return null;
            var result = new ContextAppDto
            {
                Id = App.AppId,
                Url = (App as Apps.IApp)?.Path,
                Name = App.Name,
                Folder = App.Folder,
            };

            // Stop now if we don't need advanced infos
            if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

            result.GettingStartedUrl = GetGettingStartedUrl();
            result.Identifier = _appToLaterInitialize.AppGuid;
            // TODO: #SiteApp v13
            result.SettingsScope = App.AppId == 1 ? "Global" : result.Identifier == Eav.Constants.DefaultAppGuid ? "Site" : "App";


            result.Permissions = new HasPermissionsDto {Count = App.Metadata.Permissions.Count()};
            return result;
        }
    }
}
