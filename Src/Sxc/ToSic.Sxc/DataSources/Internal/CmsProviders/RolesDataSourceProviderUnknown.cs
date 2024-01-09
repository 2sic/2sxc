using System.Collections.Generic;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.DataSources;

internal class RolesDataSourceProviderUnknown : RolesDataSourceProvider
{
    public RolesDataSourceProviderUnknown(WarnUseOfUnknown<RolesDataSourceProviderUnknown> _): base($"{SxcLogging.SxcLogName}.{LogConstants.NameUnknown}")
    {
    }
        
    public override IEnumerable<RoleDataRaw> GetRolesInternal(
    ) => new List<RoleDataRaw>();
}