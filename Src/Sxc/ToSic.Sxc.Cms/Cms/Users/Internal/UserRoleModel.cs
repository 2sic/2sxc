using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Cms.Users.Internal;

/// <summary>
/// Internal class to hold all the information about the role.
/// until it's converted to an IEntity in the <see cref="UserRoles"/> DataSource.
///
/// TODO:
/// </summary>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeSpecs(
    Guid = "dc104414-e61a-4a59-bda8-455772ceb0cc",
    Description = "User-Role in the site",
    Name = TypeName
)]
public record UserRoleModel: IRawEntity, IRole, IUserRoleModel
{
    #region IRawEntity

    internal const string TypeName = "Role";

    internal static DataFactoryOptions Options = new()
    {
        AutoId = false,
        TypeName = TypeName,
        TitleField = nameof(Name),
    };

    IDictionary<string, object> IRawEntity.Attributes(RawConvertOptions options)
        => new Dictionary<string, object>
        {
            { nameof(Name), Name },
        };

    Guid IRawEntity.Guid => Guid.Empty;

    #endregion

    public int Id { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;
    public DateTime Modified { get; init; } = DateTime.Now;

    public string Name { get; init; }

}