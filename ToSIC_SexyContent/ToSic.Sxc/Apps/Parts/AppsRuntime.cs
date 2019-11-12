using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using ToSic.Sxc.SxcTemp;

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
                GetApps(tenant,false)
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
        public List<IApp> GetApps(ITenant tenant, bool includeDefaultApp)
        {
            var appIds = new ZoneRuntime(ZoneRuntime.ZoneId, Log).Apps;
            var builtApps = appIds.Select(eavApp => GetApp.LightWithoutData(tenant, ZoneRuntime.ZoneId, eavApp.Key, parentLog: Log));

            if (!includeDefaultApp)
                builtApps = builtApps.Where(a => a.Name != Eav.Constants.ContentAppName);

            return builtApps.OrderBy(a => a.Name).ToList();
        }

    }
}
