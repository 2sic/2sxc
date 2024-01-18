using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.DataSources.Internal;

internal class UsersDataSourceProviderUnknown : UsersDataSourceProvider
{
    public UsersDataSourceProviderUnknown(WarnUseOfUnknown<UsersDataSourceProviderUnknown> _): base($"{SxcLogging.SxcLogName}.{LogConstants.NameUnknown}")
    {
    }
        
    public override IEnumerable<CmsUserRaw> GetUsersInternal() => new List<CmsUserRaw>();
}