using ToSic.Eav.Documentation;

namespace ToSic.Sxc
{
    /// <summary>
    /// Helpers to create links with parameters or base-tag links (important for SPAs)
    /// </summary>
    [PublicApi]
    public interface ILinkHelper: SexyContent.Interfaces.ILinkHelper    // inherits from old namespace for compatibility
    {
        /// <summary>
        /// returns a link to the current page with parameters resolved in a way that DNN wants it
        /// </summary>
        /// <param name="requiresNamedParameters">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
        /// <param name="parameters">the parameters either as "/id/47/name/daniel" or "id=47&amp;name=daniel"</param>
        /// <returns></returns>
        new string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null);

        /// <summary>
        /// A base url for the current page, for use in html-base tags
        /// </summary>
        /// <returns></returns>
        new string Base();
    }
}
