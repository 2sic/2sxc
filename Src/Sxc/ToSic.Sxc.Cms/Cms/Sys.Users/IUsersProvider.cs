namespace ToSic.Sxc.Cms.Users.Internal;

/// <summary>
/// Provider user data from platform.
///
/// Must be overriden in each platform.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IUsersProvider
{
    public string PlatformIdentityTokenPrefix { get; }

    /// <summary>
    /// Get user information from platform
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="siteId"></param>
    /// <returns></returns>
    public IUserModel? GetUser(int userId, int siteId);

    /// <summary>
    /// The inner list retrieving the users.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public IEnumerable<UserModel> GetUsers(UsersGetSpecs specs);
}