using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Internal;
using static ToSic.Sxc.Dnn.Run.DnnSecurity;

namespace ToSic.Sxc.Dnn.Context;

internal class DnnUser: ServiceBase, IUser<UserInfo>
{
    private readonly LazySvc<DnnSecurity> _dnnSecurity;

    public DnnUser(LazySvc<DnnSecurity> dnnSecurity) : base("dnnUsr")
    {
        ConnectServices(
            _dnnSecurity = dnnSecurity
        );
    }

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

    public bool IsSystemAdmin => UnwrappedContents?.IsSuperUser ?? false;

    public bool IsSiteAdmin => _getAdminPermissions().IsSiteAdmin;
    public bool IsContentAdmin => _getAdminPermissions().IsContentAdmin;
    public bool IsSiteDeveloper => IsSystemAdmin;

    private DnnSiteAdminPermissions _getAdminPermissions() => _adminPermissions.Get(
        () => UnwrappedContents != null 
            ? _dnnSecurity.Value.UserMayAdminThis(UnwrappedContents) 
            : new DnnSiteAdminPermissions(false)
    );
    private readonly GetOnce<DnnSiteAdminPermissions> _adminPermissions = new();


    public UserInfo UnwrappedContents => _user.Get(() => PortalSettings.Current?.UserInfo);
    private readonly GetOnce<UserInfo> _user = new();

    public UserInfo GetContents() => UnwrappedContents;

    private static List<int> BuildRoleList()
    {
        var psCurrent = PortalSettings.Current;
        if (psCurrent == null) return new List<int>();

        var portalId = psCurrent.PortalId;
        var user = psCurrent.UserInfo;
        if (user == null) return new List<int>();

        var rc = new DotNetNuke.Security.Roles.RoleController();
        return user.Roles
            .Select(r => rc.GetRoleByName(portalId, r))
            .Where(r => r != null)
            .Select(r => r.RoleID)
            .ToList();
    }

    public int Id => UnwrappedContents?.UserID ?? -1;

    public bool IsAnonymous => Id == -1;

    public string Username => UnwrappedContents?.Username;

    public string Name => UnwrappedContents?.DisplayName;

    public string Email => UnwrappedContents?.Email;

    #region Removed in v15.03 2023-02-20 - already deprecated in v14.09 and probably never used outside of core code

    //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
    //public bool IsSuperUser => IsSystemAdmin;

    //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
    //public bool IsAdmin => IsSiteAdmin;

    #endregion
}