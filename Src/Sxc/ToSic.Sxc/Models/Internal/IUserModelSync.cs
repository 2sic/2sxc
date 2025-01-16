using ToSic.Eav.Context;

namespace ToSic.Sxc.Models.Internal;

public interface IUserModelSync
{
    /// <inheritdoc cref="IUser.Id"/>>
    public int Id { get; }

    /// <inheritdoc cref="IUser.Guid"/>
    public Guid Guid { get; }

    /// <summary>
    /// When the user was first created.
    /// </summary>
    public DateTime Created { get; }

    /// <summary>
    /// When the user was last modified.
    /// </summary>
    public DateTime Modified { get; }

    public string NameId { get; }

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
    public bool IsSystemAdmin { get; }

    /// <summary>
    /// True if the user is admin - allowing full content-management and user-management.
    /// </summary>
    public bool IsSiteAdmin { get; }

    /// <summary>
    /// True if the user is a content admin / editor.
    /// If the user only has this role, then he/she can only edit pages and content, but not users. 
    /// </summary>
    public bool IsContentAdmin { get; }

    /// <summary>
    /// Determines if the user is a content editor.
    /// In DNN 10, ContentEditors cannot publish pages (unless they are also ContentAdmins).
    /// </summary>
    public bool IsContentEditor { get; }

    /// <summary>
    /// True if a user is in the SxcDesigners group.
    /// Such a person can actually do a lot more, like access the advanced toolbars. 
    /// </summary>
    public bool IsSiteDeveloper { get; }

    /// <summary>
    /// True if the user is anonymous / not logged in. 
    /// </summary>
    public bool IsAnonymous { get; }

    /// <inheritdoc cref="IUser.Username"/>
    public string Username { get; }

    /// <inheritdoc cref="IUser.Email"/>
    public string Email { get; }

    /// <inheritdoc cref="IUser.Name"/>
    public string Name { get; }

}