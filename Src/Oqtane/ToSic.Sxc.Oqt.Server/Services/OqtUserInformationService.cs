using Oqtane.Repository;
using System;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Sxc.Oqt.Shared;
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

        public override string PlatformIdentityTokenPrefix() => $"{OqtConstants.UserTokenPrefix}:";

        public override UserInformationDto PlatformUserInformationDto(int userId)
        {
            var user = _userRepository.Value.GetUser(userId, false);
            if (user == null) return null;
            return new UserInformationDto()
                {
                    Id = user.UserId,
                    Name = user.Username
                };
        }
    }
}
