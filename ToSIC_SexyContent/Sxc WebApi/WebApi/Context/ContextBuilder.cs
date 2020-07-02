using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Web.ClientInfos;
using Assembly = System.Reflection.Assembly;

namespace ToSic.Sxc.WebApi.Context
{
    internal class ContextBuilder
    {
        protected readonly PortalSettings Portal;
        protected readonly ModuleInfo Module;
        protected readonly UserInfo User;
        protected readonly int ZoneId;
        protected readonly IApp App;

        public ContextBuilder(PortalSettings portal, ModuleInfo module, UserInfo user = null, int? zoneId = null, IApp app = null)
        {
            App = app;
            Portal = portal;
            Module = module;
            User = user;
            ZoneId = zoneId ?? 0;
        }

        public ContextDto Get(bool? all = null, bool? app = null, bool? enable = null, 
            bool? language = null, bool? page = null, bool? user = null, bool? site = null,
            bool? system = null)
        {
            var ctx = new ContextDto();
            // logic for activating each part
            // 1. either that switch is on
            // 2. or the null-check: all is on
            // 3. This also means if the switch is off, it's off
            if (app ?? all == true) ctx.App = GetApp();
            if (enable ?? all == true) ctx.Enable = GetEnable();
            if (language ?? all == true) ctx.Language = GetLanguage();
            if (page ?? all == true) ctx.Page = GetPage();
            if (user ?? all == true) ctx.User = GetUser();
            if (site ?? all == true) ctx.Site = GetSite();
            if (system ?? all == true) ctx.System = GetSystem();
            return ctx;
        }

        private LanguageDto GetLanguage()
        {
            if (Portal == null || ZoneId == 0) return null;
            var language = new ClientInfosLanguages(Portal, ZoneId);
            return new LanguageDto
            {
                Current = language.Current,
                Primary = language.Primary,
                All = language.All.ToDictionary(l => l.key, l => l.name),
            };
        }

        private UserDto GetUser()
        {
            if (User == null) return null;
            var tmp = new ClientInfosUser(User);
            return new UserDto
            {
                CanDesign = tmp.CanDesign,
                CanDevelop = tmp.CanDevelop
            };
        }

        private WebResourceDto GetSystem()
        {
            return new WebResourceDto
            {
                Url = VirtualPathUtility.ToAbsolute("~/")
            };
        }

        private WebResourceDto GetSite()
        {
            return new WebResourceDto
            {
                Id = Portal.PortalId,
                Url = "//" + Portal.PortalAlias.HTTPAlias + "/",
            };
        }

        private WebResourceDto GetPage()
        {
            if (Module == null) return null;
            return new WebResourceDto
            {
                Id = Module.ModuleID,
                // todo: maybe page url
                // used to be on ps.ActiveTab.FullUrl;
                // but we can't get it from there directly
            };
        }

        private WebResourceDto GetApp()
        {
            if (App == null) return null;
            return new WebResourceDto
            {
                Id = App.AppId,
                Url = App.Path,
            };
        }

        private EnableDto GetEnable()
        {
            if (App == null) return null;
            return new EnableDto
            {
                App = App.AppGuid != Eav.Constants.DefaultAppName
            };
        }

        /// <summary>
        /// build a getting-started url which is used to correctly show the user infos like
        /// warnings related to his dnn or 2sxc version
        /// infos based on his languages
        /// redirects based on the app he's looking at, etc.
        /// </summary>
        /// <returns></returns>
        internal string IntroductionToAppUrl()
        {
            if (App == null) return "";

            var gsUrl =
                "//gettingstarted.2sxc.org/router.aspx?" +
                $"DnnVersion={Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)}" +
                $"&2SexyContentVersion={Settings.ModuleVersion}" +
                $"&ModuleName={Module.DesktopModule.ModuleName}" +
                $"&ModuleId={Module.ModuleID}" +
                $"&PortalID={Portal.PortalId}" +
                $"&ZoneID={App.ZoneId}" +
                $"&DefaultLanguage={Portal.DefaultLanguage}" +
                $"&CurrentLanguage={Portal.CultureCode}";

            // Add AppStaticName and Version
            if (Module.DesktopModule.ModuleName != "2sxc")
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
