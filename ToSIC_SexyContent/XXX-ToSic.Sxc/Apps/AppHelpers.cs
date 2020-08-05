//using System.Linq;
//using ToSic.Eav.Apps;

//namespace ToSic.Sxc.Apps
//{
//    internal class AppHelpers
//    {
//        internal static int GetAppIdFromGuidName(int zoneId, string appName, bool alsoCheckFolderName = false)
//        {
//            var zones = State.Zones;

//            if (string.IsNullOrEmpty(appName))
//                return 0; 

//            var appId = zones[zoneId].Apps
//                    .Where(p => p.Value == appName)
//                    .Select(p => p.Key).FirstOrDefault();

//            // optionally check folder names
//            if (appId == 0 && alsoCheckFolderName)
//                appId = AppIdFromFolderName(zoneId, appName);
//            return appId > 0 ? appId : AppConstants.AppIdNotFound;
//        }

//        private static int AppIdFromFolderName(int zoneId, string folderName)
//        {
//            var nameLower = folderName.ToLower();

//            foreach (var p in State.Zones[zoneId].Apps)
//            {
//                var mds = State.Get(new AppIdentity(zoneId, p.Key));
//                var appMetaData = mds
//                    .Get(SystemRuntime.MetadataType(Eav.Constants.AppAssignmentName), p.Key,
//                        AppConstants.TypeAppConfig)
//                    .FirstOrDefault();
//                var folder = appMetaData?.GetBestValue(AppConstants.FieldFolder).ToString();
//                if (!string.IsNullOrEmpty(folder) && folder.ToLower() == nameLower)
//                    return p.Key;
//            }

//            // not found
//            return 0;
//        }
//    }
//}