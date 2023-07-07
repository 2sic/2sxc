using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Context.Raw;

namespace ToSic.Sxc.Services
{
    internal class UsersServiceUnknown : UsersServiceBase, IIsUnknown
    {
        public UsersServiceUnknown(WarnUseOfUnknown<UsersServiceUnknown> _, LazySvc<IContextOfSite> context) : base(context)
        { }

        public override string PlatformIdentityTokenPrefix => $"{Eav.Constants.NullNameId}:";

        public override IUser PlatformUserInformationDto(int userId) => new CmsUserRaw();
    }
}
