using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Data;

namespace ToSic.Sxc.Cms.Users.Internal;

/// <summary>
/// Internal class to hold all the information about the user,
/// until it's converted to an IEntity in the <see cref="Users"/> DataSource.
///
/// * TODO:
/// </summary>
[PrivateApi("this is only internal - public access is always through interface")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeSpecs(
    Guid = "612f9341-ff91-443d-be58-500e55bec2d8",
    Description = "User Information",
    Name = TypeName
)]
public record UserModel : IRawEntity, IHasIdentityNameId, IUserModel
{
    #region Types and Names for Raw Entities

    internal const string TypeName = "User";
    internal static DataFactoryOptions Options = new()
    {
        TypeName = TypeName,
        TitleField = nameof(Name)
    };

    IDictionary<string, object> IRawEntity.Attributes(RawConvertOptions options)
    {
        var data = new Dictionary<string, object>
        {
            { nameof(Name), Name },
            { nameof(NameId), NameId },
            { nameof(IsSystemAdmin), IsSystemAdmin },
            { nameof(IsSiteAdmin), IsSiteAdmin },
            { nameof(IsContentAdmin), IsContentAdmin },
            { nameof(IsAnonymous), IsAnonymous },
            { nameof(Username), Username },
            { nameof(Email), Email },
        };

        if (options.ShouldAddKey(nameof(IUserModel.Roles)))
            data.Add(
                nameof(IUserModel.Roles),
                new RawRelationship(keys: Roles?.Select(object (r) => $"{RoleRelationshipPrefix}{r.Id}").ToList() ?? [])
            );

        return data;
    }

    internal const string RoleRelationshipPrefix = "Role:";

    ///// <summary>
    ///// Role ID List.
    ///// Important: Internally we use a list to do checks etc.
    ///// But for creating the entity we need the raw ID list.
    ///// </summary>
    //internal List<int> RolesRaw { get; init; }

    #endregion

    public int Id { get; init; }
    public Guid Guid { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;
    public DateTime Modified { get; init; } = DateTime.Now;


    public string NameId { get; init; }

    public bool IsSystemAdmin { get; init; }
    public bool IsSiteAdmin { get; init; }
    public bool IsContentAdmin { get; init; }
    public bool IsContentEditor { get; init; }
    public bool IsSiteDeveloper => IsSystemAdmin;

    public bool IsAnonymous { get; init; }

    ///// <summary>
    ///// Ignore, just included for IUser compatibility
    ///// </summary>
    //string IUser.IdentityToken => null;

    public string Username { get; init; }
    public string Email { get; init; } // aka PreferredEmail
    public string Name { get; init; } // aka DisplayName

    public IEnumerable<IUserRoleModel> Roles { get; init; }

}