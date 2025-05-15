using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// Information about the page which is the context for the currently running code.
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.Page`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyPage`
/// </summary>
/// <remarks>
/// Note that the module context is the module for which the code is currently running.
/// In some scenarios (like Web-API scenarios) the code is running _for_ this page but _not on_ this page,
/// as it would then be running on a WebApi.
/// </remarks>
[PublicApi]
public interface ICmsPage: IHasMetadata
{
    /// <summary>
    /// The Id of the page.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Page.Id`  
    /// 🪒 Use in Typed Razor: `MyPage.Id`
    /// </summary>
    /// <remarks>
    /// Corresponds to the Dnn `TabId` or the Oqtane `Page.PageId`
    /// </remarks>
    int Id { get; }

    /// <summary>
    /// The page parameters, cross-platform.
    /// Use this for easy access to url parameters like ?id=xyz
    /// with `CmsContext.Page.Parameters["id"]` as a replacement for `Request.QueryString["id"]`
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Page.Parameters`  
    /// 🪒 Use in Typed Razor: `MyPage.Parameters`
    /// </summary>
    IParameters Parameters { get; }

    /// <summary>
    /// The resource specific Url, like the one to this page or portal.
    ///
    /// 🪒 Use in Dynamic Razor: `CmsContext.Page.Url`  
    /// 🪒 Use in Typed Razor: `MyPage.Url`
    /// </summary>
    /// <remarks>
    /// Added ca. v12.
    /// </remarks>
    string Url { get; }
    // ^^^ note: property added ca. v12 but was not visible in docs till 16.04

    /// <summary>
    /// Metadata of the current page
    /// </summary>
    /// <remarks>
    /// Added in v13.12
    /// </remarks>
    [JsonIgnore] // prevent serialization as it's not a normal property
    new IMetadata Metadata { get; }
}