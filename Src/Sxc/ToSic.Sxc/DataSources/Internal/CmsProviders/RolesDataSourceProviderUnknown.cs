using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.DataSources.Internal;

internal class RolesDataSourceProviderUnknown(WarnUseOfUnknown<RolesDataSourceProviderUnknown> _) : RolesDataSourceProvider($"{SxcLogName}.{LogConstants.NameUnknown}")
{
    public override IEnumerable<RoleDataRaw> GetRolesInternal(
    ) => new List<RoleDataRaw>();
}