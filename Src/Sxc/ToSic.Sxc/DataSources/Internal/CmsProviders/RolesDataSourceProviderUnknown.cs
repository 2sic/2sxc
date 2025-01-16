using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Models.Internal;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.DataSources.Internal;

internal class RolesDataSourceProviderUnknown(WarnUseOfUnknown<RolesDataSourceProviderUnknown> _) : RolesDataSourceProvider($"{SxcLogName}.{LogConstants.NameUnknown}")
{
    public override IEnumerable<UserRoleModel> GetRolesInternal(
    ) => new List<UserRoleModel>();
}