using Oqtane.Repository;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Oqt.Server.Services;

internal class OqtUsersService : UsersServiceBase
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

    protected override ICmsUser PlatformUserInformationDto(int userId)
    {
        var user = _userRepository.Value.GetUser(userId, false);
        return user == null ? null : _oqtSecurity.Value.CmsUserBuilder(user);
    }
}