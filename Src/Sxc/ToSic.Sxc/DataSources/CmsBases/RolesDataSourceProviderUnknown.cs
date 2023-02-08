using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class RolesDataSourceProviderUnknown : RolesDataSourceProvider
    {
        public RolesDataSourceProviderUnknown(WarnUseOfUnknown<RolesDataSourceProviderUnknown> _): base($"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        {
        }
        
        public override IEnumerable<CmsRoleInfo> GetRolesInternal(
        ) => new List<CmsRoleInfo>();
    }
}
