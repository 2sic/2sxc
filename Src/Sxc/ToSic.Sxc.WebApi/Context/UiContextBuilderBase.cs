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

            public Dependencies(IContextOfSite siteCtx, JsContextLanguage jsCtx, Apps.App appToLaterInitialize)
            {
                SiteCtx = siteCtx;
                JsCtx = jsCtx;
                AppToLaterInitialize = appToLaterInitialize;
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
            if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite();
            if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem();
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

        protected virtual WebResourceDto GetSystem() =>
            new WebResourceDto
            {
                Url = "/"
            };

        protected virtual WebResourceDto GetSite() =>
            new WebResourceDto
            {
                Id = Deps.SiteCtx.Site.Id,
                Url = "//" + Deps.SiteCtx.Site.Url + "/",
            };

        protected virtual WebResourceDto GetPage() =>
            new WebResourceDto
            {
                Id = Eav.Constants.NullId,
            };

        protected virtual ContextEnableDto GetEnable(CtxEnable ctx)
        {
            var isRealApp = App != null && App.AppGuid != Eav.Constants.DefaultAppName;
            var tmp = new JsContextUser(Deps.SiteCtx.User);
            var dto = new ContextEnableDto();
            if (ctx.HasFlag(CtxEnable.AppPermissions)) dto.AppPermissions = isRealApp;
            if (ctx.HasFlag(CtxEnable.CodeEditor)) dto.CodeEditor = tmp.CanDevelop;
            if(ctx.HasFlag(CtxEnable.Query)) dto.Query = isRealApp && tmp.CanDevelop;
            if (ctx.HasFlag(CtxEnable.FormulaSave)) dto.FormulaSave = tmp.CanDevelop;
            return dto;
            //return new ContextEnableDto
            //{
            //    AppPermissions = isRealApp,
            //    CodeEditor = tmp.CanDevelop,
            //    Query = isRealApp,
            //};
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
            if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

            result.GettingStartedUrl = GetGettingStartedUrl();
            result.Identifier = _appToLaterInitialize.AppGuid;
            result.Permissions = new HasPermissionsDto {Count = App.Metadata.Permissions.Count()};
            return result;
        }
    }
}
