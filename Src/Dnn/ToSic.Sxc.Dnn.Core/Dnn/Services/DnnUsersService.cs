using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnUsersService : UsersServiceBase
{
    private readonly LazySvc<DnnSecurity> _dnnSecurity;

    public DnnUsersService(LazySvc<IContextOfSite> context, LazySvc<DnnSecurity> dnnSecurity) : base(context)
    {
        ConnectServices(
            _dnnSecurity = dnnSecurity
        );
    }

    public override string PlatformIdentityTokenPrefix => DnnConstants.UserTokenPrefix;

    public override IUser PlatformUserInformationDto(int userId)
    {
        var user = UserController.Instance.GetUserById(SiteId, userId);
        if (user == null) return null;
        return _dnnSecurity.Value.CmsUserBuilder(user, SiteId);
    }
}