using ToSic.Lib.Services;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the UsersDataSourceProvider.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class UsersDataSourceProvider(string logName, object[] connect = default) : ServiceBase(logName, connect: connect)
{
    /// <summary>
    /// The inner list retrieving the users.
    /// </summary>
    /// <returns></returns>
    [PrivateApi]
    public abstract IEnumerable<UserRaw> GetUsersInternal();
}