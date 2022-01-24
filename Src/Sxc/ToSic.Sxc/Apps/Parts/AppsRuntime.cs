using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class AppsRuntime: ZonePartRuntimeBase<CmsZones, AppsRuntime>
    {
        #region Constructor / DI

        public AppsRuntime(IServiceProvider serviceProvider, IAppStates appStates) : base(serviceProvider, "Cms.AppsRt")
        {
            _appStates = appStates;
        }

        private readonly IAppStates _appStates;

        #endregion

        public IList<AppUiInfo> GetSelectableApps(ISite site, string filter)
        {
            var callLog = Log.Call<IList<AppUiInfo>>(filter);
            var list =
                GetApps(site, null)
                    .Where(a => a.Name != Eav.Constants.ContentAppName && a.Name != Eav.Constants.PrimaryAppName && a.Name != Eav.Constants.PrimaryAppGuid) // #SiteApp v13
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

        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <returns></returns>
        public List<IApp> GetInheritableApps(ISite site, Func<Eav.Apps.App, IAppDataConfiguration> buildConfig)
        {
            // Get existing apps, as we should not list inheritable apps which are already inherited
            var siteApps = _appStates.Apps(site.ZoneId)
                .Select(a => _appStates.Get(a.Key).Folder)
                .ToList();

            var zones = _appStates.Zones;
            var appStateWithCacheInfo = (AppStates)_appStates;
            var result = zones
                // Skip all global apps on the current site, as they shouldn't be inheritable
                .Where(z => z.Key != site.ZoneId)
                .SelectMany(zSet =>
                {
                    // todo: unclear if this is the right way to do this - probably the ZoneId should come from the site?
                    var zId = zSet.Key;
                    var appIds = _appStates.Apps(zId);

                    return appIds
                        .Select(a => new AppIdentity(zId, a.Key))
                        .Where(aId =>
                        {
                            if (!appStateWithCacheInfo.IsCached(aId)) return false;
                            var appState = _appStates.Get(aId);
                            return appState != null && appState.IsGlobal() && !siteApps.Any(sa => sa.Equals(appState.Folder, StringComparison.InvariantCultureIgnoreCase));
                        })
                        .Select(a => ServiceProvider.Build<App>()
                            .PreInit(site)
                            .Init(a, buildConfig, Log) as IApp)
                        .OrderBy(a => a.Name)
                        .ToList();
                })
                .ToList();
            return result;
        }

    }
}
