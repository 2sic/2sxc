using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Services
{
    public class DnnUserInformationService : UserInformationServiceBase
    {

        public DnnUserInformationService(LazyInitLog<IContextOfSite> context) : base(context)
        { }

        public override string PlatformIdentityTokenPrefix() => DnnConstants.UserTokenPrefix;

        public override UserInformationDto Find(string identityToken)
        {
            var wrapLog = Log.Call<UserInformationDto>($"t:{identityToken}");
            var user = UserController.Instance.GetUserById(SiteId, UserId(identityToken));
            return wrapLog("OK", (user == null) 
                                    ?  UserUnknown 
                                    : new UserInformationDto()
                                    {
                                        Id = user.UserID,
                                        Name = user.Username
                                    });
        }
    }
}
