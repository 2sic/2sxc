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
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.DataSources.CmsBases
{
    [PrivateApi("this is half a DataSource - the final implementation must come from each platform")]
    // additional info so the visual query can provide the correct buttons and infos
    public abstract class RolesDataSourceBase : ExternalData
    {
        #region Public Consts for inheriting implementations

        public const string VqNiceName = "Roles - BETA - DO NOT USE YET";
        public const string VqIcon = Icons.DateRange;
        public const string VqUiHint = "Roles in the CMS";
        public const string VqGlobalName = "eee54266-d7ad-4f5e-9422-2d00c8f93b45"; // random & unique Guid
        public const DataSourceType VqType = DataSourceType.Source;
        public const string VqExpectsDataOfType = "";
        public const string VqHelpLink = ""; // TODO

        #endregion

        #region Other Constants

        public const char RoleSeparator = ',';
        public const char RoleSeparatorAdvanced = '§';

        #endregion

        #region Configuration-properties
        private const string RestrictRoleIdsKey = "FilterRoleIds";
        private const string ExcludeRoleIdsKey = "ExcludeRoleIds";

        /// <summary>
        /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
        /// include roles based on roleId
        /// </summary>
        public string FilterRoleIds
        {
            get => Configuration[RestrictRoleIdsKey];
            set => Configuration[RestrictRoleIdsKey] = value;
        }

        /// <summary>
        /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
        /// exclude roles based on roleId
        /// </summary>
        public string ExcludeRoleIds
        {
            get => Configuration[ExcludeRoleIdsKey];
            set => Configuration[ExcludeRoleIdsKey] = value;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        protected RolesDataSourceBase()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList

            ConfigMask(RestrictRoleIdsKey, $"[Settings:{RestrictRoleIdsKey}]");
            ConfigMask(ExcludeRoleIdsKey, $"[Settings:{ExcludeRoleIdsKey}]");

            // TEST cases
            //Configuration[ExcludeRolesFilterKey] = "1096,1097,1101,1102,1103";
            //Configuration[ExcludeRolesFilterKey] = "1102,1103";
            //Configuration[ExcludeRolesFilterKey] = "1101";
            //Configuration[ExcludeRolesFilterKey] = "not-a-integer,-1";
        }

        #endregion

        public IImmutableList<IEntity> GetList()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var roles = GetRolesInternal()?.ToList();
            
            if (roles == null || !roles.Any()) return wrapLog.Return(new ImmutableArray<IEntity>(), "null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();

            var includeRolesPredicate = IncludeRolesPredicate();
            if (includeRolesPredicate != null) roles = roles.Where(includeRolesPredicate).ToList();

            var excludeRolesPredicate = ExcludeRolesPredicate();
            if (excludeRolesPredicate != null) roles = roles.Where(excludeRolesPredicate).ToList();

            var result = roles
                .Select(r =>
                    DataBuilder.Entity(new Dictionary<string, object>
                        {
                            {"Name", r.Name},
                        },
                        id: r.Id,
                        created: r.Created,
                        modified: r.Modified,
                        titleField: "Name"))
                .ToImmutableList();

            return wrapLog.Return(result, "found");
        }

        private Func<RoleDataSourceInfo, bool> IncludeRolesPredicate()
        {
            var includeRolesFilter = CsvListToInt(FilterRoleIds);
            return includeRolesFilter.Any() 
                ? (Func<RoleDataSourceInfo, bool>) (r => includeRolesFilter.Contains(r.Id)) 
                : null;
        }

        private Func<RoleDataSourceInfo, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = CsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<RoleDataSourceInfo, bool>)(r => !excludeRolesFilter.Contains(r.Id))
                : null;
        }
        private static List<int> CsvListToInt(string stringList)
        {
            if (!stringList.HasValue()) return new List<int>();
            var separator = stringList.Contains(RoleSeparatorAdvanced) ? RoleSeparatorAdvanced : RoleSeparator;
            return stringList.Split(separator)
                .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : -1)
                .Where(r => r != -1)
                .ToList();
        }


        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<RoleDataSourceInfo> GetRolesInternal();
        
        protected class RoleDataSourceInfo : IRole
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
        }

        #endregion
    }
}
