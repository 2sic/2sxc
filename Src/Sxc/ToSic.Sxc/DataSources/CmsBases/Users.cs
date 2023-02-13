using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// Important Info to people working with this
// It depends on abstract provder, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Will get all (or just some) users of the current site.
    /// </summary>
    [PublicApi]
    [VisualQuery(
        NiceName = "Users",
        Icon = Icons.UserCircled,
        UiHint = "Users in this site",
        HelpLink = "https://r.2sxc.org/ds-users",
        GlobalName = "93ac53c6-adc6-4218-b979-48d1071a5765", // random & unique Guid
        Type = DataSourceType.Source,
        ExpectsDataOfType = "ac11fae7-1916-4d2d-8583-09872e1e6966",
        Difficulty = DifficultyBeta.Default
    )]
    public class Users : ExternalData
    {
        private readonly IDataBuilderPro _usersDataBuilder;
        private readonly UsersDataSourceProvider _provider;

        #region Other Constants

        private const char Separator = ',';

        #endregion

        #region Configuration-properties

        /// <summary>
        /// Optional Users (single value or comma-separated guids or integers) filter,
        /// include users based on guid or id
        /// </summary>
        public string UserIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional exclude Users (single value or comma-separated guids or integers) filter,
        /// exclude users based on guid or id
        /// </summary>
        public string ExcludeUserIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
        /// include users that have any of roles from filter
        /// </summary>
        public string RoleIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
        /// exclude users that have any of roles from filter
        /// </summary>
        public string ExcludeRoleIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional SuperUserFilter (boolean) filter,
        /// true - only SuperUsers
        /// false - without SuperUsers
        /// </summary>
        public bool IncludeSystemAdmins
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        [PrivateApi]
        public Users(Dependencies dependencies, UsersDataSourceProvider provider, IDataBuilderPro usersDataBuilder) : base(dependencies, "SDS.Users")
        {
            ConnectServices(
                _provider = provider,
                _usersDataBuilder = usersDataBuilder.Configure(typeName: "User", titleField: nameof(CmsUserInfo.Name))
            );
            Provide(GetList); // default out, if accessed, will deliver GetList
            
            // UserRoles not final...
            // Provide("UserRoles", GetRoles);

            ConfigMask(nameof(UserIds));
            ConfigMask(nameof(ExcludeUserIds));
            ConfigMask(nameof(RoleIds));
            ConfigMask(nameof(ExcludeRoleIds));
            ConfigMask($"{nameof(IncludeSystemAdmins)}||false");
        }

        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetList() => Log.Func(l =>
        {
            var users = GetUsersAndFilter();
            var result = _usersDataBuilder.CreateMany(users);

            return (result, "found");
        });

        //// WIP - not final yet
        //// We should only implement this when we're sure about how it can be used
        //// Maybe a sub-entity-property would be better...
        //private IImmutableList<IEntity> GetRoles()
        //{
        //    var wrapLog = Log.Fn<IImmutableList<IEntity>>();
        //    var users = GetUsersAndFilter();

        //    var result = users
        //        .Where(u => u.RoleIds?.Any() == true)
        //        .SelectMany(u => u.RoleIds.Select(r =>
        //            DataBuilder.Entity(new Dictionary<string, object>
        //                {
        //                    { "RoleId", r },
        //                    { "UserId", u.Id },
        //                    { Attributes.TitleNiceName, $"User {u.Id} in Role {r}" },
        //                },
        //                titleField: Attributes.TitleNiceName))
        //        )
        //        .ToImmutableList();

        //    return wrapLog.Return(result, "found");
        //}

        private List<CmsUserInfo> GetUsersAndFilter() => Log.Func(l =>
        {
            if (_usersFiltered != null) return (_usersFiltered, "re-use");
            
            var users = _provider.GetUsersInternal()?.ToList();

            if (users == null || !users.Any()) return (new List<CmsUserInfo>(), "null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();

            foreach (var filter in Filters())
                users = users.Where(filter).ToList();

            return (_usersFiltered = users, $"found {users.Count}");
        });
        private List<CmsUserInfo> _usersFiltered;

        private List<Func<CmsUserInfo, bool>> Filters() => Log.Func(l =>
        {
            var filters = new List<Func<CmsUserInfo, bool>>
            {
                IncludeUsersPredicate(),
                ExcludeUsersPredicate(),
                IncludeRolesPredicate(),
                ExcludeRolesPredicate(),
                SuperUserPredicate()
            };
            filters = filters.Where(f => f != null).ToList();
            return (filters, $"{filters.Count}");
        });

        private Func<CmsUserInfo, bool> IncludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(UserIds)) return null;
            var includeUserGuids = IncludeUserGuids();
            var includeUserIds = IncludeUserIds();
            if (includeUserGuids == null && includeUserIds == null) return null;
            return u => (includeUserGuids != null && includeUserGuids(u)) || (includeUserIds != null && includeUserIds(u));
        }

        private Func<CmsUserInfo, bool> IncludeUserGuids()
        {
            var userGuidFilter = UserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return userGuidFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => u.Guid != Guid.Empty && userGuidFilter.Contains(u.Guid))
                : null;
        }

        private Func<CmsUserInfo, bool> IncludeUserIds()
        {
            var userIdFilter = UserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return userIdFilter.Any() 
                ? (Func<CmsUserInfo, bool>) (u => userIdFilter.Contains(u.Id)) 
                : null;
        }

        private Func<CmsUserInfo, bool> ExcludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeUserIds)) return null;
            var excludeUserGuids = ExcludeUsersByGuids();
            var excludeUserIds = ExcludeUsersById();
            if (excludeUserGuids == null && excludeUserIds == null) return null;
            return u => (excludeUserGuids == null || excludeUserGuids(u)) && (excludeUserIds == null || excludeUserIds(u));
        }

        private Func<CmsUserInfo, bool> ExcludeUsersByGuids()
        {
            var excludeUserGuidsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return excludeUserGuidsFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => u.Guid != Guid.Empty && !excludeUserGuidsFilter.Contains(u.Guid))
                : null;
        }

        private Func<CmsUserInfo, bool> ExcludeUsersById()
        {
            var excludeUserIdsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return excludeUserIdsFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => !excludeUserIdsFilter.Contains(u.Id))
                : null;
        }

        private Func<CmsUserInfo, bool> IncludeRolesPredicate()
        {
            var rolesFilter = Roles.RolesCsvListToInt(RoleIds);
            return rolesFilter.Any()
                ? (Func<CmsUserInfo, bool>) (u => u.RoleIds.Any(r => rolesFilter.Contains(r)))
                : null;
        }

        private Func<CmsUserInfo, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = Roles.RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<CmsUserInfo, bool>) (u => !u.RoleIds.Any(r => excludeRolesFilter.Contains(r)))
                : null;
        }

        private Func<CmsUserInfo, bool> SuperUserPredicate() =>
            /*bool.TryParse(*/IncludeSystemAdmins/*, out var superUserFilter)*/
                ? (Func<CmsUserInfo, bool>)(u => u.IsSystemAdmin /*== superUserFilter*/)
                : null;

    }
}
