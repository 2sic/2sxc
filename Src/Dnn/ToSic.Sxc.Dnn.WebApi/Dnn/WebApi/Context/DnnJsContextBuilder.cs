using System;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web.JsContext;
using ToSic.Sxc.WebApi.Context;
using Assembly = System.Reflection.Assembly;

namespace ToSic.Sxc.Dnn.WebApi.Context
{
    public sealed class DnnJsContextBuilder : JsContextBuilderBase
    {
        #region Constructor / DI


        private readonly IServiceProvider _serviceProvider;
        private readonly IContextResolver _ctxResolver;
        private readonly PortalSettings _portal = PortalSettings.Current;

        private ModuleInfo Module => (_ctxResolver.BlockOrNull()?.Module as DnnModule)?.UnwrappedContents;

        public DnnJsContextBuilder(IServiceProvider serviceProvider, IContextResolver ctxResolver)
        {
            _serviceProvider = serviceProvider;
            _ctxResolver = ctxResolver;
            InitApp(null, null);
        }

        #endregion

        public override IJsContextBuilder InitApp(int? zoneId, IApp app)
        {
            // check if we're providing context for missing app
            // in this case we must find the zone based on the portals.
            if ((zoneId ?? 0) == 0 && app == null) zoneId = _serviceProvider.Build<DnnZoneMapper>().Init(null).GetZoneId(_portal.PortalId);
            return base.InitApp(zoneId, app);
        }

        protected override LanguageDto GetLanguage()
        {
            if (_portal == null || ZoneId == 0) return null;
            var language = new JsContextLanguage(_serviceProvider, new DnnSite(), ZoneId);
            return new LanguageDto
            {
                Current = language.Current,
                Primary = language.Primary,
                All = language.All.ToDictionary(l => l.key, l => l.name),
            };
        }

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
            Module == null ? null
                : new WebResourceDto
                {
                    Id = Module.TabID,
                    // todo: maybe page url
                    // used to be on ps.ActiveTab.FullUrl;
                    // but we can't get it from there directly
                };

        protected override EnableDto GetEnable()
        {
            var isRealApp = App != null && App.AppGuid != Eav.Constants.DefaultAppName;
            var tmp = new JsContextUser(new DnnUser());
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
            var app = App as Sxc.Apps.IApp;
            if (app == null) return "";

            var gsUrl =
                "//gettingstarted.2sxc.org/router.aspx?" +
                $"DnnVersion={Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)}" +
                $"&2SexyContentVersion={Settings.ModuleVersion}" +
                $"&ModuleName={Module.DesktopModule.ModuleName}" +
                $"&ModuleId={Module.ModuleID}" +
                $"&PortalID={_portal.PortalId}" +
                $"&ZoneID={app.ZoneId}" +
                $"&DefaultLanguage={_portal.DefaultLanguage}" +
                $"&CurrentLanguage={_portal.CultureCode}";

            // Add AppStaticName and Version
            if (Module.DesktopModule.ModuleName != "2sxc")
            {
                gsUrl += "&AppGuid=" + app.AppGuid;
                if (app.Configuration != null)
                    gsUrl += $"&AppVersion={app.Configuration.Version}&AppOriginalId={app.Configuration.OriginalId}";
            }

            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gsUrl += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
            return gsUrl;
        }
    }
}
