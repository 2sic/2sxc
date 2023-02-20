using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Raw;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the UsersDataSourceProvider.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class UsersDataSourceProvider: ServiceBase
    {
        protected UsersDataSourceProvider(string logName) : base(logName)
        { }

        /// <summary>
        /// The inner list retrieving the users.
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        public abstract IEnumerable<CmsUserRaw> GetUsersInternal();
    }
}
