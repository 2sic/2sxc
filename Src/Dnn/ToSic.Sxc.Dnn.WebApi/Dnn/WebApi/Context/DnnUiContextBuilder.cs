using System;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.WebApi.Context;
using Assembly = System.Reflection.Assembly;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Dnn.WebApi.Context
{
    public sealed class DnnUiContextBuilder : UiContextBuilderBase
    {
        #region Constructor / DI

        private readonly Lazy<IZoneMapper> _zoneMapper;
        private readonly IContextResolver _ctxResolver;
        private readonly PortalSettings _portal = PortalSettings.Current;

        private ModuleInfo Module => (_ctxResolver.BlockOrNull()?.Module as DnnModule)?.UnwrappedContents;

        public DnnUiContextBuilder(Lazy<IZoneMapper> zoneMapper, IContextResolver ctxResolver, Dependencies deps): base(deps)
        {
            _zoneMapper = zoneMapper;
            _ctxResolver = ctxResolver;
        }

        #endregion

        public override IUiContextBuilder SetZoneAndApp(int zoneId, IAppIdentity app)
        {
            // check if we're providing context for missing app
            // in this case we must find the zone based on the portals.
            if (zoneId == 0 && app == null) zoneId = _zoneMapper.Value.Init(null).GetZoneId(_portal.PortalId);
            return base.SetZoneAndApp(zoneId, app);
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

        protected override AppDto GetApp(Ctx flags)
        {
            var appDto = base.GetApp(flags);
            string apiRoot = null;
            try
            {
                var roots = DnnJsApiHeader.GetApiRoots();
                apiRoot = roots.Item2;
            } catch { /* ignore */ }
            appDto.Api = apiRoot;
            return appDto;
        }

        /// <summary>
        /// build a getting-started url which is used to correctly show the user infos like
        /// warnings related to his dnn or 2sxc version
        /// infos based on his languages
        /// redirects based on the app he's looking at, etc.
        /// </summary>
        /// <returns></returns>
        protected override string GetGettingStartedUrl()
        {
            if (!(App is IApp app)) return "";

            var gsUrl =
                BaseGettingStartedUrl("Dnn",
                    Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4),
                    Module.DesktopModule.ModuleName,
                    Module.ModuleID,
                    _portal.DefaultLanguage,
                    _portal.CultureCode);
            //"//gettingstarted.2sxc.org/router.aspx?" +
                //"Platform=Dnn" +
                //$"SysVersion={Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)}" +
                //$"&SxcVersion={Settings.ModuleVersion}" +
                //$"&ModuleName={Module.DesktopModule.ModuleName}" +
                //$"&ModuleId={Module.ModuleID}" +
                //$"&SiteId={_portal.PortalId}" +
                //$"&ZoneID={app.ZoneId}" +
                //$"&DefaultLanguage={_portal.DefaultLanguage}" +
                //$"&CurrentLanguage={_portal.CultureCode}";

            // Add AppStaticName and Version if _not_ the primary content-app
            if (Module.DesktopModule.ModuleName != "2sxc")
            {
                gsUrl += "&AppGuid=" + app.AppGuid;
                if (app.Configuration != null)
                    gsUrl += $"&AppVersion={app.Configuration.Version}&AppOriginalId={app.Configuration.OriginalId}";
            }

            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gsUrl += hostSettings.ContainsKey("GUID") ? "&SysGUID=" + hostSettings["GUID"] : "";
            return gsUrl;
        }
    }
}
