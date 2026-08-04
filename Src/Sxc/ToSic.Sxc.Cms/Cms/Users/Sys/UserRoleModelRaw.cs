using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Sxc.DataSources;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Cms.Users.Sys;

/// <summary>
/// Internal class to hold all the information about the role.
/// until it's converted to an IEntity in the <see cref="UserRoles"/> DataSource.
///
/// TODO:
/// </summary>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeUse(Type = typeof(IUserRoleModel))]
public record UserRoleModelRaw: IRawEntity, IRole, IUserRoleModel
{
    #region IRawEntity

    IDictionary<string, object?> IRawEntity.Values
        => new Dictionary<string, object?>
        {
            { nameof(Name), Name },
        };

    Guid IRawEntity.Guid => Guid.Empty;

    #endregion

    public int Id { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;
    public DateTime Modified { get; init; } = DateTime.Now;

    public string Name { get; init; } = "unknown";

}