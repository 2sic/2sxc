using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.DI;
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
    public partial class Users : ExternalData
    {
        private readonly ITreeMapper _treeMapper;
        private readonly LazySvc<DataSourceFactory> _dsFactory;
        private readonly IDataBuilder _usersBuilder;
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
        /// Optional SuperUserFilter (boolean) filter,
        /// true - only SuperUsers
        /// false - without SuperUsers
        /// </summary>
        [Configuration(Fallback = false)]
        public bool IncludeSystemAdmins
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Add property `Roles` as a relationship to role entities.
        /// </summary>
        [Configuration(Fallback = true)] // TEMP/ WIP
        [PrivateApi]
        public bool AddRoles
        {
            get => Configuration.GetThis(true);
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// Add property `RoleIds` as a comma separated list of IDs.
        /// </summary>
        [Configuration(Fallback = false)]
        [PrivateApi]
        public bool AddRoleIds
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
        public Users(MyServices services,
            UsersDataSourceProvider provider,
            IDataBuilder dataBuilder,
            LazySvc<DataSourceFactory> dsFactory,
            ITreeMapper treeMapper) : base(services, "SDS.Users")
        {
            ConnectServices(
                _provider = provider,
                _usersBuilder = dataBuilder,
                _dsFactory = dsFactory,
                _treeMapper = treeMapper
            );
            Provide(GetList); // default out, if accessed, will deliver GetList
        }

        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetList() => Log.Func(l =>
        {
            // Always parse configuration first
            Configuration.Parse();

            // Get raw users from provider, then generate entities
            var usersRaw = GetUsersAndFilter();

            // Figure out options to be sure we have the roles/roleids
            var keysToAdd = new List<string>();
            if (AddRoleIds) keysToAdd.Add(nameof(CmsUserInfo.RoleIds));
            _usersBuilder.Configure(typeName: "User", titleField: nameof(CmsUserInfo.Name), createRawOptions: new CreateRawOptions(addKeys: keysToAdd));

            var users = _usersBuilder.CreateMany(usersRaw);

            // If we should include the roles, create them now and attach
            if (AddRoles)
            {
                try
                {
                    // Mix generated users with the RoleIds which only exist on the raw list
                    var userNeeds = users.ToList()
                        .Select(u =>
                            (u, usersRaw.FirstOrDefault(usr => usr.Id == u.EntityId)?.RoleIds ?? new List<int>()))
                        .ToList();
                    var rolesLookup = GetRolesForLookup(usersRaw);

                    var mapped = _treeMapper.AddSomeRelationshipsWIP("Roles", userNeeds, rolesLookup);
                    users = mapped.ToImmutableList();
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    /* ignore for now */
                }
            }

            return (users, $"found {users.Count}");
        });


        private List<CmsUserInfo> GetUsersAndFilter() => Log.Func(l =>
        {
            var users = _provider.GetUsersInternal()?.ToList();
            if (users == null || !users.Any()) return (new List<CmsUserInfo>(), "null/empty");

            foreach (var filter in GetAllFilters())
                users = users.Where(filter).ToList();

            return (users, $"found {users.Count}");
        });


        private List<(IEntity r, int EntityId)> GetRolesForLookup(List<CmsUserInfo> usersRaw)
        {
            // Get list of all role IDs which are to be used
            var roleIds = usersRaw.SelectMany(u => u.RoleIds).Distinct().ToList();
            // Get roles, use the current data source to provide aspects such as lookups etc.
            var rolesDs = _dsFactory.Value.GetDataSource<Roles>(this);
            // Set filter parameter to only get roles we'll need
            rolesDs.RoleIds = string.Join(",", roleIds);
            // Retrieve roles and create lookup for relationship-mapper
            var rolesLookup = rolesDs.List.Select(r => (r, r.EntityId)).ToList();
            return rolesLookup;
        }



    }
}
