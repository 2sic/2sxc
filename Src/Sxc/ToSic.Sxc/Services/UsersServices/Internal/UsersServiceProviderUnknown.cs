using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Services.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class UsersServiceProviderUnknown : UserSourceProvider, IIsUnknown
{
    public UsersServiceProviderUnknown(WarnUseOfUnknown<UsersServiceProviderUnknown> _) : base($"{SxcLogging.SxcLogName}.{LogConstants.NameUnknown}")
    { }

    public override string PlatformIdentityTokenPrefix => $"{Eav.Constants.NullNameId}:";

    internal override ICmsUser PlatformUserInformationDto(int userId, int siteId) => new CmsUserRaw();
}