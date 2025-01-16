using ToSic.Sxc.Data.Model;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// User Model for data returned by the <see cref="Users"/> DataSource or other sources.
/// </summary>
/// <remarks>
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn UserInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Users.UserInfo.html)
/// * [Oqtane User](https://docs.oqtane.org/api/Oqtane.Models.User.html)
///
/// History
/// 
/// * Introduced in v19.01
/// </remarks>
[DataModelConversion(Map = [
    typeof(DataModelFrom<IEntity, IUserModel, UserModelOfEntity>),
])]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IUserModel : IDataModel
{
    /// <inheritdoc cref="IUserModelSync.Email" />
    string Email { get; }

    /// <inheritdoc cref="IUserModelSync.Id" />
    int Id { get; }

    /// <inheritdoc cref="IUserModelSync.Guid" />
    Guid Guid { get; }

    /// <inheritdoc cref="IUserModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="IUserModelSync.Modified" />
    DateTime Modified { get; }

    /// <inheritdoc cref="IUserModelSync.IsAnonymous" />
    bool IsAnonymous { get; }

    /// <inheritdoc cref="IUserModelSync.IsSiteAdmin" />
    bool IsSiteAdmin { get; }

    /// <inheritdoc cref="IUserModelSync.IsContentAdmin" />
    bool IsContentAdmin { get; }

    /// <inheritdoc cref="IUserModelSync.IsContentEditor" />
    bool IsContentEditor { get; }

    /// <inheritdoc cref="IUserModelSync.NameId" />
    string NameId { get; }

    /// <inheritdoc cref="IUserModelSync.IsSystemAdmin" />
    bool IsSystemAdmin { get; }

    /// <inheritdoc cref="IUserModelSync.IsSiteDeveloper" />
    bool IsSiteDeveloper { get; }

    /// <inheritdoc cref="IUserModelSync.Name" />
    string Name { get; }

    /// <inheritdoc cref="IUserModelSync.Username" />
    string Username { get; }

    /// <summary>
    /// Roles of the user.
    /// </summary>
    IEnumerable<IUserRoleModel> Roles { get; }
}