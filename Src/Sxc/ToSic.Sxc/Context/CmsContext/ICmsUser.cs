using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// The user context of your code - so it's information about the user your code is using. 
/// </summary>
/// <example>
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.User`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyUser`
/// </example>
/// <remarks>
/// History
/// 
/// * v18 enhanced to serialize - so it can be returned by a WebApi Controller
/// * v20 inheriting from <see cref="IUserModel"/> now, so most properties are from there
/// </remarks>
[PublicApi]
public interface ICmsUser: IUserModel, IHasMetadata
{
    /// <summary>
    /// The user e-mail.
    /// If anonymous/not logged in, would be an empty string.
    /// </summary>
    /// 
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.Email`  
    /// 🪒 Use in Typed Razor: `MyUser.Email`
    /// </example>
    /// 
    /// <remarks>
    ///
    /// History: Added in v.14.09
    /// </remarks>
    string Email { get; }

    /// <summary>
    /// User Id as int. Works in DNN and Oqtane.
    /// </summary>
    /// 
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.UrlRoot`  
    /// 🪒 Use in Typed Razor: `MyUser.Id`
    /// </example>
    /// 
    /// <returns>The ID or 0 (zero) if anonymous</returns>
    int Id { get; }

    /// <summary>
    /// Information if the user is anonymous (not logged in)
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsAnonymous`  
    /// 🪒 Use in Typed Razor: `MyUser.IsAnonymous`
    /// </example>
    /// <remarks>
    /// History: This was added fairly late in v14.08
    /// </remarks>
    bool IsAnonymous { get; }

    /// <summary>
    /// Information if the current user is Site Administrator.
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsSiteAdmin`  
    /// 🪒 Use in Typed Razor: `MyUser.IsSiteAdmin`
    /// </example>
    /// <remarks>
    /// Basically this means a user has very high permissions - incl. the ability
    /// to create users in a site etc.
    /// But these are not the highest possible privileges
    /// 
    /// * For the site it would be IsSiteDeveloper
    /// * For the entire system that would be IsSystemAdmin.
    /// 
    /// History: New in 2sxc 12
    /// </remarks>
    bool IsSiteAdmin { get; }


    /// <summary>
    /// Information if the current user is Site Content Administrator.
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsContentAdmin`  
    /// 🪒 Use in Typed Razor: `MyUser.IsContentAdmin`
    /// </example>
    /// <remarks>
    /// Basically this means a user has Admin permissions, but may not have all admin permissions if excluded through special 2sxc-groups.
    /// These are not the highest possible privileges
    /// 
    /// * For the site it would be IsSiteDeveloper
    /// * For the entire system that would be IsSystemAdmin.
    /// 
    /// History: New in 2sxc 14.09
    /// </remarks>
    bool IsContentAdmin { get; }

    /// <summary>
    /// Information if the current user is System Administrator.
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsSystemAdmin`  
    /// 🪒 Use in Typed Razor: `MyUser.IsSystemAdmin`
    /// </example>
    /// <remarks>
    /// Basically this means a user has maximum permissions - incl. the ability
    /// to install additional components or do dangerous things like edit razor.
    /// 
    /// History: New in 2sxc 12
    /// </remarks>
    bool IsSystemAdmin { get; }


    /// <summary>
    /// Information if the current user is Developer on the current site.
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsSiteDeveloper`  
    /// 🪒 Use in Typed Razor: `MyUser.IsSiteDeveloper`
    /// </example>
    /// <remarks>
    /// Basically this means a user has maximum site permissions - incl. the ability
    /// to install additional components or do dangerous things like edit razor.
    /// These are almost the highest possible privileges
    /// 
    /// * For the entire system that would be IsSystemAdmin.
    /// 
    /// History: New in 2sxc 12
    /// </remarks>
    bool IsSiteDeveloper { get; }

    /// <summary>
    /// Metadata of the current user
    /// </summary>
    /// <remarks>
    /// History: Added in v13.12
    /// </remarks>
    [JsonIgnore] // prevent serialization as it's not a normal property
    new IMetadata Metadata { get; }

    /// <summary>
    /// The username as should be displayed. 
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.Name`  
    /// 🪒 Use in Typed Razor: `MyUser.Name`
    /// </example>
    /// <remarks>
    /// History: Added in v.14.09
    /// </remarks>
    /// <returns>The name or an empty string if anonymous</returns>
    string Name { get; }


    /// <summary>
    /// The username used on the login.
    /// </summary>
    /// <example>
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.Username`  
    /// 🪒 Use in Typed Razor: `MyUser.Username`
    /// </example>
    /// <remarks>
    /// History: Added in v.14.09
    /// </remarks>
    /// <returns>The username or an empty string if anonymous</returns>
    string Username { get; }
}