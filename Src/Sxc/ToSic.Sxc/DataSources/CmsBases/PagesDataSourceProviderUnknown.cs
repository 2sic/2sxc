using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the Pages DataSource.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public class PagesDataSourceProviderUnknown: PagesDataSourceProvider
    {
        public PagesDataSourceProviderUnknown(WarnUseOfUnknown<ModuleAndBlockBuilderUnknown> _): base($"{Constants.SxcLogName}.PgBase")
        {
        }

        public override List<CmsPageInfo> GetPagesInternal() => new List<CmsPageInfo>();
    }
}
