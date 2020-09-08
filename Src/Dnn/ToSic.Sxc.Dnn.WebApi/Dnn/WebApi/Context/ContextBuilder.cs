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

        public DnnContextBuilder(PortalSettings portal, ModuleInfo module, UserInfo user = null, int? zoneId = null, IApp app = null)
        {
            _portal = portal;
            _module = module;
            _user = user;
            InitApp(zoneId, app);
        }

        public override IContextBuilder InitApp(int? zoneId, IApp app)
        {
            // check if we're providing context for missing app
            // in this case we must find the zone based on the portals.
            if ((zoneId ?? 0) == 0 && app == null) zoneId = new DnnZoneMapper().Init(null).GetZoneId(_portal.PortalId);
            return base.InitApp(zoneId, app);
        }

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

        protected override WebResourceDto GetSystem() =>
            new WebResourceDto
            {
                Url = VirtualPathUtility.ToAbsolute("~/")
            };

        protected override WebResourceDto GetSite() =>
            new WebResourceDto
            {
                Id = _portal.PortalId,
                Url = "//" + _portal.PortalAlias.HTTPAlias + "/",
            };

        protected override WebResourceDto GetPage() =>
            _module == null ? null
                : new WebResourceDto
                {
                    Id = _module.TabID, // .ModuleID,
                    // todo: maybe page url
                    // used to be on ps.ActiveTab.FullUrl;
                    // but we can't get it from there directly
                };

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
