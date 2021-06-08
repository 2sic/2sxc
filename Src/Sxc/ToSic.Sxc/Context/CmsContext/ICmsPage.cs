using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Information about the page which is the context for the currently running code.
    /// </summary>
    /// <remarks>
    /// Note that the module context is the module for which the code is currently running.
    /// In some scenarios (like Web-API scenarios) the code is running _for_ this page but _not on_ this page,
    /// as it would then be running on a WebApi.
    /// </remarks>
    [PublicApi]
    public interface ICmsPage
    {
        /// <summary>
        /// The Id of the page.
        /// 
        /// 🪒 Use in Razor: `CmsContext.Page.Type`
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
        /// 🪒 Use in Razor: `CmsContext.Page.Parameters["id"]`
        /// </summary>
        IReadOnlyDictionary<string, string> Parameters { get; }
    }
}
