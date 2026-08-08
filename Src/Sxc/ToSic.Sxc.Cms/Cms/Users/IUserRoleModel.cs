using ToSic.Eav.Data.ContentTypes;
using ToSic.Sxc.Cms.Users.Sys;
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
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ContentType(
    Guid = "dc104414-e61a-4a59-bda8-455772ceb0cc",
    Description = "User-Role in the site",
    Name = Constants.ContentTypeName
)]
public interface IUserRoleModel : IModelFromEntity<UserRoleModel>
{
    private static class Constants
    {
        internal const string ContentTypeName = "Role";
    }
    /// <summary>
    /// The Role ID in the database.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The Role Name as it is displayed everywhere.
    /// </summary>
    [ContentTypeTitle]
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