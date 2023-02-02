using System.Collections.Generic;
using ToSic.Lib.Services;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the Pages DataSource.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class PagesDataSourceProvider: ServiceBase
    {
        public const int NoParent = 0;

        protected PagesDataSourceProvider(string logName) : base(logName)
        {

        }

        /// <summary>
        /// FYI: The filters are not actually implemented yet.
        /// So the core data source doesn't have settings to configure this
        /// </summary>
        /// <returns></returns>
        public abstract List<CmsPageInfo> GetPagesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            bool includeHidden = default,
            bool includeDeleted = default,
            bool includeAdmin = default,
            bool includeSystem = default,
            bool includeLinks = default,
            bool requireViewPermissions = true,
            bool requireEditPermissions = true
        );
    }
}
