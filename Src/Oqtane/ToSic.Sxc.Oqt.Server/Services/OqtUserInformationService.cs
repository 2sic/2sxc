using Oqtane.Repository;
using System;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtUserInformationService : UserInformationServiceBase
    {
        private readonly Lazy<IUserRepository> _userRepository;

        public OqtUserInformationService(LazyInitLog<IContextOfSite> context, Lazy<IUserRepository> userRepository) : base(context)
        {
            _userRepository = userRepository;
        }

        public override string PlatformIdentityTokenPrefix() => "Oqt:";

        public override UserInformationDto Find(string identityToken)
        {
            var wrapLog = Log.Call<UserInformationDto>($"t:{identityToken}");
            var user = _userRepository.Value.GetUser(UserId(identityToken), false);
            return (user == null)
                ? wrapLog("Err", UserUnknown)
                : wrapLog("Ok", new UserInformationDto()
                {
                    Id = user.UserId,
                    Name = user.Username
                });
        }
    }
}
