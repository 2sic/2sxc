namespace ToSic.Sxc.Cms.Users.Sys;

public record UsersGetSpecs
{
    /// <summary>
    /// Optional Users (single value or comma-separated guids or integers) filter,
    /// include users based on guid or id
    /// </summary>
    public string? UserIds { get; init; }

    /// <summary>
    /// Optional exclude Users (single value or comma-separated guids or integers) filter,
    /// exclude users based on guid or id
    /// </summary>
    public string? ExcludeUserIds { get; init; }

    /// <summary>
    /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
    /// include users that have any of roles from filter
    /// </summary>
    public string? RoleIds { get; init; }

    /// <summary>
    /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
    /// exclude users that have any of roles from filter
    /// </summary>
    public string? ExcludeRoleIds { get; init; }

    /// <summary>
    /// Optional SystemAdmins filter.
    /// 
    /// * `true` - with System Admins
    /// * `false` - without System Admins
    /// * `required` - only System Admins (no normal users)
    /// </summary>
    public string? IncludeSystemAdmins { get; init; }

    /// <summary>
    /// Add property `Roles` as a relationship to role entities.
    /// </summary>
    public bool AddRoles { get; init; }

}