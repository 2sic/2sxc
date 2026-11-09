using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Dnn.Context;

/// <summary>
/// Factory to initialize the IUser object within DNN.
/// </summary>
internal class DnnUserFactory(DnnSecurity dnnSecurity)
    : ServiceBase("Dnn.UsrFac", connect: [dnnSecurity])
{
    public IUser GetUser()
    {
        var l = Log.Fn<IUser>();

        var userInfo = PortalSettings.Current?.UserInfo;
        var userId = userInfo?.UserID ?? -1;

        var permissions = userInfo == null
            ? null
            : dnnSecurity.UserPermissions(userInfo);

        var user = new DnnUser(userInfo, permissions)
        {
            Id = userId,
            Guid = dnnSecurity.UserGuid(userInfo),
            IdentityToken = dnnSecurity.UserIdentityToken(userInfo),
            Roles = [.. dnnSecurity.UserRoleInfos(userInfo, PortalSettings.Current?.PortalId ?? -1).Select(r => r.RoleID)],
        };

        return l.Return(user, $"{user.Id}");
    }
}