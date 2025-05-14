using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Services;

/// <summary>
/// Service on [`Kit.User`](xref:ToSic.Sxc.Services.ServiceKit16.User) to get users and roles of the platform.
/// </summary>
/// <remarks>
/// History: Released in 19.02 after being internal since 15.03.
/// </remarks>
[PublicApi]
public interface IUserService: INeedsCodeApiService
{
    /// <summary>
    /// Get current user.
    /// </summary>
    /// <returns>
    /// The current user or a default anonymous user if not logged in.
    /// </returns>
    /// <remarks>
    /// New v20
    /// </remarks>
    public IUserModel GetCurrentUser();


    /// <summary>
    /// Get a user by id.
    /// </summary>
    /// <param name="id">the user id</param>
    /// <returns>
    /// If found, a user model containing the user specs.
    /// If not found, a user model containing the unknown user specs.
    /// </returns>
    IUserModel GetUser(int id);

    /// <summary>
    /// Get a user by nameId.
    /// </summary>
    /// <param name="nameId">The nameID which is the identity token like `dnn:42`.</param>
    /// <returns>
    /// If found, a user model containing the user specs.
    /// If not found, a user model containing the unknown user specs.
    /// </returns>
    IUserModel GetUser(string nameId);

    /// <summary>
    /// Get all users.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IUserModel> GetUsers();

    /// <summary>
    /// Get all user roles.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IUserRoleModel> GetRoles();

}