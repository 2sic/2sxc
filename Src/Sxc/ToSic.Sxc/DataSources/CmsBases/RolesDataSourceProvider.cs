using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the RolesDataSourceProvider.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class RolesDataSourceProvider: ServiceBase
    {
        protected RolesDataSourceProvider(string logName) : base(logName)
        { }

        /// <summary>
        /// The inner list retrieving roles. 
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        public abstract IEnumerable<RoleDataRaw> GetRolesInternal();
    }
}
