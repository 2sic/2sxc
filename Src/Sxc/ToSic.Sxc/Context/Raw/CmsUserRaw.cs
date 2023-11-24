using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Context.Raw
{
    /// <summary>
    /// Internal class to hold all the information about the user,
    /// until it's converted to an IEntity in the <see cref="Users"/> DataSource.
    ///
    /// For detailed documentation, check the docs of the underlying objects:
    ///
    /// * TODO:
    /// * TODO:
    /// Important: this is an internal object.
    /// We're just including in in the docs to better understand where the properties come from.
    /// We'll probably move it to another namespace some day.
    /// </summary>
    /// <remarks>
    /// Make sure the property names never change, as they are critical for the created Entity.
    /// They must also match the ICmsUser interface
    /// </remarks>
    [PrivateApi("this is only internal - public access is always through interface")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CmsUserRaw: RawEntityBase, IUser, IRawEntity, IHasIdentityNameId
    {
        #region Types and Names for Raw Entities

        internal static string TypeName = "User";
        internal static DataFactoryOptions Options = new(typeName: TypeName, titleField: nameof(Name));

        #endregion

        #region Constant user objects for Unknown/Anonymous

        public static readonly CmsUserRaw AnonymousUser = new() { Id = -1, Name = Constants.Anonymous };

        public static readonly CmsUserRaw UnknownUser = new() { Id = -2, Name = Eav.Constants.NullNameId };


        #endregion

        public string NameId { get; set; }
        /// <summary>
        /// Role ID List.
        /// Important: Internally we use a list to do checks etc.
        /// But for creating the entity we return a CSV
        /// </summary>
        [PrivateApi]
        public List<int> Roles { get; set; }
        public bool IsSystemAdmin { get; set; }
        public bool IsSiteAdmin { get; set; }
        public bool IsContentAdmin { get; set; }
        public bool IsSiteDeveloper => IsSystemAdmin;


        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Ignore, just included for IUser compatibility
        /// </summary>
        string IUser.IdentityToken => null;

        public string Username { get; set; }
        public string Email { get; set; } // aka PreferredEmail
        public string Name { get; set; } // aka DisplayName

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
                data.Add("Roles", new RawRelationship(keys: Roles?.Select(r => $"{RoleRelationshipPrefix}{r}" as object)
                                                            ?? new List<object>())
                );
            return data;
        }

        internal const string RoleRelationshipPrefix = "Role:";
    }
}