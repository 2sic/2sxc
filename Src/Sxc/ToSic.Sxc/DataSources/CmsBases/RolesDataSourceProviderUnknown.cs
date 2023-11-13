using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    internal class RolesDataSourceProviderUnknown : RolesDataSourceProvider
    {
        public RolesDataSourceProviderUnknown(WarnUseOfUnknown<RolesDataSourceProviderUnknown> _): base($"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        {
        }
        
        public override IEnumerable<RoleDataRaw> GetRolesInternal(
        ) => new List<RoleDataRaw>();
    }
}
