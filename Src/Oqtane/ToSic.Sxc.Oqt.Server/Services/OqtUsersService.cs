using Oqtane.Repository;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtUsersService : UsersServiceBase
    {
        private readonly LazySvc<IUserRepository> _userRepository;
        private readonly LazySvc<OqtSecurity> _oqtSecurity;

        public OqtUsersService(LazySvc<IContextOfSite> context, LazySvc<IUserRepository> userRepository, LazySvc<OqtSecurity> oqtSecurity) : base(context)
        {

            ConnectServices(
                _userRepository = userRepository,
                _oqtSecurity = oqtSecurity
            );
        }

        public override string PlatformIdentityTokenPrefix => OqtConstants.UserTokenPrefix;

        public override IUser PlatformUserInformationDto(int userId)
        {
            var user = _userRepository.Value.GetUser(userId, false);
            return user == null ? null : _oqtSecurity.Value.CmsUserBuilder(user);
        }
    }
}
