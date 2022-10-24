using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Services
{
    public class UserInformationServiceUnknown : UserInformationServiceBase, IIsUnknown
    {
        public UserInformationServiceUnknown(WarnUseOfUnknown<UserInformationServiceUnknown> warn, LazyInitLog<IContextOfSite> context) : base(context)
        { }

        public override string PlatformIdentityTokenPrefix() => Unknown;

        public override UserInformationDto PlatformUserInformationDto(int userId) => null;
    }
}
