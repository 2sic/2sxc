using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class AppFilesDataSourceProviderUnknown: AppFilesDataSourceProvider
    {
        public AppFilesDataSourceProviderUnknown(Dependencies dependencies, WarnUseOfUnknown<AppFilesDataSourceProviderUnknown> _): base(dependencies, $"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        { }


        public new List<AppFileInfo> GetAppFilesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = default,
            int appId = default,
            bool onlyFolders = default,
            bool onlyFiles = default,
            string root = default,
            string filter = default
        ) => new List<AppFileInfo>();


    }
}
