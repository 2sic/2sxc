using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.DataSources.CmsBases
{
    [PrivateApi("this is half a DataSource - the final implementation must come from each platform")]
    // additional info so the visual query can provide the correct buttons and infos
    public abstract class UsersDataSourceBase : ExternalData
    {
        #region Public Consts for inheriting implementations

        public const string VqNiceName = "Users BETA - DO NOT USE YET";
        public const string VqIcon = "date_range";
        public const string VqUiHint = "Users in the CMS";
        public const string VqGlobalName = "93ac53c6-adc6-4218-b979-48d1071a5765"; // random & unique Guid
        public const DataSourceType VqType = DataSourceType.Source;
        public const string VqExpectsDataOfType = "";
        public const string VqHelpLink = ""; // TODO

        #endregion

        #region Configuration-properties

        private const string IncludeUsersFilterKey = "IncludeUsersFilter";
        private const string ExcludeUsersFilterKey = "ExcludeUsersFilter";
        private const string IncludeRolesFilterKey = "IncludeRolesFilter";
        private const string ExcludeRolesFilterKey = "ExcludeRolesFilter";

        /// <summary>
        /// Optional Users (comma-separated guids) filter,
        /// include users based on guid or id
        /// </summary>
        public string IncludeUsersFilter
        {
            get => Configuration[IncludeUsersFilterKey];
            set => Configuration[IncludeUsersFilterKey] = value;
        }

        /// <summary>
        /// Optional exclude Users (comma-separated guids) filter,
        /// exclude users based on guid or id
        /// </summary>
        public string ExcludeUsersFilter
        {
            get => Configuration[ExcludeUsersFilterKey];
            set => Configuration[ExcludeUsersFilterKey] = value;
        }
        
        /// <summary>
        /// Optional IncludeRolesFilter (comma-separated integers) filter,
        /// include users that have any of roles from filter
        /// </summary>
        public string IncludeRolesFilter
        {
            get => Configuration[IncludeRolesFilterKey];
            set => Configuration[IncludeRolesFilterKey] = value;
        }

        /// <summary>
        /// Optional ExcludeRolesFilter (comma-separated integers) filter,
        /// exclude users that have any of roles from filter
        /// </summary>
        public string ExcludeRolesFilter
        {
            get => Configuration[ExcludeRolesFilterKey];
            set => Configuration[ExcludeRolesFilterKey] = value;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        protected UsersDataSourceBase()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList

            ConfigMask(IncludeUsersFilterKey, "[Settings:IncludeUsersFilter]");
            ConfigMask(ExcludeUsersFilterKey, "[Settings:ExcludeUsersFilter]");
            ConfigMask(IncludeRolesFilterKey, "[Settings:IncludeRolesFilter]");
            ConfigMask(ExcludeRolesFilterKey, "[Settings:ExcludeRolesFilter]");

            // TEST cases
            //Configuration[ExcludeUsersFilterKey] = "d65e097e-afde-4a46-a8ab-9d3ed277b4a9,989358ab-86ad-44a7-8b35-412e076e469a";
            //Configuration[ExcludeUsersFilterKey] = "d65e097e-afde-4a46-a8ab-9d3ed277b4a9";
            //Configuration[ExcludeUsersFilterKey] = "not-a-guid";
            //Configuration[ExcludeRolesFilterKey] = "1096,1097,1101,1102,1103";
            //Configuration[ExcludeRolesFilterKey] = "1102,1103";
            //Configuration[ExcludeRolesFilterKey] = "1101";
            //Configuration[ExcludeRolesFilterKey] = "not-a-integer,-1";
        }

        #endregion

        public IImmutableList<IEntity> GetList()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var users = GetUsersInternal()?.ToList();

            if (users == null || !users.Any()) return wrapLog.Return(new ImmutableArray<IEntity>(), "null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();

            var includeUsersPredicate = IncludeUsersPredicate();
            if (includeUsersPredicate != null) users = users.Where(includeUsersPredicate).ToList();

            var excludeUsersPredicate = ExcludeUsersPredicate();
            if (excludeUsersPredicate != null) users = users.Where(excludeUsersPredicate).ToList();
            
            var includeRolesPredicate = IncludeRolesPredicate();
            if (includeRolesPredicate != null) users = users.Where(includeRolesPredicate).ToList();
            
            var excludeRolesPredicate = ExcludeRolesPredicate();
            if (excludeRolesPredicate != null) users = users.Where(excludeRolesPredicate).ToList();

            var result = users
                .Select(u =>
                    DataBuilder.Entity(new Dictionary<string, object>
                        {
                            {"IdentityToken", u.IdentityToken},
                            {"Roles", u.Roles},
                            {"IsSuperUser", u.IsSuperUser},
                            {"IsAdmin", u.IsAdmin},
                            {"IsDesigner", u.IsDesigner},
                            {"IsAnonymous", u.IsAnonymous},
                            {"Username", u.Username},
                            {"Email", u.Email},
                            {"Name", u.Name},
                        },
                        id: u.Id,
                        guid: u.Guid,
                        created: u.Created,
                        modified: u.Modified,
                        titleField: "Name"))
                .ToImmutableList();

            return wrapLog.Return(result, "found");
        }

        private Func<UserDataSourceInfo, bool> IncludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(IncludeUsersFilter)) return null;
            var includeUserGuids = IncludeUserGuids();
            var includeUserIds = IncludeUserIds();
            return u => (includeUserGuids != null && includeUserGuids(u)) || (includeUserIds != null && includeUserIds(u));
        }

        private Func<UserDataSourceInfo, bool> IncludeUserGuids()
        {
            var userGuidFilter = IncludeUsersFilter.Split(',')
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return userGuidFilter.Any()
                ? (Func<UserDataSourceInfo, bool>)(u => u.Guid.HasValue && userGuidFilter.Contains(u.Guid.Value))
                : null;
        }

        private Func<UserDataSourceInfo, bool> IncludeUserIds()
        {
            var userIdFilter = IncludeUsersFilter.Split(',')
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return userIdFilter.Any() 
                ? (Func<UserDataSourceInfo, bool>) (u => userIdFilter.Contains(u.Id)) 
                : null;
        }

        private Func<UserDataSourceInfo, bool> ExcludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeUsersFilter)) return null;
            var excludeUserGuids = ExcludeUserGuids();
            var excludeUserIds = ExcludeUserIds();
            return u => (excludeUserGuids != null && excludeUserGuids(u)) || (excludeUserIds != null && excludeUserIds(u));
        }

        private Func<UserDataSourceInfo, bool> ExcludeUserGuids()
        {
            var excludeUserGuidsFilter = ExcludeUsersFilter.Split(',')
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return excludeUserGuidsFilter.Any()
                ? (Func<UserDataSourceInfo, bool>)(u => u.Guid.HasValue && !excludeUserGuidsFilter.Contains(u.Guid.Value))
                : null;
        }

        private Func<UserDataSourceInfo, bool> ExcludeUserIds()
        {
            var excludeUserIdsFilter = ExcludeUsersFilter.Split(',')
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return excludeUserIdsFilter.Any()
                ? (Func<UserDataSourceInfo, bool>)(u => excludeUserIdsFilter.Contains(u.Id))
                : null;
        }

        private Func<UserDataSourceInfo, bool> IncludeRolesPredicate()
        {
            if (string.IsNullOrEmpty(IncludeRolesFilter)) return null;
            var rolesFilter = IncludeRolesFilter.Split(',')
                .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : -1)
                .Where(r => r != -1).ToList();
            return rolesFilter.Any()
                ? (Func<UserDataSourceInfo, bool>) (u => u.Roles.Any(r => rolesFilter.Contains(r)))
                : null;
        }

        private Func<UserDataSourceInfo, bool> ExcludeRolesPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeRolesFilter)) return null;
            var excludeRolesFilter = ExcludeRolesFilter.Split(',')
                .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : -1)
                .Where(r => r != -1).ToList();
            return excludeRolesFilter.Any()
                ? (Func<UserDataSourceInfo, bool>) (u => !u.Roles.Any(r => excludeRolesFilter.Contains(r)))
                : null;
        }

        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<UserDataSourceInfo> GetUsersInternal();

        protected class UserDataSourceInfo : IUser
        {
            public int Id { get; set; }
            public Guid? Guid { get; set; }
            public string IdentityToken { get; set; }
            public List<int> Roles { get; set; }
            public bool IsSuperUser { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsDesigner { get; set; }
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
