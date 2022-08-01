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

        public const string VqNiceName = "Users";
        public const string VqIcon = "date_range";
        public const string VqUiHint = "Users in the CMS";
        public const string VqGlobalName = "93ac53c6-adc6-4218-b979-48d1071a5765"; // random & unique Guid
        public const DataSourceType VqType = DataSourceType.Source;
        public const string VqExpectsDataOfType = "";
        public const string VqHelpLink = ""; // TODO

        #endregion

        #region Configuration-properties
        private const string RolesFilterKey = "RolesFilter";
        private const string UsersFilterKey = "UsersFilter";

        /// <summary>
        /// Optional RolesFilter (comma-separated integers) filter,
        /// exclude users that have any of roles from filter
        /// </summary>
        public string RolesFilter
        {
            get => Configuration[RolesFilterKey];
            set => Configuration[RolesFilterKey] = value;
        }

        /// <summary>
        /// Optional exclude Users (comma-separated guids) filter,
        /// exclude users based on guid
        /// </summary>
        public string UsersFilter
        {
            get => Configuration[UsersFilterKey];
            set => Configuration[UsersFilterKey] = value;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        protected UsersDataSourceBase()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList

            ConfigMask(RolesFilterKey, "[Settings:RolesFilter]");

            ConfigMask(UsersFilterKey, "[Settings:UsersFilter]");

            // TEST cases
            //Configuration[UsersFilterKey] = "d65e097e-afde-4a46-a8ab-9d3ed277b4a9,989358ab-86ad-44a7-8b35-412e076e469a";
            //Configuration[UsersFilterKey] = "d65e097e-afde-4a46-a8ab-9d3ed277b4a9";
            //Configuration[UsersFilterKey] = "not-a-guid";
            //Configuration[RolesFilterKey] = "1096,1097,1101,1102,1103";
            //Configuration[RolesFilterKey] = "1102,1103";
            //Configuration[RolesFilterKey] = "1101";
            //Configuration[RolesFilterKey] = "not-a-integer,-1";
        }

        #endregion

        public IImmutableList<IEntity> GetList()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var users = GetUsersInternal()?.ToList();

            if (users == null || !users.Any()) return wrapLog.Return(new ImmutableArray<IEntity>(), "null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();

            if (!string.IsNullOrEmpty(RolesFilter))
            {
                var rolesFilter = RolesFilter.Split(',')
                    .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : -1)
                    .Where(r => r != -1).ToList();
                if (rolesFilter.Any())
                    users = users.Where(u => !u.Roles.Any(r => rolesFilter.Contains(r))).ToList();
            }

            if (!string.IsNullOrEmpty(UsersFilter))
            {
                var usersFilter = UsersFilter.Split(',')
                    .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                    .Where(u => u != Guid.Empty).ToList();
                if (usersFilter.Any())
                    users = users.Where(u => u.Guid.HasValue && !usersFilter.Contains(u.Guid.Value)).ToList();
            }

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
