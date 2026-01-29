using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Cms.Users;

/// <summary>
/// BETA Model to return role information as provided by the <see cref="UserRoles"/> DataSource.
/// </summary>
/// <remarks>
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn RoleInfo](https://docs.dnncommunity.org/api/DotNetNuke.Security.Roles.RoleInfo.html)
/// * [Oqtane UserRole](https://docs.oqtane.org/api/Oqtane.Models.UserRole.html)
/// 
/// History
/// 
/// * Introduced in v19.01
/// </remarks>
[ModelSpecs(Use = typeof(UserRoleModelOfEntity))]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IUserRoleModel : IDataWrapper
{
    /// <summary>
    /// The Role ID in the database.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The Role Name as it is displayed everywhere.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// When the user role was first created.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// When the user role was last modified.
    /// </summary>
    DateTime Modified { get; }
}