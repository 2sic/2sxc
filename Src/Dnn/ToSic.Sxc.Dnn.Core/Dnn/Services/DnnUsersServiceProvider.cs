using DotNetNuke.Entities.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnUsersServiceProvider(LazySvc<DnnSecurity> dnnSecurity)
    : UserSourceProvider("Dnn.UsersSvc", connect: [dnnSecurity])
{
    public override string PlatformIdentityTokenPrefix => DnnConstants.UserTokenPrefix;

    internal override ICmsUser PlatformUserInformationDto(int userId, int siteId)
    {
        var user = UserController.Instance.GetUserById(siteId, userId);
        return user == null
            ? null
            : dnnSecurity.Value.CmsUserBuilder(user, siteId);
    }
}