using System.Collections.Generic;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.DataSources;

internal class UsersDataSourceProviderUnknown : UsersDataSourceProvider
{
    public UsersDataSourceProviderUnknown(WarnUseOfUnknown<UsersDataSourceProviderUnknown> _): base($"{SxcLogging.SxcLogName}.{LogConstants.NameUnknown}")
    {
    }
        
    public override IEnumerable<CmsUserRaw> GetUsersInternal() => new List<CmsUserRaw>();
}