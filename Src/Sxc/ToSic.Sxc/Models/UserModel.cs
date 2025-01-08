using ToSic.Sxc.Context.Internal.Raw;
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
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class UserModel : DataModel, IUserModel
{
    /// <summary>
    /// Special override object to provide the values without using the entity.
    ///
    /// WIP possible solution for the IUserService
    /// </summary>
    internal CmsUserRaw Override { get; init; }

    /// <inheritdoc />
    public string Email
        => Override?.Email
           ?? _entity.Get<string>(nameof(Email));

    /// <inheritdoc />
    public int Id
        => Override?.Id
           ?? _entity.EntityId;

    /// <inheritdoc />
    public Guid Guid
        => Override?.Guid
           ?? _entity.EntityGuid;

    /// <inheritdoc />
    public DateTime Created
        => Override?.Created
           ?? _entity.Created;

    /// <inheritdoc />
    public DateTime Modified
        => Override?.Modified
           ?? _entity.Modified;

    /// <inheritdoc />
    public bool IsAnonymous
        => Override?.IsAnonymous
           ?? _entity.Get<bool>(nameof(IsAnonymous));

    /// <inheritdoc />
    public bool IsSiteAdmin
        => Override?.IsSiteAdmin
           ?? _entity.Get<bool>(nameof(IsSiteAdmin));

    /// <inheritdoc />
    public bool IsContentAdmin
        => Override?.IsContentAdmin
           ?? _entity.Get<bool>(nameof(IsContentAdmin));

    /// <inheritdoc />
    public bool IsContentEditor
        => Override?.IsContentEditor
           ?? _entity.Get<bool>(nameof(IsContentEditor));

    /// <inheritdoc />
    public string NameId
        => Override?.NameId
           ?? _entity.Get<string>(nameof(NameId));

    /// <inheritdoc />
    public bool IsSystemAdmin
        => Override?.IsSystemAdmin
           ?? _entity.Get<bool>(nameof(IsSystemAdmin));

    /// <inheritdoc />
    public bool IsSiteDeveloper
        => Override?.IsSiteDeveloper
           ?? _entity.Get<bool>(nameof(IsSiteDeveloper));

    //IMetadata ICmsUser.Metadata => null;

    /// <inheritdoc />
    public string Name
        => Override?.Name
           ?? _entity.Get<string>(nameof(Name));

    /// <inheritdoc />
    public string Username
        => Override?.Username
           ?? _entity.Get<string>(nameof(Username));

    //IMetadataOf IHasMetadata.Metadata => null;

    /// <summary>
    /// Roles of the user.
    /// </summary>
    public IEnumerable<UserRoleModel> Roles
        => AsList<UserRoleModel>(_entity.Children(field: nameof(Roles)));

}