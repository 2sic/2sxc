using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal.Raw;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Services.Internal;

internal class UsersServiceProviderUnknown(WarnUseOfUnknown<UsersServiceProviderUnknown> _) : UserSourceProvider($"{SxcLogName}.{LogConstants.NameUnknown}"), IIsUnknown
{
    public override string PlatformIdentityTokenPrefix => $"{Eav.Constants.NullNameId}:";

    internal override ICmsUser PlatformUserInformationDto(int userId, int siteId) => new CmsUserRaw();
}