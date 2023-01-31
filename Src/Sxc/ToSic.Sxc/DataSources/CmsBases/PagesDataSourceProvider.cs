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

        public abstract List<CmsPageInfo> GetPagesInternal();
    }
}
