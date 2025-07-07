using ToSic.Sxc.DataSources;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Cms.Users.Sys;

internal class UserRolesProviderUnknown(WarnUseOfUnknown<UserRolesProviderUnknown> _)
    : ServiceBase($"{SxcLogName}.{LogConstants.NameUnknown}"),
        IUserRolesProvider
{
    public IEnumerable<UserRoleModel> GetRoles()
        => [];
}