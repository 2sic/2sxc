using System.Collections.Generic;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.DataSources.Internal;

internal class SitesDataSourceProviderUnknown: SitesDataSourceProvider
{
    public SitesDataSourceProviderUnknown(MyServices services, WarnUseOfUnknown<SitesDataSourceProviderUnknown> _): base(services, $"{SxcLogging.SxcLogName}.{LogConstants.NameUnknown}")
    { }

    public override List<SiteDataRaw> GetSitesInternal() => new();
}