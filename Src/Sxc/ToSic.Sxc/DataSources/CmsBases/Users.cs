using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Process;
using ToSic.Eav.Data.Source;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Will get all (or just some) users of the current site.
    /// The resulting Entity will match the <see cref="IUser"/> interface.
    /// </summary>
    [PublicApi]
    [VisualQuery(
        NiceName = "Users",
        Icon = Icons.UserCircled,
        UiHint = "Users in this site",
        HelpLink = "https://r.2sxc.org/ds-users",
        GlobalName = "93ac53c6-adc6-4218-b979-48d1071a5765", // random & unique Guid
        Type = DataSourceType.Source,
        ExpectsDataOfType = "ac11fae7-1916-4d2d-8583-09872e1e6966"
    )]
    public partial class Users : ExternalData
    {
        private readonly ITreeMapper _treeMapper;
        private readonly LazySvc<DataSourceFactory> _dsFactory;
        private readonly IDataFactory _usersFactory;
        private readonly UsersDataSourceProvider _provider;

        #region Other Constants

        private const char Separator = ',';

        #endregion

        #region Configuration-properties

        /// <summary>
        /// Optional Users (single value or comma-separated guids or integers) filter,
        /// include users based on guid or id
        /// </summary>
        [Configuration]
        public string UserIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional exclude Users (single value or comma-separated guids or integers) filter,
        /// exclude users based on guid or id
        /// </summary>
        [Configuration]
        public string ExcludeUserIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
        /// include users that have any of roles from filter
        /// </summary>
        [Configuration]
        public string RoleIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
        /// exclude users that have any of roles from filter
        /// </summary>
        [Configuration]
        public string ExcludeRoleIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Optional SystemAdmins filter.
        /// 
        /// * `true` - with System Admins
        /// * `false` - without System Admins
        /// * `required` - only System Admins (no normal users)
        /// </summary>
        /// <remarks>
        /// * Changed to be string in v15.03 (before bool) to allow more options such as `required`
        /// </remarks>
        [Configuration]
        public string IncludeSystemAdmins
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        private static readonly string IncludeRequired = "required";
        private static readonly string IncludeOptional = true.ToString();
        private static readonly string IncludeForbidden = false.ToString();

        /// <summary>
        /// Add property `Roles` as a relationship to role entities.
        /// </summary>
        /// <remarks>
        /// * Added v15.03 - minimal breaking change, before the source return a non-standard `RoleIds` string-array.
        /// </remarks>
        [Configuration(Fallback = true)]
        public bool AddRoles
        {
            get => Configuration.GetThis(true);
            set => Configuration.SetThis(value);
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        [PrivateApi]
        public Users(MyServices services,
            UsersDataSourceProvider provider,
            IDataFactory dataFactory,
            LazySvc<DataSourceFactory> dsFactory,
            ITreeMapper treeMapper) : base(services, "SDS.Users")
        {
            ConnectServices(
                _provider = provider,
                _usersFactory = dataFactory,
                _dsFactory = dsFactory,
                _treeMapper = treeMapper
            );
            Provide(() => GetUsersAndRoles().Users); // default out, if accessed, will deliver GetList
            Provide(() => GetUsersAndRoles().UserRoles, "Roles");
        }

        #endregion

        [PrivateApi]
        private (IImmutableList<IEntity> Users, IImmutableList<IEntity> UserRoles) GetUsersAndRoles() => Log.Func(l =>
        {
            if (_usersAndRolesCache != default) 
                return (_usersAndRolesCache, "from cache");

            // Always parse configuration first
            Configuration.Parse();

            // Get raw users from provider, then generate entities
            var usersRaw = GetUsersAndFilter();

            // Figure out options to be sure we have the roles/roleids
            var relationships = new LazyLookup<object, IEntity>();
            _usersFactory.Configure(typeName: CmsUserRaw.TypeName, titleField: CmsUserRaw.TitleFieldName,
                relationships: relationships,
                rawConvertOptions: new RawConvertOptions(addKeys: new []{ "Roles"}));

            var users = _usersFactory.Create(usersRaw);
            var roles = EmptyList;

            // If we should include the roles, create them now and attach
            if (AddRoles)
                try
                {
                    // New...
                    roles = GetRolesStream(usersRaw);
                    relationships.Add(roles.Select(r =>
                        new KeyValuePair<object, IEntity>($"{CmsUserRaw.RoleRelationshipPrefix}{r.EntityId}", r)));
                }
                catch (Exception ex)
                {
                    l.A("Error trying to add roles");
                    l.Ex(ex);
                    /* ignore for now */
                }

            _usersAndRolesCache = (users, roles);
            return (_usersAndRolesCache, $"users {users.Count}; roles {roles.Count}");
        });

        private (IImmutableList<IEntity> Users, IImmutableList<IEntity> UserRoles) _usersAndRolesCache;


        private List<CmsUserRaw> GetUsersAndFilter() => Log.Func(l =>
        {
            var users = _provider.GetUsersInternal()?.ToList();
            if (users == null || !users.Any()) return (new List<CmsUserRaw>(), "null/empty");

            foreach (var filter in GetAllFilters())
                users = users.Where(filter).ToList();

            return (users, $"found {users.Count}");
        });


        /// <summary>
        /// Retrieve roles and create lookup for relationship-mapper
        /// </summary>
        /// <param name="usersRaw"></param>
        /// <returns></returns>
        private ImmutableList<IEntity> GetRolesStream(List<CmsUserRaw> usersRaw)
        {
            // Get list of all role IDs which are to be used
            var roleIds = usersRaw.SelectMany(u => u.Roles).Distinct().ToList();
            // Get roles, use the current data source to provide aspects such as lookups etc.
            var rolesDs = _dsFactory.Value.GetDataSource<Roles>(this);
            // Set filter parameter to only get roles we'll need
            rolesDs.RoleIds = string.Join(",", roleIds);
            var roles = rolesDs.List;
            return roles.ToImmutableList();
        }
    }
}
