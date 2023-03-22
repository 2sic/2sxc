using Microsoft.AspNetCore.Identity;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Raw;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtSecurity : ServiceBase
    {
        private readonly LazySvc<IUserRoleRepository> _userRoleRepository;
        private readonly UserManager<IdentityUser> _identityUserManager;

        public OqtSecurity(LazySvc<IUserRoleRepository> userRoleRepository, UserManager<IdentityUser> identityUserManager) : base($"{OqtConstants.OqtLogPrefix}.User")
        {
            ConnectServices(
                _userRoleRepository = userRoleRepository,
                _identityUserManager = identityUserManager
            );
        }

        public int Id(User user) => user?.UserId ?? -1;

        public string Username(User user) => user?.Username;

        public string Name(User user) => user?.DisplayName;

        public string Email(User user) => user?.Email;

        public Guid UserGuid(string username) => new(_identityUserManager.FindByNameAsync(username).Result.Id);

        public string UserIdentityToken(User user) => $"{OqtConstants.UserTokenPrefix}{Id(user)}";

        public List<int> Roles(User user) => _userRoleRepository.Value.GetUserRoles(Id(user), user.SiteId).Select(r => r.RoleId).ToList();

        public bool IsSystemAdmin(User user) => UserSecurity.IsAuthorized(user, RoleNames.Host);

        public bool IsSiteAdmin(User user) => UserSecurity.IsAuthorized(user, RoleNames.Admin);

        public bool IsAnonymous(User user) => Id(user) == -1;

        public CmsUserRaw CmsUserBuilder(User user) =>
            new()
            {
                Id = Id(user),
                Guid = UserGuid(user.Username),
                NameId = UserIdentityToken(user),
                Roles = Roles(user),
                IsSystemAdmin = IsSystemAdmin(user),
                IsSiteAdmin = IsSiteAdmin(user),
                IsContentAdmin = IsSiteAdmin(user),
                IsAnonymous = IsAnonymous(user),
                Created = user.CreatedOn,
                Modified = user.ModifiedOn,
                Username = Username(user),
                Email = Email(user),
                Name = Name(user),
            };
    }
}
