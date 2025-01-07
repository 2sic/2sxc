using ToSic.Eav.Plumbing;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.DataSources;

public partial class Users
{
    private List<FilterOp<UserRaw>> GetAllFilters2()
    {
        var l = Log.Fn<List<FilterOp<UserRaw>>>();
        var filters = new List<FilterOp<UserRaw>>
        {
            IncludeUsersPredicate().NullOrGetWith(f => new FilterOp<UserRaw>(nameof(IncludeUsersPredicate), f)),
            ExcludeUsersPredicate().NullOrGetWith(f => new FilterOp<UserRaw>(nameof(ExcludeUsersPredicate), f)),
            FilterIncludeUsersOfRoles().NullOrGetWith(f => new FilterOp<UserRaw>(nameof(FilterIncludeUsersOfRoles), f)),
            ExcludeRolesPredicate().NullOrGetWith(f => new FilterOp < UserRaw >(nameof(ExcludeRolesPredicate), f)),
            SuperUserPredicate().NullOrGetWith(f => new FilterOp < UserRaw >(nameof(SuperUserPredicate), f))
        };

        filters = filters
            .Where(f => f != null)
            .ToList();

        return l.Return(filters, $"{filters.Count}");
    }

    private List<Func<UserRaw, bool>> GetAllFilters()
    {
        var l = Log.Fn<List<Func<UserRaw, bool>>>();
        var filters = new List<Func<UserRaw, bool>>
        {
            IncludeUsersPredicate(),
            ExcludeUsersPredicate(),
            FilterIncludeUsersOfRoles(),
            ExcludeRolesPredicate(),
            SuperUserPredicate()
        };

        filters = filters
            .Where(f => f != null)
            .ToList();

        return l.Return(filters, $"{filters.Count}");
    }


    private Func<UserRaw, bool> IncludeUsersPredicate()
    {
        if (string.IsNullOrEmpty(UserIds))
            return null;
        var includeUserGuids = FilterKeepUserGuids();
        var includeUserIds = FilterKeepUserIds();
        if (includeUserGuids == null && includeUserIds == null) return null;
        return u => (includeUserGuids != null && includeUserGuids(u))
                    || (includeUserIds != null && includeUserIds(u));
    }

    private Func<UserRaw, bool> FilterKeepUserGuids()
    {
        var userGuidFilter = UserIds
            .Split(Separator)
            .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
            .Where(u => u != Guid.Empty)
            .ToList();
        return userGuidFilter.Any()
            ? u => u.Guid != Guid.Empty && userGuidFilter.Contains(u.Guid)
            : null;
    }

    private Func<UserRaw, bool> FilterKeepUserIds()
    {
        var userIdFilter = UserIds
            .Split(Separator)
            .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
            .Where(u => u != -1)
            .ToList();
        return userIdFilter.Any()
            ? u => userIdFilter.Contains(u.Id)
            : null;
    }

    private Func<UserRaw, bool> ExcludeUsersPredicate()
    {
        if (string.IsNullOrEmpty(ExcludeUserIds))
            return null;
        var excludeUserGuids = FilterExcludeUserGuids();
        var excludeUserIds = FilterExcludeUserIds();
        if (excludeUserGuids == null && excludeUserIds == null) return null;
        return u => (excludeUserGuids == null || excludeUserGuids(u))
                    && (excludeUserIds == null || excludeUserIds(u));
    }

    private Func<UserRaw, bool> FilterExcludeUserGuids()
    {
        var excludeUserGuidsFilter = ExcludeUserIds
            .Split(Separator)
            .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
            .Where(u => u != Guid.Empty)
            .ToList();
        return excludeUserGuidsFilter.Any()
            ? u => u.Guid != Guid.Empty && !excludeUserGuidsFilter.Contains(u.Guid)
            : null;
    }

    private Func<UserRaw, bool> FilterExcludeUserIds()
    {
        var excludeUserIdsFilter = ExcludeUserIds
            .Split(Separator)
            .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
            .Where(u => u != -1)
            .ToList();
        return excludeUserIdsFilter.Any()
            ? u => !excludeUserIdsFilter.Contains(u.Id)
            : null;
    }

    private Func<UserRaw, bool> FilterIncludeUsersOfRoles()
    {
        var rolesFilter = Roles.RolesCsvListToInt(RoleIds);
        return rolesFilter.Any()
            ? u => u.Roles.Any(r => rolesFilter.Contains(r))
            : null;
    }

    private Func<UserRaw, bool> ExcludeRolesPredicate()
    {
        var excludeRolesFilter = Roles.RolesCsvListToInt(ExcludeRoleIds);
        return excludeRolesFilter.Any()
            ? u => !u.Roles.Any(r => excludeRolesFilter.Contains(r))
            : null;
    }

    private Func<UserRaw, bool> SuperUserPredicate()
    {
        var l = Log.Fn<Func<UserRaw, bool>>();
        // If "include" == "only" return only super users
        if (IncludeSystemAdmins.EqualsInsensitive(IncludeRequired))
            return l.Return(u => u.IsSystemAdmin, IncludeRequired);

        // If "include" == true, return all
        if (IncludeSystemAdmins.EqualsInsensitive(IncludeOptional))
            return l.ReturnNull($"{IncludeOptional} = any"); // skip IsSystemAdmin check will return normal and super users

        // If "include" == false - or basically any unknown value, return only normal users
        return l.Return(u => !u.IsSystemAdmin, IncludeForbidden);
    }
}