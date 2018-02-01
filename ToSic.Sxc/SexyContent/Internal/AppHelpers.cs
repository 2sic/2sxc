using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources.Caches;
using static System.String;

namespace ToSic.SexyContent.Internal
{
    public class AppHelpers
    {
        internal static int GetAppIdFromGuidName(int zoneId, string appName, bool alsoCheckFolderName = false)
        {
            // ToDo: Fix issue in EAV (cache is only ensured when a CacheItem-Property is accessed like LastRefresh)
            var baseCache = (BaseCache) DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId);
            // ReSharper disable once UnusedVariable
            var dummy = baseCache.CacheTimestamp;

            if (IsNullOrEmpty(appName))
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
                            .GetMetadata(SystemRuntime.MetadataType(Constants.AppAssignmentName), p.Key,
                                AppConstants.AttributeSetStaticNameApps)
                            .FirstOrDefault();
                        string folder = appMetaData?.GetBestValue("Folder").ToString();
                    if (!IsNullOrEmpty(folder) && folder.ToLower() == nameLower)
                        return p.Key;

                }
            }
            return appId > 0 ? appId : Settings.DataIsMissingInDb;
        }
        
    }
}