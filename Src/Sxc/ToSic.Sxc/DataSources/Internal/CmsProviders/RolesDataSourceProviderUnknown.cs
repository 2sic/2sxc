using ToSic.Eav.Internal.Unknown;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.DataSources.Internal;

internal class RolesDataSourceProviderUnknown(WarnUseOfUnknown<RolesDataSourceProviderUnknown> _) : RolesDataSourceProvider($"{SxcLogName}.{LogConstants.NameUnknown}")
{
    public override IEnumerable<RoleDataRaw> GetRolesInternal(
    ) => new List<RoleDataRaw>();
}