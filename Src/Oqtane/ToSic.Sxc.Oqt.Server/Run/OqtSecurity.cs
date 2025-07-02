using Microsoft.AspNetCore.Identity;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Lib.DI;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class OqtSecurity(LazySvc<IUserRoleRepository> userRoleRepository, UserManager<IdentityUser> identityUserManager)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.User", connect: [userRoleRepository, identityUserManager])
{
    public int Id(User user) => user?.UserId ?? -1;

    public string Username(User user) => user?.Username;

    public string Name(User user) => user?.DisplayName;

    public string Email(User user) => user?.Email;

    public Guid UserGuid(string username) => new(identityUserManager.FindByNameAsync(username).Result.Id);

    public string UserIdentityToken(User user) => $"{OqtConstants.UserTokenPrefix}{Id(user)}";

    public List<int> Roles(User user) => userRoleRepository.Value.GetUserRoles(Id(user), user.SiteId).Select(r => r.RoleId).ToList();
    public List<UserRoleModel> Roles2(User user) => userRoleRepository.Value
        .GetUserRoles(Id(user), user.SiteId)
        .Select(r => new UserRoleModel
        {
            Id = r.RoleId,
            Name = r.Role.Name,
            Created = r.CreatedOn,
            Modified = r.ModifiedOn,
        })
        .ToList();

    public bool IsSystemAdmin(User user) => UserSecurity.IsAuthorized(user, RoleNames.Host);

    public bool IsSiteAdmin(User user) => UserSecurity.IsAuthorized(user, RoleNames.Admin);

    public bool IsAnonymous(User user) => Id(user) == -1;

    public UserModel CmsUserBuilder(User user)
    {
        var isSiteAdmin = IsSiteAdmin(user);
        return new()
        {
            Id = Id(user),
            Guid = UserGuid(user.Username),
            NameId = UserIdentityToken(user),
            //RolesRaw = Roles(user),
            Roles = Roles2(user),
            IsSystemAdmin = IsSystemAdmin(user),
            IsSiteAdmin = isSiteAdmin,
            IsContentAdmin = isSiteAdmin,
            IsContentEditor = isSiteAdmin,
            IsAnonymous = IsAnonymous(user),
            Created = user.CreatedOn,
            Modified = user.ModifiedOn,
            Username = Username(user),
            Email = Email(user),
            Name = Name(user),
        };
    }
}