using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Apps
{
    public class AppsRuntime: ZonePartRuntimeBase<CmsZones, AppsRuntime>
    {
        #region Constructor / DI

        public AppsRuntime(IServiceProvider serviceProvider, ZoneRuntime zoneRuntime, IAppStates appStates) : base(serviceProvider, "Cms.AppsRt")
        {
            _zoneRuntime = zoneRuntime;
            _appStates = appStates;
        }
        private readonly ZoneRuntime _zoneRuntime;
        private readonly IAppStates _appStates;

        #endregion

        public IList<AppUiInfo> GetSelectableApps(ISite site, string filter)
        {
            var callLog = Log.Call<IList<AppUiInfo>>(filter);
            var list =
                GetApps(site, null)
                    .Where(a => a.Name != Eav.Constants.ContentAppName && a.Name != Eav.Constants.PrimaryAppName) // #SiteApp v13
                    .Where(a => !a.Hidden)
                    .Select(a => new AppUiInfo
                    {
                        Name = a.Name,
                        AppId = a.AppId,
                        SupportsAjaxReload = a.Configuration?.EnableAjax ?? false,
                        Thumbnail = a.Thumbnail,
                        Version = a.Configuration?.Version?.ToString() ?? ""
                    })
                    .ToList();

            if (string.IsNullOrWhiteSpace(filter)) return callLog("unfiltered", list);

            // New feature in 10.27 - if app-list is provided, only return these
            var appNames = filter.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            list = list.Where(ap => appNames
                    .Any(name => string.Equals(name, ap.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
            return callLog(null, list);
        }

        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <returns></returns>
        public List<IApp> GetApps(ISite site, Func<Eav.Apps.App, IAppDataConfiguration> buildConfig)
        {
            // todo: unclear if this is the right way to do this - probably the ZoneId should come from the site?
            var zId = ZoneRuntime.ZoneId;
            var appIds = _appStates.Apps(zId);
            return appIds
                .Select(a => ServiceProvider.Build<App>()
                    .PreInit(site)
                    .Init(new AppIdentity(zId, a.Key), buildConfig, Log) as IApp)
                .OrderBy(a => a.Name)
                .ToList();
        }

    }
}
