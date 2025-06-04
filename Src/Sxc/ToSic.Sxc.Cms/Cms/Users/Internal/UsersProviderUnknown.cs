using ToSic.Eav.Sys;
using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Cms.Users.Internal;

internal class UsersProviderUnknown(WarnUseOfUnknown<UsersProviderUnknown> _) : ServiceBase($"{SxcLogName}.{LogConstants.NameUnknown}"), IUsersProvider
{
    public string PlatformIdentityTokenPrefix => $"{EavConstants.NullNameId}:";

    public IUserModel GetUser(int userId, int siteId) => new UserModel();

    public IEnumerable<UserModel> GetUsers(UsersGetSpecs specs) => new List<UserModel>();
}