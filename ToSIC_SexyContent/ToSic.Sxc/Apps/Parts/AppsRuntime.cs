using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Apps
{
    public class AppsRuntime: ZonePartRuntimeBase
    {
        internal AppsRuntime(CmsZones cmsRuntime, ILog parentLog) : base(cmsRuntime, parentLog, "Cms.AppsRt")
        {
        }

        public IEnumerable<AppUiInfo> GetSelectableApps(ITenant tenant)
        {
            Log.Add("get selectable apps");
            return
                GetApps(tenant, null)
                    .Where(a => a.Name != Eav.Constants.ContentAppName)
                    .Where(a => !a.Hidden)
                    .Select(a => new AppUiInfo
                    {
                        Name = a.Name,
                        AppId = a.AppId,
                        SupportsAjaxReload = a.Configuration?.EnableAjax ?? false,
                        Thumbnail = a.Thumbnail,
                        Version = a.Configuration?.Version?.ToString() ?? ""
                    });
        }

        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <returns></returns>
        public List<IApp> GetApps(ITenant tenant, Func<Eav.Apps.App, IAppDataConfiguration> buildConfig/*, bool includeDefaultApp*/)
        {
            var appIds = new ZoneRuntime(ZoneRuntime.ZoneId, Log).Apps;
            return appIds
                .Select(a => new App(tenant, ZoneRuntime.ZoneId, a.Key, buildConfig, true, Log) as IApp)
                .OrderBy(a => a.Name)
                .ToList();
        }

    }
}
