using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public partial class Users
    {
        private List<Func<UserDataRaw, bool>> GetAllFilters() => Log.Func(l =>
        {
            var filters = new List<Func<UserDataRaw, bool>>
            {
                IncludeUsersPredicate(),
                ExcludeUsersPredicate(),
                FilterIncludeUsersOfRoles(),
                ExcludeRolesPredicate(),
                SuperUserPredicate()
            };
            filters = filters.Where(f => f != null).ToList();
            return (filters, $"{filters.Count}");
        });


        private Func<UserDataRaw, bool> IncludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(UserIds)) return null;
            var includeUserGuids = FilterKeepUserGuids();
            var includeUserIds = FilterKeepUserIds();
            if (includeUserGuids == null && includeUserIds == null) return null;
            return u => (includeUserGuids != null && includeUserGuids(u)) || (includeUserIds != null && includeUserIds(u));
        }

        private Func<UserDataRaw, bool> FilterKeepUserGuids()
        {
            var userGuidFilter = UserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return userGuidFilter.Any()
                ? (Func<UserDataRaw, bool>)(u => u.Guid != Guid.Empty && userGuidFilter.Contains(u.Guid))
                : null;
        }

        private Func<UserDataRaw, bool> FilterKeepUserIds()
        {
            var userIdFilter = UserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return userIdFilter.Any()
                ? (Func<UserDataRaw, bool>)(u => userIdFilter.Contains(u.Id))
                : null;
        }

        private Func<UserDataRaw, bool> ExcludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeUserIds)) return null;
            var excludeUserGuids = FilterExcludeUserGuids();
            var excludeUserIds = FilterExcludeUserIds();
            if (excludeUserGuids == null && excludeUserIds == null) return null;
            return u => (excludeUserGuids == null || excludeUserGuids(u)) && (excludeUserIds == null || excludeUserIds(u));
        }

        private Func<UserDataRaw, bool> FilterExcludeUserGuids()
        {
            var excludeUserGuidsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return excludeUserGuidsFilter.Any()
                ? (Func<UserDataRaw, bool>)(u => u.Guid != Guid.Empty && !excludeUserGuidsFilter.Contains(u.Guid))
                : null;
        }

        private Func<UserDataRaw, bool> FilterExcludeUserIds()
        {
            var excludeUserIdsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return excludeUserIdsFilter.Any()
                ? (Func<UserDataRaw, bool>)(u => !excludeUserIdsFilter.Contains(u.Id))
                : null;
        }

        private Func<UserDataRaw, bool> FilterIncludeUsersOfRoles()
        {
            var rolesFilter = Roles.RolesCsvListToInt(RoleIds);
            return rolesFilter.Any()
                ? (Func<UserDataRaw, bool>)(u => u.RoleIds.Any(r => rolesFilter.Contains(r)))
                : null;
        }

        private Func<UserDataRaw, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = Roles.RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<UserDataRaw, bool>)(u => !u.RoleIds.Any(r => excludeRolesFilter.Contains(r)))
                : null;
        }

        private Func<UserDataRaw, bool> SuperUserPredicate() =>
            IncludeSystemAdmins
                ? (u => true) // skip IsSystemAdmin check will return normal and super users
                : (Func<UserDataRaw, bool>)(u => !u.IsSystemAdmin); // only normal users
    }
}
