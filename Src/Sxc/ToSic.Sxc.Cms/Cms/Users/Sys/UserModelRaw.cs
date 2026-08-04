using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw.Sys;

namespace ToSic.Sxc.Cms.Users.Sys;

/// <summary>
/// Internal class to hold all the information about the user,
/// until it's converted to an IEntity in the <see cref="Users"/> DataSource.
///
/// * TODO:
/// </summary>
[PrivateApi("this is only internal - public access is always through interface")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeUse(Type = typeof(IUserModel))]
public record UserModelRaw : IHasIdentityNameId, IUserModel, IRawEntityConvertible
{
    #region Types and Names for Raw Entities

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


#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public string? NameId { get; init; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

    public bool IsSystemAdmin { get; init; }
    public bool IsSiteAdmin { get; init; }
    public bool IsContentAdmin { get; init; }
    public bool IsContentEditor { get; init; }
    public bool IsSiteDeveloper => IsSystemAdmin;

    public bool IsAnonymous { get; init; } = true;  // Default is true, everything else is default false.

    ///// <summary>
    ///// Ignore, just included for IUser compatibility
    ///// </summary>
    //string IUser.IdentityToken => null;

    public string? Username { get; init; }
    public string? Email { get; init; } // aka PreferredEmail

    [ContentTypeField(IsTitle = true)]
    public string? Name { get; init; } // aka DisplayName

    public IEnumerable<IUserRoleModel> Roles { get; init; } = [];


    /// <summary>
    /// Use this converter when about to convert to IEntity
    /// </summary>
    /// <returns></returns>
    IRawEntityConverter IRawEntityConvertible.GetConverter() => Converter;

    /// <summary>
    /// Prepare a reusable, factory-based converter for User Models to IRawEntity
    /// </summary>
    private static IRawEntityConverter Converter { get; } =
        new RawEntityConverterFactory<UserModelRaw>((source, options) =>
        {
            // New optimized way to get a dictionary with all properties, will reliably get all public properties
            var data = source
                .ObjectToDictionary()
                .FilterOutKeys(RawEntityConstants.KeysToRemove
                    .Concat([nameof(IUserModel.Roles)])
                );

            if (options.ShouldAddKey(nameof(IUserModel.Roles)))
                data.Add(
                    nameof(IUserModel.Roles),
                    new RawRelationship
                    {
                        Keys = source.Roles
                            .Select(object (r) => $"{RoleRelationshipPrefix}{r.Id}")
                            .ToList()
                    }
                );
            
            return new RawEntity
            {
                Id = source.Id,
                Guid = source.Guid,
                Values = data,
            };
        });


}