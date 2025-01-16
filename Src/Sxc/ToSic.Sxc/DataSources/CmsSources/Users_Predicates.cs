using ToSic.Eav.Plumbing;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.DataSources;

public partial class Users
{
    private List<FilterOp<UserModel>> GetAllFilters2()
    {
        var l = Log.Fn<List<FilterOp<UserModel>>>();
        var filters = new List<FilterOp<UserModel>>
        {
            KeepUsersCondition(),
            ExcludeUsersPredicate(),
            KeepUsersInRoles(),
            DropUsersInRoles(),
            SuperUserCondition(),
        };

        filters = filters
            .Where(f => f != null)
            .ToList();

        return l.Return(filters, $"{filters.Count}");
    }

    private FilterOp<UserModel> KeepUsersCondition()
    {
        if (string.IsNullOrEmpty(UserIds))
            return null;

        var keepGuids = FilterKeepUserGuids();
        var keepIds = FilterKeepUserIds();
        if (keepGuids == null && keepIds == null)
            return null;

        return new(u => (keepGuids != null && keepGuids(u)) || (keepIds != null && keepIds(u)));


        Func<UserModel, bool> FilterKeepUserGuids()
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

        Func<UserModel, bool> FilterKeepUserIds()
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
    }




    private FilterOp<UserModel> ExcludeUsersPredicate()
    {
        if (string.IsNullOrEmpty(ExcludeUserIds))
            return null;

        var dropGuids = FilterExcludeUserGuids();
        var dropIds = FilterExcludeUserIds();
        if (dropGuids == null && dropIds == null)
            return null;

        return new(u => (dropGuids == null || dropGuids(u))
                        && (dropIds == null || dropIds(u))
        );


        Func<UserModel, bool> FilterExcludeUserGuids()
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

        Func<UserModel, bool> FilterExcludeUserIds()
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
    }



    private FilterOp<UserModel> KeepUsersInRoles()
    {
        var rolesFilter = UserRoles.RolesCsvListToInt(RoleIds);
        return rolesFilter.Any()
            ? new(u => u.RolesRaw.Any(r => rolesFilter.Contains(r)))
            : null;
    }

    private FilterOp<UserModel> DropUsersInRoles()
    {
        var excludeRolesFilter = UserRoles.RolesCsvListToInt(ExcludeRoleIds);
        return excludeRolesFilter.Any()
            ? new(u => !u.RolesRaw.Any(r => excludeRolesFilter.Contains(r)))
            : null;
    }

    private FilterOp<UserModel> SuperUserCondition()
    {
        var l = Log.Fn<FilterOp<UserModel>>();
        // If "include" == "only" return only superusers
        if (IncludeSystemAdmins.EqualsInsensitive(IncludeRequired))
            return l.Return(new(u => u.IsSystemAdmin, IncludeRequired));

        // If "include" == true, return all
        if (IncludeSystemAdmins.EqualsInsensitive(IncludeOptional))
            return l.ReturnNull($"{IncludeOptional} = any"); // skip IsSystemAdmin check will return normal and superusers

        // If "include" == false - or basically any unknown value, return only normal users
        return l.Return(new(u => !u.IsSystemAdmin, IncludeForbidden));
    }
}