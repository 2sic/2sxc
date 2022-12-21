using Oqtane.Repository;
using System;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtUserInformationService : UserInformationServiceBase
    {
        private readonly ILazySvc<IUserRepository> _userRepository;

        public OqtUserInformationService(LazyInit<IContextOfSite> context, ILazySvc<IUserRepository> userRepository) : base(context)
        {
            ConnectServices(
                _userRepository = userRepository
            );
        }

        public override string PlatformIdentityTokenPrefix() => $"{OqtConstants.UserTokenPrefix}:";

        public override UserInformationDto PlatformUserInformationDto(int userId)
        {
            var user = _userRepository.Value.GetUser(userId, false);
            if (user == null) return null;
            return new()
                {
                    Id = user.UserId,
                    Name = user.Username
                };
        }
    }
}
