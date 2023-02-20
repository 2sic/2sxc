using Oqtane.Repository;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Context.Raw;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtUsersService : UsersServiceBase
    {
        private readonly LazySvc<IUserRepository> _userRepository;

        public OqtUsersService(LazySvc<IContextOfSite> context, LazySvc<IUserRepository> userRepository) : base(context)
        {
            ConnectServices(
                _userRepository = userRepository
            );
        }

        public override string PlatformIdentityTokenPrefix => OqtConstants.UserTokenPrefix;

        public override IUser PlatformUserInformationDto(int userId)
        {
            var user = _userRepository.Value.GetUser(userId, false);
            if (user == null) return null;
            return new CmsUserRaw
            {
                Id = user.UserId,
                Name = user.Username
            };
        }
    }
}
