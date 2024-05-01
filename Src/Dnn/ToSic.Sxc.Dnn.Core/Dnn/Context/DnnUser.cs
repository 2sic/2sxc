using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Dnn.Context;

internal class DnnUser(LazySvc<DnnSecurity> dnnSecurity)
    : ServiceBase("dnnUsr", connect: [dnnSecurity]), IUser<UserInfo>
{
    private string GetUserIdentityToken ()
    {
        var userId = Id;
        var token = userId == -1 ? SxcUserConstants.Anonymous : $"{DnnConstants.UserTokenPrefix}{userId}";
        return token;
    }

    public Guid Guid => Membership.GetUser()?.ProviderUserKey is Guid realGuid ? realGuid : default;

    public string IdentityToken => GetUserIdentityToken();

    public List<int> Roles => _roles.Get(BuildRoleList);
    private readonly GetOnce<List<int>> _roles = new();

    public bool IsSystemAdmin => DnnUserInfo?.IsSuperUser ?? false;

    public bool IsSiteAdmin => AdminPermissions.IsSiteAdmin;
    public bool IsContentAdmin => AdminPermissions.IsContentAdmin;
    public bool IsSiteDeveloper => IsSystemAdmin;

    private AdminPermissions AdminPermissions => _adminPermissions.Get(
        () => DnnUserInfo.NullOrGetWith(userInfo => dnnSecurity.Value.UserMayAdminThis(userInfo)) ?? new(false)
    );
    private readonly GetOnce<AdminPermissions> _adminPermissions = new();


    private UserInfo DnnUserInfo => _user.Get(() => PortalSettings.Current?.UserInfo);
    private readonly GetOnce<UserInfo> _user = new();

    public UserInfo GetContents() => DnnUserInfo;

    private static List<int> BuildRoleList()
    {
        var psCurrent = PortalSettings.Current;
        if (psCurrent == null) return [];

        var portalId = psCurrent.PortalId;
        var user = psCurrent.UserInfo;
        if (user == null) return [];

        var rc = new DotNetNuke.Security.Roles.RoleController();
        return user.Roles
            .Select(r => rc.GetRoleByName(portalId, r))
            .Where(r => r != null)
            .Select(r => r.RoleID)
            .ToList();
    }

    public int Id => DnnUserInfo?.UserID ?? -1;

    public bool IsAnonymous => Id == -1;

    public string Username => DnnUserInfo?.Username;

    public string Name => DnnUserInfo?.DisplayName;

    public string Email => DnnUserInfo?.Email;

}