using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// The site context of the code - so basically which website / portal it's running on. 
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.Site`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyContext.Site`
/// </summary>
[PublicApi]
public interface ICmsSite: IHasMetadata
{
    /// <summary>
    /// The Id of the site in systems like DNN and Oqtane.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Site.Id`  
    /// 🪒 Use in Typed Razor: `MyContext.Site.Name`
    /// </summary>
    /// <remarks>
    /// In DNN this is the same as the `PortalId`
    /// </remarks>
    int Id { get; }

    /// <summary>
    /// The site url with protocol. Can be variation of any such examples:
    /// 
    /// - https://website.org
    /// - https://www.website.org
    /// - https://website.org/products
    /// - https://website.org/en-us
    /// - https://website.org/products/en-us
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Site.Url`  
    /// 🪒 Use in Typed Razor: `MyContext.Site.Url`
    /// </summary>
    string Url { get; }

    /// <summary>
    /// The url root which identifies the current site / portal as is. It does not contain a protocol, but can contain subfolders.
    /// This is mainly used to clearly identify a site in a multi-site system or a language-variation in a multi-language setup.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Site.UrlRoot`  
    /// 🪒 Use in Typed Razor: `MyContext.Site.UrlRoot`
    /// </summary>
    /// <remarks>
    /// introduced in 2sxc 13
    /// </remarks>
    string UrlRoot { get; }

    /// <summary>
    /// Metadata of the current site
    /// </summary>
    /// <remarks>
    /// Added in v13.12
    /// </remarks>
    [JsonIgnore] // prevent serialization as it's not a normal property
    new IMetadata Metadata { get; }

    //[PrivateApi]
    //ICmsSite Init(CmsContext parent, IAppStateInternal appState);

    // 2023-08-24 2dm hide for now, not sure if we want to publish like this, or just provide appIdentity to get it yourself
    //[PrivateApi("WIP v13/14")]
    //IApp App { get; }

}