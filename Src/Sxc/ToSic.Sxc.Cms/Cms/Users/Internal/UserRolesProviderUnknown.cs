using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Users.Internal;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.DataSources.Internal;

internal class UserRolesProviderUnknown(WarnUseOfUnknown<UserRolesProviderUnknown> _) : ServiceBase($"{SxcLogName}.{LogConstants.NameUnknown}"), IUserRolesProvider
{
    public IEnumerable<UserRoleModel> GetRoles()
        => [];
}