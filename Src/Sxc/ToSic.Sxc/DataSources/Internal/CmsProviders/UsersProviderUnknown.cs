using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.Models;
using ToSic.Sxc.Models.Internal;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.DataSources.Internal;

internal class UsersProviderUnknown(WarnUseOfUnknown<UsersProviderUnknown> _) : ServiceBase($"{SxcLogName}.{LogConstants.NameUnknown}"), IUsersProvider
{
    public string PlatformIdentityTokenPrefix => $"{Eav.Constants.NullNameId}:";

    public IUserModel GetUser(int userId, int siteId) => new CmsUserRaw();

    public IEnumerable<UserModel> GetUsers(UsersGetSpecs specs) => new List<UserModel>();
}