using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.Data;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Cms.Users;

/// <summary>
/// BETA User Model for data returned by the <see cref="Users"/> DataSource or other sources.
/// </summary>
/// <remarks>
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn UserInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Users.UserInfo.html)
/// * [Oqtane User](https://docs.oqtane.org/api/Oqtane.Models.User.html)
///
/// History
/// 
/// * Introduced in v19.01
/// </remarks>
[ModelSpecs(Use = typeof(UserModelOfEntity))]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IUserModel : IDataWrapper
{
    /// <inheritdoc cref="IUser.Email"/>
    string? Email { get; }

    /// <inheritdoc cref="IUser.Id"/>>
    int Id { get; }

    /// <inheritdoc cref="IUser.Guid"/>
    Guid Guid { get; }

    /// <summary>
    /// When the user was first created.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// When the user was last modified.
    /// </summary>
    DateTime Modified { get; }

    /// <summary>
    /// True if the user is anonymous / not logged in. 
    /// </summary>
    bool IsAnonymous { get; }

    /// <summary>
    /// True if the user is admin - allowing full content-management and user-management.
    /// </summary>
    bool IsSiteAdmin { get; }

    /// <summary>
    /// True if the user is a content admin / editor.
    /// If the user only has this role, then he/she can only edit pages and content, but not users. 
    /// </summary>
    bool IsContentAdmin { get; }

    /// <summary>
    /// Determines if the user is a content editor.
    /// In DNN 10, ContentEditors cannot publish pages (unless they are also ContentAdmins).
    /// </summary>
    bool IsContentEditor { get; }

    // TODO:
    string? NameId { get; }

    /// <summary>
    /// True if the user has super-user rights.
    /// This kind of user can do everything, incl. create apps. 
    /// </summary>
    bool IsSystemAdmin { get; }

    /// <summary>
    /// True if a user is in the SxcDesigners group.
    /// Such a person can actually do a lot more, like access the advanced toolbars. 
    /// </summary>
    bool IsSiteDeveloper { get; }

    /// <inheritdoc cref="IUser.Name"/>
    string? Name { get; }

    /// <inheritdoc cref="IUser.Username"/>
    string? Username { get; }

    /// <summary>
    /// Roles of the user.
    /// </summary>
    IEnumerable<IUserRoleModel> Roles { get; }
}