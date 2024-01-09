using ToSic.Eav.Context;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.DI;
using ToSic.Eav.Run;
using ToSic.Sxc.Context.Internal.Raw;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class UsersServiceUnknown : UsersServiceBase, IIsUnknown
{
    public UsersServiceUnknown(WarnUseOfUnknown<UsersServiceUnknown> _, LazySvc<IContextOfSite> context) : base(context)
    { }

    public override string PlatformIdentityTokenPrefix => $"{Eav.Constants.NullNameId}:";

    public override IUser PlatformUserInformationDto(int userId) => new CmsUserRaw();
}