using ToSic.Sxc.Data.Model;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// Model to return role information as provided by the <see cref="UserRoles"/> DataSource.
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
[DataModelConversion(Map = [
    typeof(DataModelFrom<IEntity, IUserRoleModel, UserRoleModel>),
])]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IUserRoleModel : IDataModel
{
    /// <inheritdoc cref="IUserRoleModelSync.Id" />
    int Id { get; }

    /// <inheritdoc cref="IUserRoleModelSync.Name" />
    string Name { get; }

    /// <inheritdoc cref="IUserRoleModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="IUserRoleModelSync.Modified" />
    DateTime Modified { get; }
}