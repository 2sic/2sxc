using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;
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
        public PagesDataSourceProviderUnknown(WarnUseOfUnknown<ModuleAndBlockBuilderUnknown> _): base($"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        {
        }

        public override List<CmsPageInfo> GetPagesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            bool includeHidden = default,
            bool includeDeleted = default,
            bool includeAdmin = default,
            bool includeSystem = default,
            bool includeLinks = default,
            bool requireViewPermissions = true,
            bool requireEditPermissions = true
        ) => new List<CmsPageInfo>();
    }
}
