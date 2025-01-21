using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Users.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the RolesDataSourceProvider.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class RolesDataSourceProvider(string logName, object[] connect = default) : ServiceBase(logName, connect: connect)
{
    /// <summary>
    /// The inner list retrieving roles. 
    /// </summary>
    /// <returns></returns>
    [PrivateApi]
    public abstract IEnumerable<UserRoleModel> GetRolesInternal();
}