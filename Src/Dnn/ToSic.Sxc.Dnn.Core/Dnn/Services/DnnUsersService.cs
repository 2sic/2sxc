using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Services
{
    public class DnnUsersService : UsersServiceBase
    {

        public DnnUsersService(LazySvc<IContextOfSite> context) : base(context)
        { }

        public override string PlatformIdentityTokenPrefix => DnnConstants.UserTokenPrefix;

        public override IUser PlatformUserInformationDto(int userId)
        {
            var user = UserController.Instance.GetUserById(SiteId, userId);
            if (user == null) return null;
            return user.CmsUserBuilder(SiteId);
        }
    }
}
