using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class SitesDataSourceProviderUnknown: SitesDataSourceProvider
    {
        public SitesDataSourceProviderUnknown(Dependencies services, WarnUseOfUnknown<SitesDataSourceProviderUnknown> _): base(services, $"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        { }

        public override List<CmsSiteInfo> GetSitesInternal(
            ) => new List<CmsSiteInfo>();
    }
}
