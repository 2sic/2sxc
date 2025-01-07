using ToSic.Eav.Context;

namespace ToSic.Sxc.Models.Internal;

public interface IUserModel
{
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

    string NameId { get; }

    ///// <summary>
    ///// Role ID List.
    ///// Important: Internally we use a list to do checks etc.
    ///// But for creating the entity we return a CSV
    ///// </summary>
    //List<int> Roles { get; }

    /// <summary>
    /// True if the user has super-user rights.
    /// This kind of user can do everything, incl. create apps. 
    /// </summary>
    bool IsSystemAdmin { get; }

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

    /// <summary>
    /// True if a user is in the SxcDesigners group.
    /// Such a person can actually do a lot more, like access the advanced toolbars. 
    /// </summary>
    bool IsSiteDeveloper { get; }

    /// <summary>
    /// True if the user is anonymous / not logged in. 
    /// </summary>
    bool IsAnonymous { get; }

    /// <inheritdoc cref="IUser.Username"/>
    string Username { get; }

    /// <inheritdoc cref="IUser.Email"/>
    string Email { get; }

    /// <inheritdoc cref="IUser.Name"/>
    string Name { get; }

}