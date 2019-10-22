using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources.Caches;
using ToSic.SexyContent;

namespace ToSic.Sxc.Internal
{
    internal class AppHelpers
    {
        internal static int GetAppIdFromGuidName(int zoneId, string appName, bool alsoCheckFolderName = false)
        {
            var baseCache = (BaseCache) DataSource.GetCache(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId);
            // ReSharper disable once UnusedVariable
            var dummy = baseCache.CacheTimestamp;

            if (String.IsNullOrEmpty(appName))
                return 0; 

            var appId = baseCache.ZoneApps[zoneId].Apps
                    .Where(p => p.Value == appName).Select(p => p.Key).FirstOrDefault();

            // optionally check folder names
            if (appId == 0 && alsoCheckFolderName)
            {
                var nameLower = appName.ToLower();
                foreach (var p in baseCache.ZoneApps[zoneId].Apps)
                {
                    var mds = DataSource.GetMetaDataSource(zoneId, p.Key);
                    var appMetaData = mds
                        .GetMetadata(SystemRuntime.MetadataType(Eav.Constants.AppAssignmentName), p.Key,
                            AppConstants.TypeAppConfig)
                        .FirstOrDefault();
                    var folder = appMetaData?.GetBestValue("Folder").ToString();
                    if (!String.IsNullOrEmpty(folder) && folder.ToLower() == nameLower)
                        return p.Key;
                }
            }
            return appId > 0 ? appId : Settings.DataIsMissingInDb;
        }
        
    }
}