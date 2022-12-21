using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Services
{
    public class DnnUserInformationService : UserInformationServiceBase
    {

        public DnnUserInformationService(LazyInit<IContextOfSite> context) : base(context)
        { }

        public override string PlatformIdentityTokenPrefix() => DnnConstants.UserTokenPrefix;

        public override UserInformationDto PlatformUserInformationDto(int userId)
        {
            var user = UserController.Instance.GetUserById(SiteId, userId);
            if (user == null) return null;
            return new UserInformationDto()
            {
                Id = user.UserID,
                Name = user.Username
            };
        }
    }
}
