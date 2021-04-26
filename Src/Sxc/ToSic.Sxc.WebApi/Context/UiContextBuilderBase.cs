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
            public Apps.App AppToLaterInitialize { get; }

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
        private readonly Apps.App _appToLaterInitialize;

        #endregion

        public virtual IUiContextBuilder SetZoneAndApp(int zoneId, IAppIdentity app)
        {
            ZoneId = zoneId;
            App = app != null ? _appToLaterInitialize.InitNoData(app, null) : null;
            return this;
        }

        public ContextDto Get(Ctx flags)
        {
            var ctx = new ContextDto();
            // logic for activating each part
            // 1. either that switch is on
            // 2. or the null-check: all is on
            // 3. This also means if the switch is off, it's off
            if (flags.HasFlag(Ctx.AppBasic) | flags.HasFlag(Ctx.AppAdvanced))
                ctx.App = GetApp(flags);
            if (flags.HasFlag(Ctx.Enable)) ctx.Enable = GetEnable();
            if (flags.HasFlag(Ctx.Language)) ctx.Language = GetLanguage();
            if (flags.HasFlag(Ctx.Page)) ctx.Page = GetPage();
            if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite();
            if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem();
            return ctx;
        }

        protected virtual LanguageDto GetLanguage()
        {
            if (ZoneId == 0) return null;
            var language = Deps.JsCtx.Init(Deps.SiteCtx.Site, ZoneId);
            return new LanguageDto
            {
                Current = language.Current,
                Primary = language.Primary,
                All = language.All.ToDictionary(l => l.key, l => l.name),
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

        protected virtual EnableDto GetEnable()
        {
            var isRealApp = App != null && App.AppGuid != Eav.Constants.DefaultAppName;
            var tmp = new JsContextUser(Deps.SiteCtx.User);
            return new EnableDto
            {
                AppPermissions = isRealApp,
                CodeEditor = tmp.CanDevelop,
                Query = isRealApp,
            };
        }

        protected virtual string GetGettingStartedUrl() => Eav.Constants.UrlNotInitialized;

        protected string BaseGettingStartedUrl(string platform, string sysVersion, string moduleName, int moduleId, string primaryLang, string currentLang)
        {
            var gsUrl =
                "//gettingstarted.2sxc.org/router.aspx?" +
                $"Platform={platform}" +
                $"&SysVersion={sysVersion}" +
                $"&SxcVersion={Settings.ModuleVersion}" +
                //$"&ModuleName={moduleName}" +
                $"&ModuleId={moduleId}" +
                $"&SiteId={Deps.SiteCtx.Site.Id}" +
                $"&ZoneID={Deps.AppToLaterInitialize?.ZoneId}" +

                $"&DefaultLanguage={primaryLang}" +
                $"&CurrentLanguage={currentLang}";
            return gsUrl;
        }

        protected virtual AppDto GetApp(Ctx flags)
        {
            if (App == null) return null;
            var result = new AppDto
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
