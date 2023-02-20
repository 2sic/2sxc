using Microsoft.AspNetCore.Identity;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;
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

        public override IEnumerable<CmsUserRaw> GetUsersInternal() => Log.Func(l =>
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
                        var myRoles = userRoles.Where(ur => ur.UserId == u.UserId).ToList();
                        var isSiteAdmin = myRoles.Any(ur => ur.Role.Name == RoleNames.Admin);
                        return new CmsUserRaw
                        {
                            Id = u.UserId,
                            Guid = new((_identityUserManager.FindByNameAsync(u.Username).Result).Id),
                            NameId = $"{OqtConstants.UserTokenPrefix}{u.UserId}",
                            Roles = myRoles.Select(ur => ur.RoleId).ToList(),
                            IsSystemAdmin = myRoles.Any(ur => ur.Role.Name == RoleNames.Host),
                            IsSiteAdmin = isSiteAdmin,
                            IsContentAdmin = isSiteAdmin,
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