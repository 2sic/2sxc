using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class AppFilesDataSourceProviderUnknown: AppFilesDataSourceProvider
    {
        public AppFilesDataSourceProviderUnknown(MyServices services, WarnUseOfUnknown<AppFilesDataSourceProviderUnknown> _): base(services, $"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        { }


        public new List<AppFileDataNew> GetAppFilesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = default,
            int appId = default,
            bool onlyFolders = default,
            bool onlyFiles = default,
            string root = default,
            string filter = default
        ) => new List<AppFileDataNew>();


    }
}
