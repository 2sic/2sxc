using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// The user context of your code - so it's information about the user your code is using. 
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.User`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyUser`
/// </summary>
/// <remarks>
/// * v18 enhanced to serialize - so it can be returned by a WebApi Controller
/// </remarks>
[PublicApi]
public interface ICmsUser: IHasMetadata
{
    /// <summary>
    /// The user e-mail.
    /// If anonymous/not logged in, would be an empty string.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.Email`  
    /// 🪒 Use in Typed Razor: `MyUser.Email`
    /// </summary>
    /// <remarks>Added in v.14.09</remarks>
    string Email { get; }

    /// <summary>
    /// User Id as int. Works in DNN and Oqtane.
    /// If anonymous is zero.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.UrlRoot`  
    /// 🪒 Use in Typed Razor: `MyUser.Id`
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Information if the user is anonymous (not logged in)
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsAnonymous`  
    /// 🪒 Use in Typed Razor: `MyUser.IsAnonymous`
    /// </summary>
    /// <remarks>
    /// This was added fairly late in v14.08
    /// </remarks>
    bool IsAnonymous { get; }

    /// <summary>
    /// Information if the current user is Site Administrator.
    /// Basically this means a user has very high permissions - incl. the ability
    /// to create users in a site etc.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsSiteAdmin`  
    /// 🪒 Use in Typed Razor: `MyUser.IsSiteAdmin`
    /// </summary>
    /// <remarks>
    /// These are not the highest possible privileges
    /// - For the site it would be IsSiteDeveloper
    /// - For the entire system that would be IsSystemAdmin.
    /// New in 2sxc 12
    /// </remarks>
    bool IsSiteAdmin { get; }


    /// <summary>
    /// Information if the current user is Site Content Administrator.
    /// Basically this means a user has Admin permissions, but may not have all admin permissions if excluded through special 2sxc-groups.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsContentAdmin`  
    /// 🪒 Use in Typed Razor: `MyUser.IsContentAdmin`
    /// </summary>
    /// <remarks>
    /// These are not the highest possible privileges
    /// - For the site it would be IsSiteDeveloper
    /// - For the entire system that would be IsSystemAdmin.
    /// New in 2sxc 14.09
    /// </remarks>
    bool IsContentAdmin { get; }

    /// <summary>
    /// Information if the current user is System Administrator.
    /// Basically this means a user has maximum permissions - incl. the ability
    /// to install additional components or do dangerous things like edit razor.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsSystemAdmin`  
    /// 🪒 Use in Typed Razor: `MyUser.IsSystemAdmin`
    /// </summary>
    /// <remarks>
    /// New in 2sxc 12
    /// </remarks>
    bool IsSystemAdmin { get; }


    /// <summary>
    /// Information if the current user is Developer on the current site.
    /// Basically this means a user has maximum site permissions - incl. the ability
    /// to install additional components or do dangerous things like edit razor.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.IsSiteDeveloper`  
    /// 🪒 Use in Typed Razor: `MyUser.IsSiteDeveloper`
    /// </summary>
    /// <remarks>
    /// These are not the highest possible privileges
    /// - For the entire system that would be IsSystemAdmin.
    /// New in 2sxc 12
    /// </remarks>
    bool IsSiteDeveloper { get; }

    /// <summary>
    /// Metadata of the current user
    /// </summary>
    /// <remarks>
    /// Added in v13.12
    /// </remarks>
    [JsonIgnore] // prevent serialization as it's not a normal property
    new IMetadata Metadata { get; }

    /// <summary>
    /// The user name as should be displayed. 
    /// If anonymous/not logged in, would be an empty string.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.Name`  
    /// 🪒 Use in Typed Razor: `MyUser.Name`
    /// </summary>
    /// <remarks>Added in v.14.09</remarks>
    string Name { get; }


    /// <summary>
    /// The user name used on the login.
    /// If anonymous/not logged in, would be an empty string.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.User.Username`  
    /// 🪒 Use in Typed Razor: `MyUser.Username`
    /// </summary>
    /// <remarks>Added in v.14.09</remarks>
    string Username { get; }
}