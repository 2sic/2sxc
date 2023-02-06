using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class UsersDataSourceProviderUnknown : UsersDataSourceProvider
    {
        public UsersDataSourceProviderUnknown(WarnUseOfUnknown<UsersDataSourceProviderUnknown> _): base($"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        {
        }
        
        public override IEnumerable<CmsUserInfo> GetUsersInternal(
        ) => new List<CmsUserInfo>();
    }
}
