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

// Important Info to people working with this
// It's an abstract class, and must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific implementation
// This is because any constructor DI should be able to target this type, and get the real implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Will get all (or just some) roles of the current site.
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
    public abstract class Roles : ExternalData
    {
        #region Public Consts for inheriting implementations

        [PrivateApi] public const string VqNiceName = "Roles (User Roles)";
        [PrivateApi] public const string VqIcon = Icons.UserCircled;
        [PrivateApi] public const string VqUiHint = "Roles in this site";
        [PrivateApi] public const string VqGlobalName = "eee54266-d7ad-4f5e-9422-2d00c8f93b45"; // random & unique Guid
        [PrivateApi] public const DataSourceType VqType = DataSourceType.Source;
        [PrivateApi] public const string VqExpectsDataOfType = "1b9fd9d1-dde0-40ad-bb66-5cd7f30de18d";
        [PrivateApi] public const string VqHelpLink = "https://r.2sxc.org/ds-roles";

        #endregion

        #region Other Constants

        private const char Separator = ',';

        #endregion

        #region Configuration-properties
        [PrivateApi] internal const string RoleIdsKey = "RoleIds";
        [PrivateApi] internal const string ExcludeRoleIdsKey = "ExcludeRoleIds";

        /// <summary>
        /// Optional (single value or comma-separated integers) filter,
        /// include roles based on roleId
        /// </summary>
        public virtual string RoleIds
        {
            get => Configuration[RoleIdsKey];
            set => Configuration[RoleIdsKey] = value;
        }

        /// <summary>
        /// Optional (single value or comma-separated integers) filter,
        /// exclude roles based on roleId
        /// </summary>
        public virtual string ExcludeRoleIds
        {
            get => Configuration[ExcludeRoleIdsKey];
            set => Configuration[ExcludeRoleIdsKey] = value;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        [PrivateApi]
        protected Roles()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList

            ConfigMask(RoleIdsKey);
            ConfigMask(ExcludeRoleIdsKey);
        }

        #endregion

        [PrivateApi]
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
            var includeRolesFilter = RolesCsvListToInt(RoleIds);
            return includeRolesFilter.Any() 
                ? (Func<RoleDataSourceInfo, bool>) (r => includeRolesFilter.Contains(r.Id)) 
                : null;
        }

        private Func<RoleDataSourceInfo, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<RoleDataSourceInfo, bool>)(r => !excludeRolesFilter.Contains(r.Id))
                : null;
        }

        [PrivateApi]
        internal static List<int> RolesCsvListToInt(string stringList)
        {
            if (!stringList.HasValue()) return new List<int>();
            return stringList.Split(Separator)
                .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : -1)
                .Where(r => r != -1)
                .ToList();
        }


        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        protected abstract IEnumerable<RoleDataSourceInfo> GetRolesInternal();

        [PrivateApi]
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
