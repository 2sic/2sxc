using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    internal class SitesDataSourceProviderUnknown: SitesDataSourceProvider
    {
        public SitesDataSourceProviderUnknown(MyServices services, WarnUseOfUnknown<SitesDataSourceProviderUnknown> _): base(services, $"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        { }

        public override List<SiteDataRaw> GetSitesInternal(
            ) => new List<SiteDataRaw>();
    }
}
