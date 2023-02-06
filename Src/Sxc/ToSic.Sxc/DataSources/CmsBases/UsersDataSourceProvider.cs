using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the Users DataSource.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class UsersDataSourceProvider: ServiceBase
    {
        public const int NoParent = 0;

        protected UsersDataSourceProvider(string logName) : base(logName)
        {

        }

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        public abstract IEnumerable<CmsUserInfo> GetUsersInternal();
    }
}
