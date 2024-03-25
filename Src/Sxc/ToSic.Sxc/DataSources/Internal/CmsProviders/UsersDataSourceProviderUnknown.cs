using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Context.Internal.Raw;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.DataSources.Internal;

internal class UsersDataSourceProviderUnknown(WarnUseOfUnknown<UsersDataSourceProviderUnknown> _) : UsersDataSourceProvider($"{SxcLogName}.{LogConstants.NameUnknown}")
{
    public override IEnumerable<CmsUserRaw> GetUsersInternal() => new List<CmsUserRaw>();
}