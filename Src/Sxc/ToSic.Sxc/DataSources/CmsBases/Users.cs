using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Logging;
using ToSic.Lib.Documentation;

// Important Info to people working with this
// It's an abstract class, and must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific implementation
// This is because any constructor DI should be able to target this type, and get the real implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Will get all (or just some) users of the current site.
    /// </summary>
    [PublicApi]
    [VisualQuery(
        NiceName = VqNiceName,
        Icon = VqIcon,
        UiHint = VqUiHint,
        HelpLink = VqHelpLink,
        GlobalName = VqGlobalName,
        Type = VqType,
        ExpectsDataOfType = VqExpectsDataOfType,
        Difficulty = DifficultyBeta.Default
    )]
    public abstract class Users : ExternalData
    {
        #region Public Consts for inheriting implementations

        [PrivateApi] public const string VqNiceName = "Users";
        [PrivateApi] public const string VqIcon = Icons.UserCircled;
        [PrivateApi] public const string VqUiHint = "Users in this site";
        [PrivateApi] public const string VqGlobalName = "93ac53c6-adc6-4218-b979-48d1071a5765"; // random & unique Guid
        [PrivateApi] public const DataSourceType VqType = DataSourceType.Source;
        [PrivateApi] public const string VqExpectsDataOfType = "ac11fae7-1916-4d2d-8583-09872e1e6966";
        [PrivateApi] public const string VqHelpLink = "https://r.2sxc.org/ds-users";

        #endregion

        #region Other Constants

        private const char Separator = ',';

        #endregion

        #region Configuration-properties

        private const string UserIdsKey = "UserIds";
        private const string ExcludeUserIdsKey = "ExcludeUserIds";
        private const string IncludeSystemAdminsKey = "IncludeSystemAdmins";

        /// <summary>
        /// Optional Users (single value or comma-separated guids or integers) filter,
        /// include users based on guid or id
        /// </summary>
        public string UserIds
        {
            get => Configuration[UserIdsKey];
            set => Configuration[UserIdsKey] = value;
        }

        /// <summary>
        /// Optional exclude Users (single value or comma-separated guids or integers) filter,
        /// exclude users based on guid or id
        /// </summary>
        public string ExcludeUserIds
        {
            get => Configuration[ExcludeUserIdsKey];
            set => Configuration[ExcludeUserIdsKey] = value;
        }

        /// <summary>
        /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
        /// include users that have any of roles from filter
        /// </summary>
        public string RoleIds
        {
            get => Configuration[Roles.RoleIdsKey];
            set => Configuration[Roles.RoleIdsKey] = value;
        }

        /// <summary>
        /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
        /// exclude users that have any of roles from filter
        /// </summary>
        public string ExcludeRoleIds
        {
            get => Configuration[Roles.ExcludeRoleIdsKey];
            set => Configuration[Roles.ExcludeRoleIdsKey] = value;
        }

        /// <summary>
        /// Optional SuperUserFilter (boolean) filter,
        /// true - only SuperUsers
        /// false - without SuperUsers
        /// </summary>
        public string IncludeSystemAdmins
        {
            get => Configuration[IncludeSystemAdminsKey];
            set => Configuration[IncludeSystemAdminsKey] = value;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        [PrivateApi]
        protected Users()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList
            
            // UserRoles not final...
            // Provide("UserRoles", GetRoles);

            // TODO: @STV - pls always also use constants in these filters, don't repeat the string (already done, just FYI)
            ConfigMask(UserIdsKey);
            ConfigMask(ExcludeUserIdsKey);
            ConfigMask(Roles.RoleIdsKey);
            ConfigMask(Roles.ExcludeRoleIdsKey);
            ConfigMask($"{IncludeSystemAdminsKey}||false");
        }

        #endregion

        private IImmutableList<IEntity> GetList()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var users = GetUsersAndFilter();

            var result = users
                .Select(u => DataBuilder.Entity(new Dictionary<string, object>
                    {
                        { nameof(u.NameId), u.NameId },
                        { nameof(u.RoleIds), u.RoleIds == null ? null : string.Join(Separator.ToString(), u.RoleIds) },
                        { nameof(u.IsSystemAdmin), u.IsSystemAdmin },
                        { nameof(u.IsSiteAdmin), u.IsSiteAdmin },
                        { nameof(u.IsContentAdmin), u.IsContentAdmin },
                        //{"IsDesigner", u.IsDesigner},
                        { nameof(u.IsAnonymous), u.IsAnonymous },
                        { nameof(u.Username), u.Username },
                        { nameof(u.Email), u.Email },
                        { nameof(u.Name), u.Name },
                    },
                    id: u.Id,
                    guid: u.Guid,
                    created: u.Created,
                    modified: u.Modified,
                    titleField: "Name"))
                .ToImmutableList();

            return wrapLog.Return(result, "found");
        }

        // WIP - not final yet
        // We should only implement this when we're sure about how it can be used
        // Maybe a sub-entity-property would be better...
        private IImmutableList<IEntity> GetRoles()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var users = GetUsersAndFilter();

            var result = users
                .Where(u => u.RoleIds?.Any() == true)
                .SelectMany(u => u.RoleIds.Select(r =>
                    DataBuilder.Entity(new Dictionary<string, object>
                        {
                            { "RoleId", r },
                            { "UserId", u.Id },
                            { Attributes.TitleNiceName, $"User {u.Id} in Role {r}" },
                        },
                        titleField: Attributes.TitleNiceName))
                )
                .ToImmutableList();

            return wrapLog.Return(result, "found");
        }

        private List<UserDataSourceInfo> GetUsersAndFilter()
        {
            var wrapLog = Log.Fn<List<UserDataSourceInfo>>();

            if (_usersFiltered != null) return wrapLog.Return(_usersFiltered, "re-use");

            var users = GetUsersInternal()?.ToList();

            if (users == null || !users.Any()) return wrapLog.ReturnNull("null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();
            
            foreach (var filter in Filters())
                users = users.Where(filter).ToList();

            return wrapLog.Return(_usersFiltered = users, $"found {users.Count}");
        }

        private List<UserDataSourceInfo> _usersFiltered;

        private List<Func<UserDataSourceInfo, bool>> Filters()
        {
            var l = Log.Fn<List<Func<UserDataSourceInfo, bool>>>();
            var filters = new List<Func<UserDataSourceInfo, bool>>
            {
                IncludeUsersPredicate(),
                ExcludeUsersPredicate(),
                IncludeRolesPredicate(),
                ExcludeRolesPredicate(),
                SuperUserPredicate()
            };
            filters = filters.Where(f => f != null).ToList();
            return l.Return(filters, $"{filters.Count}");
        }

        private Func<UserDataSourceInfo, bool> IncludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(UserIds)) return null;
            var includeUserGuids = IncludeUserGuids();
            var includeUserIds = IncludeUserIds();
            if (includeUserGuids == null && includeUserIds == null) return null;
            return u => (includeUserGuids != null && includeUserGuids(u)) || (includeUserIds != null && includeUserIds(u));
        }

        private Func<UserDataSourceInfo, bool> IncludeUserGuids()
        {
            var userGuidFilter = UserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return userGuidFilter.Any()
                ? (Func<UserDataSourceInfo, bool>)(u => u.Guid.HasValue && userGuidFilter.Contains(u.Guid.Value))
                : null;
        }

        private Func<UserDataSourceInfo, bool> IncludeUserIds()
        {
            var userIdFilter = UserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return userIdFilter.Any() 
                ? (Func<UserDataSourceInfo, bool>) (u => userIdFilter.Contains(u.Id)) 
                : null;
        }

        private Func<UserDataSourceInfo, bool> ExcludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeUserIds)) return null;
            var excludeUserGuids = ExcludeUsersByGuids();
            var excludeUserIds = ExcludeUsersById();
            if (excludeUserGuids == null && excludeUserIds == null) return null;
            return u => (excludeUserGuids == null || excludeUserGuids(u)) && (excludeUserIds == null || excludeUserIds(u));
        }

        private Func<UserDataSourceInfo, bool> ExcludeUsersByGuids()
        {
            var excludeUserGuidsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return excludeUserGuidsFilter.Any()
                ? (Func<UserDataSourceInfo, bool>)(u => u.Guid.HasValue && !excludeUserGuidsFilter.Contains(u.Guid.Value))
                : null;
        }

        private Func<UserDataSourceInfo, bool> ExcludeUsersById()
        {
            var excludeUserIdsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return excludeUserIdsFilter.Any()
                ? (Func<UserDataSourceInfo, bool>)(u => !excludeUserIdsFilter.Contains(u.Id))
                : null;
        }

        private Func<UserDataSourceInfo, bool> IncludeRolesPredicate()
        {
            var rolesFilter = Roles.RolesCsvListToInt(RoleIds);
            return rolesFilter.Any()
                ? (Func<UserDataSourceInfo, bool>) (u => u.RoleIds.Any(r => rolesFilter.Contains(r)))
                : null;
        }

        private Func<UserDataSourceInfo, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = Roles.RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<UserDataSourceInfo, bool>) (u => !u.RoleIds.Any(r => excludeRolesFilter.Contains(r)))
                : null;
        }

        private Func<UserDataSourceInfo, bool> SuperUserPredicate() =>
            bool.TryParse(IncludeSystemAdmins, out var superUserFilter)
                ? (Func<UserDataSourceInfo, bool>)(u => u.IsSystemAdmin == superUserFilter)
                : null;

        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        protected abstract IEnumerable<UserDataSourceInfo> GetUsersInternal();

        protected class UserDataSourceInfo: IHasIdentityNameId // : IUser - not inheriting for the moment, to not include deprecated properties IsAdmin, IsSuperUser, IsDesigner...
        {
            public int Id { get; set; }
            public Guid? Guid { get; set; }
            public string NameId { get; set; }
            public List<int> RoleIds { get; set; }
            public bool IsSystemAdmin { get; set; }

            public bool IsSiteAdmin { get; set; }
            public bool IsContentAdmin { get; set; }

            //public bool IsDesigner { get; set; }
            public bool IsAnonymous { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }

            public string Username { get; set; }
            public string Email { get; set; } // aka PreferredEmail
            public string Name { get; set; } // aka DisplayName

            #endregion
        }
    }
}
