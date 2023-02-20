using Microsoft.AspNetCore.Identity;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Sxc.Oqt.Shared;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    public class OqtUsersDsProvider : UsersDataSourceProvider
    {
        private readonly IUserRoleRepository _userRoles;
        private readonly SiteState _siteState;
        private readonly UserManager<IdentityUser> _identityUserManager;

        public OqtUsersDsProvider(IUserRoleRepository userRoles, SiteState siteState, UserManager<IdentityUser> identityUserManager) : base("Oqt.Users")
        {
            ConnectServices(
                _userRoles = userRoles,
                _siteState = siteState,
                _identityUserManager = identityUserManager
            );
        }

        public override IEnumerable<UserDataRaw> GetUsersInternal()
            => Log.Func(l =>
            {
                var siteId = _siteState.Alias.SiteId;
                l.A($"Portal Id {siteId}");
                try
                {
                    var userRoles = _userRoles.GetUserRoles(siteId).ToList();
                    var users = userRoles.Select(ur => ur.User).Distinct().ToList();
                    if (!users.Any()) return (new(), "null/empty");

                    var result = users
                        .Where(u => !u.IsDeleted)
                        .Select(u =>
                        {
                            var isSiteAdmin = userRoles.Any(ur =>
                                ur.UserId == u.UserId && ur.Role.Name == RoleNames.Admin);
                            return new UserDataRaw
                            {
                                Id = u.UserId,
                                Guid = new((_identityUserManager.FindByNameAsync(u.Username).Result).Id),
                                NameId = $"{OqtConstants.UserTokenPrefix}:{u.UserId}",
                                RoleIds = userRoles.Where(ur => ur.UserId == u.UserId).Select(ur => ur.RoleId).ToList(),
                                IsSystemAdmin =
                                    userRoles.Any(ur => ur.UserId == u.UserId && ur.Role.Name == RoleNames.Host),
                                IsSiteAdmin = isSiteAdmin,
                                IsContentAdmin = isSiteAdmin,
                                //IsDesigner = userRoles.Any(ur => ur.UserId == u.UserId && ur.Role.Name == RoleNames.Host),
                                IsAnonymous = u.UserId == -1,
                                Created = u.CreatedOn,
                                Modified = u.ModifiedOn,
                                Username = u.Username,
                                Email = u.Email,
                                Name = u.DisplayName,
                            };
                        }).ToList();
                    return (result, "found");
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    return (new(), "error");
                }
            });
    }
}