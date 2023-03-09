using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;

namespace ToSic.Sxc.DataSources
{
    public class UsersDataSourceProviderUnknown : UsersDataSourceProvider
    {
        public UsersDataSourceProviderUnknown(WarnUseOfUnknown<UsersDataSourceProviderUnknown> _): base($"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        {
        }
        
        public override IEnumerable<CmsUserRaw> GetUsersInternal() => new List<CmsUserRaw>();
    }
}
