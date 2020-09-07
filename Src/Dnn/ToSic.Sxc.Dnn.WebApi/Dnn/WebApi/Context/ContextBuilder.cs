using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web.JsContext;
using ToSic.Sxc.WebApi.Context;
using Assembly = System.Reflection.Assembly;

namespace ToSic.Sxc.Dnn.WebApi.Context
{

    internal sealed class DnnContextBuilder : ContextBuilderBase
    {
        private readonly PortalSettings _portal;
        private readonly ModuleInfo _module;
        private readonly UserInfo _user;
        //protected int ZoneId;
        //protected IApp App;

        public DnnContextBuilder(PortalSettings portal, ModuleInfo module, UserInfo user = null, int? zoneId = null, IApp app = null)
        {
            _portal = portal;
            _module = module;
            _user = user;
            InitApp(zoneId, app);
        }

        public override IContextBuilder InitApp(int? zoneId, IApp app)
        {
            //App = app;
            //ZoneId = zoneId ?? 0;

            // check if we're providing context for missing app
            // in this case we must find the zone based on the portals.
            if ((zoneId ?? 0) == 0 && app == null) zoneId = new DnnZoneMapper().Init(null).GetZoneId(_portal.PortalId);
            return base.InitApp(zoneId, app);
            
            //return this;
        }

        //public ContextDto Get(Ctx flags)
        //{
        //    var ctx = new ContextDto();
        //    // logic for activating each part
        //    // 1. either that switch is on
        //    // 2. or the null-check: all is on
        //    // 3. This also means if the switch is off, it's off
        //    if (flags.HasFlag(Ctx.AppBasic) | flags.HasFlag(Ctx.AppAdvanced)) 
        //        ctx.App = GetApp(flags);
        //    if (flags.HasFlag(Ctx.Enable)) ctx.Enable = GetEnable();
        //    if (flags.HasFlag(Ctx.Language)) ctx.Language = GetLanguage();
        //    if (flags.HasFlag(Ctx.Page)) ctx.Page = GetPage();
        //    if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite();
        //    if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem();
        //    return ctx;
        //}

        protected override LanguageDto GetLanguage()
        {
            if (_portal == null || ZoneId == 0) return null;
            var language = new JsContextLanguage(new DnnTenant(_portal), ZoneId);
            return new LanguageDto
            {
                Current = language.Current,
                Primary = language.Primary,
                All = language.All.ToDictionary(l => l.key, l => l.name),
            };
        }

        //private UserDto GetUser()
        //{
        //    if (User == null) return null;
        //    var tmp = new ClientInfosUser(User);
        //    return new UserDto
        //    {
        //        CanDesign = tmp.CanDesign,
        //        CanDevelop = tmp.CanDevelop
        //    };
        //}

        protected override WebResourceDto GetSystem()
        {
            return new WebResourceDto
            {
                Url = VirtualPathUtility.ToAbsolute("~/")
            };
        }

        protected override WebResourceDto GetSite()
        {
            return new WebResourceDto
            {
                Id = _portal.PortalId,
                Url = "//" + _portal.PortalAlias.HTTPAlias + "/",
            };
        }

        protected override WebResourceDto GetPage()
        {
            if (_module == null) return null;
            return new WebResourceDto
            {
                Id = _module.TabID, // .ModuleID,
                // todo: maybe page url
                // used to be on ps.ActiveTab.FullUrl;
                // but we can't get it from there directly
            };
        }

        //private AppDto GetApp(Ctx flags)
        //{
        //    if (App == null) return null;
        //    var result = new AppDto
        //    {
        //        Id = App.AppId,
        //        Url = App.Path,
        //        Name = App.Name,
        //    };
        //    if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        //    result.GettingStartedUrl = GettingStartedUrl();
        //    result.Identifier = App.AppGuid;
        //    return result;
        //}

        protected override EnableDto GetEnable()
        {
            var isRealApp = App != null && App.AppGuid != Eav.Constants.DefaultAppName;
            var tmp = _user == null ? null : new JsContextUser(new DnnUser(_user));
            return new EnableDto
            {
                AppPermissions = isRealApp,
                CodeEditor = tmp?.CanDevelop ?? false,
                Query = isRealApp,
            };
        }

        /// <summary>
        /// build a getting-started url which is used to correctly show the user infos like
        /// warnings related to his dnn or 2sxc version
        /// infos based on his languages
        /// redirects based on the app he's looking at, etc.
        /// </summary>
        /// <returns></returns>
        public string GettingStartedUrl() => GetGettingStartedUrl();

        protected override string GetGettingStartedUrl()
        {
            if (App == null) return "";

            var gsUrl =
                "//gettingstarted.2sxc.org/router.aspx?" +
                $"DnnVersion={Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)}" +
                $"&2SexyContentVersion={Settings.ModuleVersion}" +
                $"&ModuleName={_module.DesktopModule.ModuleName}" +
                $"&ModuleId={_module.ModuleID}" +
                $"&PortalID={_portal.PortalId}" +
                $"&ZoneID={App.ZoneId}" +
                $"&DefaultLanguage={_portal.DefaultLanguage}" +
                $"&CurrentLanguage={_portal.CultureCode}";

            // Add AppStaticName and Version
            if (_module.DesktopModule.ModuleName != "2sxc")
            {
                gsUrl += "&AppGuid=" + App.AppGuid;
                if (App.Configuration != null)
                    gsUrl += $"&AppVersion={App.Configuration.Version}&AppOriginalId={App.Configuration.OriginalId}";
            }

            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gsUrl += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
            return gsUrl;
        }
    }
}
