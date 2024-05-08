using DotNetNuke.Entities.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnUsersServiceProvider : UserSourceProvider
{
    private readonly LazySvc<DnnSecurity> _dnnSecurity;

    public DnnUsersServiceProvider(LazySvc<DnnSecurity> dnnSecurity) : base("Dnn.UsersSvc")
    {
        ConnectLogs([
            _dnnSecurity = dnnSecurity
        ]);
    }

    public override string PlatformIdentityTokenPrefix => DnnConstants.UserTokenPrefix;

    internal override ICmsUser PlatformUserInformationDto(int userId, int siteId)
    {
        var user = UserController.Instance.GetUserById(siteId, userId);
        if (user == null) return null;
        return _dnnSecurity.Value.CmsUserBuilder(user, siteId);
    }
}