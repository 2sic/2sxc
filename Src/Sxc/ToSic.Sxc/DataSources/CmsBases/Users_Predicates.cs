using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public partial class Users
    {
        private List<Func<CmsUserInfo, bool>> GetAllFilters() => Log.Func(l =>
        {
            var filters = new List<Func<CmsUserInfo, bool>>
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


        private Func<CmsUserInfo, bool> IncludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(UserIds)) return null;
            var includeUserGuids = FilterKeepUserGuids();
            var includeUserIds = FilterKeepUserIds();
            if (includeUserGuids == null && includeUserIds == null) return null;
            return u => (includeUserGuids != null && includeUserGuids(u)) || (includeUserIds != null && includeUserIds(u));
        }

        private Func<CmsUserInfo, bool> FilterKeepUserGuids()
        {
            var userGuidFilter = UserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return userGuidFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => u.Guid != Guid.Empty && userGuidFilter.Contains(u.Guid))
                : null;
        }

        private Func<CmsUserInfo, bool> FilterKeepUserIds()
        {
            var userIdFilter = UserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return userIdFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => userIdFilter.Contains(u.Id))
                : null;
        }

        private Func<CmsUserInfo, bool> ExcludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeUserIds)) return null;
            var excludeUserGuids = FilterExcludeUserGuids();
            var excludeUserIds = FilterExcludeUserIds();
            if (excludeUserGuids == null && excludeUserIds == null) return null;
            return u => (excludeUserGuids == null || excludeUserGuids(u)) && (excludeUserIds == null || excludeUserIds(u));
        }

        private Func<CmsUserInfo, bool> FilterExcludeUserGuids()
        {
            var excludeUserGuidsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return excludeUserGuidsFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => u.Guid != Guid.Empty && !excludeUserGuidsFilter.Contains(u.Guid))
                : null;
        }

        private Func<CmsUserInfo, bool> FilterExcludeUserIds()
        {
            var excludeUserIdsFilter = ExcludeUserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return excludeUserIdsFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => !excludeUserIdsFilter.Contains(u.Id))
                : null;
        }

        private Func<CmsUserInfo, bool> FilterIncludeUsersOfRoles()
        {
            var rolesFilter = Roles.RolesCsvListToInt(RoleIds);
            return rolesFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => u.RoleIds.Any(r => rolesFilter.Contains(r)))
                : null;
        }

        private Func<CmsUserInfo, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = Roles.RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<CmsUserInfo, bool>)(u => !u.RoleIds.Any(r => excludeRolesFilter.Contains(r)))
                : null;
        }

        private Func<CmsUserInfo, bool> SuperUserPredicate() =>
            IncludeSystemAdmins
                ? (u => true) // skip IsSystemAdmin check will return normal and super users
                : (Func<CmsUserInfo, bool>)(u => !u.IsSystemAdmin); // only normal users
    }
}
