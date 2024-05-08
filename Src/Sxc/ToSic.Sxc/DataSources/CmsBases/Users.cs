using System.Collections.Immutable;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Source;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Services;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.DataSources.Internal;
using static ToSic.Eav.DataSource.Internal.DataSourceConstants;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Will get all (or just some) users of the current site.
/// The resulting Entity will match the <see cref="IUser"/> interface.
/// </summary>
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
    private readonly IDataSourceGenerator<Roles> _rolesGenerator;
    private readonly IDataFactory _dataFactory;
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
        get => _userIds ?? Configuration.GetThis();
        set => _userIds = value;
    }
    private string _userIds;
    /// <summary>
    /// Optional exclude Users (single value or comma-separated guids or integers) filter,
    /// exclude users based on guid or id
    /// </summary>
    [Configuration]
    public string ExcludeUserIds
    {
        get => _excludeUserIds ?? Configuration.GetThis();
        set => _excludeUserIds = value;
    }
    private string _excludeUserIds;

    /// <summary>
    /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
    /// include users that have any of roles from filter
    /// </summary>
    [Configuration]
    public string RoleIds
    {
        get => _roleIds ?? Configuration.GetThis();
        set => _roleIds = value;
    }
    private string _roleIds;

    /// <summary>
    /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
    /// exclude users that have any of roles from filter
    /// </summary>
    [Configuration]
    public string ExcludeRoleIds
    {
        get => _excludeRoleIds ?? Configuration.GetThis();
        set => _excludeRoleIds = value;
    }
    private string _excludeRoleIds;

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
        get => _includeSystemAdmins ?? Configuration.GetThis();
        set => _includeSystemAdmins = value;
    }
    private string _includeSystemAdmins;

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
        get => _addRoles ?? Configuration.GetThis(true);
        set => _addRoles = value;
    }
    private bool? _addRoles;

    #endregion


    #region Constructor

    /// <summary>
    /// Constructor to tell the system what out-streams we have
    /// </summary>
    [PrivateApi]
    public Users(MyServices services,
        UsersDataSourceProvider provider,
        IDataFactory dataFactory,
        IDataSourceGenerator<Roles> rolesGenerator) : base(services, "SDS.Users")
    {
        ConnectLogs([
            _provider = provider,
            _dataFactory = dataFactory,
            _rolesGenerator = rolesGenerator
        ]);
        ProvideOut(() => GetUsersAndRoles().Users); // default out, if accessed, will deliver GetList
        ProvideOut(() => GetUsersAndRoles().UserRoles, "Roles");
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
        var userFactory = _dataFactory.New(options: CmsUserRaw.Options,
            relationships: relationships,
            rawConvertOptions: new(addKeys: new []{ "Roles"}));

        var users = userFactory.Create(usersRaw);
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
        if (users == null || !users.Any()) return ([], "null/empty");

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
        var rolesDs = _rolesGenerator.New(attach: this);
        // Set filter parameter to only get roles we'll need
        rolesDs.RoleIds = string.Join(",", roleIds);
        var roles = rolesDs.List;
        return roles.ToImmutableList();
    }
}