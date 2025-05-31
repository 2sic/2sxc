using ToSic.Eav.Data.Source;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Services;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Internal;
using ToSic.Sys.Users;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Will get all (or just some) users of the current site.
/// </summary>
/// <remarks>
/// You can cast the result to <see cref="IUserModel"/> for typed use in your code.
/// To figure out the returned properties, best also consult the <see cref="IUserModel"/>.
/// 
/// The resulting Entity will almost match the <see cref="IUser"/> interface.
/// 
/// History
/// 
/// * Created ca. v.16 early 2023 but not officially communicated
/// * Models <see cref="IUserModel"/> and <see cref="IUserRoleModel"/> created in v19.01 and officially released
/// </remarks>
[PublicApi]
[VisualQuery(
    NiceName = "Users",
    Icon = DataSourceIcons.UserCircled,
    UiHint = "Users in this site",
    HelpLink = "https://go.2sxc.org/ds-users",
    NameId = "93ac53c6-adc6-4218-b979-48d1071a5765", // random & unique Guid
    Type = DataSourceType.Source,
    ConfigurationType = "ac11fae7-1916-4d2d-8583-09872e1e6966"
)]
public partial class Users : CustomDataSourceAdvanced
{
    private readonly IDataSourceGenerator<UserRoles> _rolesGenerator;
    private readonly IUsersProvider _provider;

    #region Configuration-properties

    /// <summary>
    /// Optional Users (single value or comma-separated guids or integers) filter,
    /// include users based on guid or id
    /// </summary>
    [Configuration]
    public string UserIds
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    /// <summary>
    /// Optional exclude Users (single value or comma-separated guids or integers) filter,
    /// exclude users based on guid or id
    /// </summary>
    [Configuration]
    public string ExcludeUserIds
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    /// <summary>
    /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
    /// include users that have any of roles from filter
    /// </summary>
    [Configuration]
    public string RoleIds
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    /// <summary>
    /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
    /// exclude users that have any of roles from filter
    /// </summary>
    [Configuration]
    public string ExcludeRoleIds
    {
        get => field ?? Configuration.GetThis();
        set;
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
        get => field ?? Configuration.GetThis();
        set;
    }

    /// <summary>
    /// Add property `Roles` as a relationship to role entities.
    /// </summary>
    /// <remarks>
    /// * Added v15.03 - minimal breaking change, before the source return a non-standard `RoleIds` string-array.
    /// </remarks>
    [Configuration(Fallback = true)]
    public bool AddRoles
    {
        get => _addRoles ?? Configuration.GetThis(true);
        set => _addRoles = value;
    }
    private bool? _addRoles;

    #endregion

    private UsersGetSpecs Specs => new()
    {
        UserIds = UserIds,
        ExcludeUserIds = ExcludeUserIds,
        RoleIds = RoleIds,
        ExcludeRoleIds = ExcludeRoleIds,
        IncludeSystemAdmins = IncludeSystemAdmins,
        AddRoles = AddRoles
    };


    #region Constructor

    /// <summary>
    /// Constructor to tell the system what out-streams we have
    /// </summary>
    [PrivateApi]
    public Users(MyServices services, IUsersProvider provider, IDataSourceGenerator<UserRoles> rolesGenerator)
        : base(services, "SDS.Users", connect: [provider, rolesGenerator])
    {
        _provider = provider;
        _rolesGenerator = rolesGenerator;

        ProvideOut(() => UsersAndRoles.Users); // default out, if accessed, will deliver GetList
        ProvideOut(() => UsersAndRoles.UserRoles, "Roles");
    }

    #endregion

    private (IEnumerable<IEntity> Users, IEnumerable<IEntity> UserRoles) UsersAndRoles => _usersAndRoles ??= GetUsersAndRoles();
    private (IEnumerable<IEntity> Users, IEnumerable<IEntity> UserRoles)? _usersAndRoles;

    private (IEnumerable<IEntity> Users, IEnumerable<IEntity> UserRoles) GetUsersAndRoles()
    {
        var l = Log.Fn<(IEnumerable<IEntity> Users, IEnumerable<IEntity> UserRoles)>();

        // Get raw users from provider, then generate entities
        var usersRaw = GetUsersAndFilter();

        // Figure out options to be sure we have the roles/roleids
        var relationships = new LazyLookup<object, IEntity>();
        var userFactory = DataFactory.SpawnNew(options: UserModel.Options with
        {
            // Option to tell the entity conversion to add the "Roles" to each user
            RawConvertOptions = new(addKeys: ["Roles"]),
            Relationships = relationships,
        });

        var users = userFactory.Create(usersRaw);
        List<IEntity> roles = [];

        // If we should include the roles, create them now and attach
        if (!AddRoles)
            return l.Return((users, []), $"users {users.Count}; no roles");

        try
        {
            // Get roles and extend with the property necessary for Users to map to the roles
            roles = GetRolesStream(usersRaw);
            relationships.Add(roles.Select(r =>
                new KeyValuePair<object, IEntity>($"{UserModel.RoleRelationshipPrefix}{r.EntityId}", r)));
            return l.Return((users, roles), $"users {users.Count}; roles {roles.Count}");
        }
        catch (Exception ex)
        {
            l.A("Error trying to add roles");
            l.Ex(ex);
            /* ignore for now */
            return l.Return((users, roles), $"users {users.Count}; roles error: {roles.Count}");
        }

    }

    private List<UserModel> GetUsersAndFilter()
    {
        var l = Log.Fn<List<UserModel>>();
        var users = _provider.GetUsers(Specs)?.ToList();
        if (users == null || users.Count == 0)
            return l.Return([], "null/empty");

        return l.Return(users, $"found {users.Count}");
    }


    /// <summary>
    /// Retrieve roles and create lookup for relationship-mapper
    /// </summary>
    /// <param name="usersRaw"></param>
    /// <returns></returns>
    private List<IEntity> GetRolesStream(List<UserModel> usersRaw)
    {
        // Get list of all role IDs which are to be used
        var roleIds = usersRaw
            .SelectMany(u => u.Roles)
            .Select(r => r.Id)
            .Distinct()
            .ToList();

        // Get roles, use the current data source to provide aspects such as lookups etc.
        var rolesDs = _rolesGenerator.New(attach: this, options: new DataSourceOptionConverter().Create(null, new
        {
            // Set filter parameter to only get roles we'll need
            RoleIds = string.Join(",", roleIds),
        }));
        
        return rolesDs.List.ToList();
    }
}