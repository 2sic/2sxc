using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using static ToSic.Eav.DataSources.DataSourceConstants;
using static ToSic.Sxc.DataSources.RoleDataRaw;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Will get all (or just some) roles of the current site.
    /// </summary>
    [PublicApi]
    [VisualQuery(
        NiceName = "Roles (User Roles)",
        Icon = Icons.UserCircled,
        UiHint = "Roles in this site",
        HelpLink = "https://r.2sxc.org/ds-roles",
        GlobalName = "eee54266-d7ad-4f5e-9422-2d00c8f93b45",
        Type = DataSourceType.Source,
        ExpectsDataOfType = "1b9fd9d1-dde0-40ad-bb66-5cd7f30de18d"
    )]
    public class Roles : ExternalData
    {
        private readonly IDataFactory _rolesFactory;
        private readonly RolesDataSourceProvider _provider;

        #region Other Constants

        private const char Separator = ',';

        #endregion

        #region Configuration-properties

        /// <summary>
        /// Optional (single value or comma-separated integers) filter,
        /// include roles based on roleId
        /// </summary>
        [Configuration]
        public virtual string RoleIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional (single value or comma-separated integers) filter,
        /// exclude roles based on roleId
        /// </summary>
        [Configuration]
        public virtual string ExcludeRoleIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        [PrivateApi]
        public Roles(MyServices services, RolesDataSourceProvider provider, IDataFactory rolesFactory) : base(services, $"SDS.Roles")
        {
            ConnectServices(
                _provider = provider,
                _rolesFactory = rolesFactory.New(typeName: TypeName, titleField: TitleFieldName, idAutoIncrementZero: false)
            );
            Provide(GetList);
        }

        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetList() => Log.Func(l =>
        {
            var roles = _provider.GetRolesInternal()?.ToList();
            l.A($"found {roles?.Count} roles");

            if (roles.SafeNone()) 
                return (EmptyList, "null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();

            var includeRolesPredicate = IncludeRolesPredicate();
            l.A($"includeRoles: {includeRolesPredicate == null}");
            if (includeRolesPredicate != null) roles = roles.Where(includeRolesPredicate).ToList();

            var excludeRolesPredicate = ExcludeRolesPredicate();
            l.A($"excludeRoles: {excludeRolesPredicate == null}");
            if (excludeRolesPredicate != null) roles = roles.Where(excludeRolesPredicate).ToList();

            var result = _rolesFactory.Create(roles);

            return (result, $"found {result.Count} roles");
        });

        private Func<RoleDataRaw, bool> IncludeRolesPredicate()
        {
            var includeRolesFilter = RolesCsvListToInt(RoleIds);
            return includeRolesFilter.Any() 
                ? (Func<RoleDataRaw, bool>) (r => includeRolesFilter.Contains(r.Id)) 
                : null;
        }

        private Func<RoleDataRaw, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<RoleDataRaw, bool>)(r => !excludeRolesFilter.Contains(r.Id))
                : null;
        }

        [PrivateApi]
        internal static List<int> RolesCsvListToInt(string stringList)
        {
            if (!stringList.HasValue()) return new List<int>();
            return stringList.Split(Separator)
                .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : int.MinValue)
                .Where(r => r != int.MinValue)
                .ToList();
        }
    }
}
