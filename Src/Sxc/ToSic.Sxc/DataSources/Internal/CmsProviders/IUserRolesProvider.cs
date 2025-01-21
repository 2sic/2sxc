using ToSic.Sxc.Cms.Users.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the RolesDataSourceProvider.
///
/// Must be overriden in each platform.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IUserRolesProvider
{
    /// <summary>
    /// The inner list retrieving roles. 
    /// </summary>
    /// <returns></returns>
    IEnumerable<UserRoleModel> GetRoles();
}