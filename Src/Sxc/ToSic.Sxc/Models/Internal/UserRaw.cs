using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Models.Internal;

/// <summary>
/// Internal class to hold all the information about the user,
/// until it's converted to an IEntity in the <see cref="Users"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * TODO:
/// * TODO:
/// Important: this is an internal object.
/// We're just including in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// They must also match the ICmsUser interface
/// </remarks>
[PrivateApi("this is only internal - public access is always through interface")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UserRaw: RawEntityBase, IUser, IRawEntity, IHasIdentityNameId, IUserModelSync
{
    #region Types and Names for Raw Entities

    internal const string TypeName = "User";
    internal static DataFactoryOptions Options = new()
    {
        TypeName = TypeName,
        TitleField = nameof(Name)
    };

    #endregion


    public string NameId { get; init; }

    /// <summary>
    /// Role ID List.
    /// Important: Internally we use a list to do checks etc.
    /// But for creating the entity we return a CSV
    /// </summary>
    [PrivateApi]
    public List<int> Roles { get; init; }
    public bool IsSystemAdmin { get; init; }
    public bool IsSiteAdmin { get; init; }
    public bool IsContentAdmin { get; init; }
    public bool IsContentEditor { get; init; }
    public bool IsSiteDeveloper => IsSystemAdmin;

    public bool IsAnonymous { get; init; }

    /// <summary>
    /// Ignore, just included for IUser compatibility
    /// </summary>
    string IUser.IdentityToken => null;

    public string Username { get; init; }
    public string Email { get; init; } // aka PreferredEmail
    public string Name { get; init; } // aka DisplayName

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public override IDictionary<string, object> Attributes(RawConvertOptions options)
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

        if (options.ShouldAddKey(nameof(Roles)))
            data.Add("Roles",
                new RawRelationship(keys: Roles?.Select(r => $"{RoleRelationshipPrefix}{r}" as object).ToList() ?? [])
            );

        return data;
    }

    internal const string RoleRelationshipPrefix = "Role:";


}