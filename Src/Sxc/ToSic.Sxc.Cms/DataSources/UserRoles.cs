using System.Collections.Immutable;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Sys;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Will get all (or just some) user roles of the current site.
/// </summary>
/// <remarks>
/// You can cast the result to <see cref="IUserRoleModel"/> for typed use in your code.
/// To figure out the returned properties, best also consult the <see cref="IUserRoleModel"/>.
/// 
/// History
///
/// * Not sure when it was first created, probably early 2023 with the name `Roles`, and not officially communicated.
/// * Model <see cref="IUserRoleModel"/> created in v19.01 and officially released
/// * Renamed to `UserRoles` for consistency in v19.0 as we believe nobody has been actively using it yet, since the models were missing.
/// </remarks>
[PublicApi]
[VisualQuery(
    NiceName = "User Roles",
    Icon = DataSourceIcons.UserCircled,
    UiHint = "User Roles in this site",
    HelpLink = "https://go.2sxc.org/ds-roles",
    NameId = "eee54266-d7ad-4f5e-9422-2d00c8f93b45",
    Type = DataSourceType.Source,
    ConfigurationType = "1b9fd9d1-dde0-40ad-bb66-5cd7f30de18d"
)]
public class UserRoles : CustomDataSourceAdvanced
{
    private readonly IUserRolesProvider _provider;

    #region Other Constants

    private const char Separator = ',';

    #endregion

    #region Configuration-properties

    /// <summary>
    /// Optional (single value or comma-separated integers) filter,
    /// include roles based on roleId
    /// </summary>
    [Configuration]
    public string? RoleIds
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    /// <summary>
    /// Optional (single value or comma-separated integers) filter,
    /// exclude roles based on roleId
    /// </summary>
    [Configuration]
    public string? ExcludeRoleIds
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    #endregion


    #region Constructor

    /// <summary>
    /// Constructor to tell the system what out-streams we have
    /// </summary>
    [PrivateApi]
    public UserRoles(Dependencies services, IUserRolesProvider provider)
        : base(services, "SDS.Roles", connect: [provider])
    {
        _provider = provider;

        ProvideOut(GetList);
    }

    #endregion

    private IImmutableList<IEntity> GetList()
    {
        var l = Log.Fn<IImmutableList<IEntity>>();
        var roles = _provider.GetRoles()?.ToList();
        l.A($"found {roles?.Count} roles");

        if (roles.SafeNone()) 
            return l.Return([], "null/empty");

        // This will resolve the tokens before starting
        Configuration.Parse();

        var includeRolesPredicate = KeepRolesCondition();
        l.A($"includeRoles: {includeRolesPredicate == null}");
        if (includeRolesPredicate != null)
            roles = roles
                .Where(includeRolesPredicate)
                .ToList();

        var excludeRolesPredicate = DropRolesCondition();
        l.A($"excludeRoles: {excludeRolesPredicate == null}");
        if (excludeRolesPredicate != null)
            roles = roles
                .Where(excludeRolesPredicate)
                .ToList();

        var rolesFactory = DataFactory.SpawnNew(options: UserRoleModel.Options);

        var result = rolesFactory.Create(roles);

        return l.Return(result, $"found {result.Count} roles");
    }

    private Func<UserRoleModel, bool>? KeepRolesCondition()
    {
        var includeRolesFilter = RolesCsvListToInt(RoleIds);
        return includeRolesFilter.Any()
            ? r => includeRolesFilter.Contains(r.Id)
            : null;
    }

    private Func<UserRoleModel, bool>? DropRolesCondition()
    {
        var excludeRolesFilter = RolesCsvListToInt(ExcludeRoleIds);
        return excludeRolesFilter.Any()
            ? r => !excludeRolesFilter.Contains(r.Id)
            : null;
    }

    [PrivateApi]
    internal static List<int> RolesCsvListToInt(string? stringList)
        => !stringList.HasValue()
            ? []
            : stringList.Split(Separator)
                .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : int.MinValue)
                .Where(r => r != int.MinValue)
                .ToList();
}