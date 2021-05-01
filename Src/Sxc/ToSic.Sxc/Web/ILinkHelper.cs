using ToSic.Eav.Documentation;
using ToSic.Eav;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Helpers to create links with parameters or base-tag links (important for SPAs)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface ILinkHelper: SexyContent.Interfaces.ILinkHelper    // inherits from old namespace for compatibility
    {
        /// <summary>
        /// returns a link to the current page with parameters resolved in a way that DNN wants it
        /// </summary>
        /// <param name="requiresNamedParameters">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
        /// <param name="parameters">the parameters either as "/id/47/name/daniel" or "id=47&amp;name=daniel"</param>
        /// <returns></returns>
        new string To(
            string requiresNamedParameters = null,
            int? pageId = null,
            string parameters = null
        );

        /// <summary>
        /// A base url for the current page, for use in html-base tags
        /// </summary>
        /// <returns></returns>
        new string Base();


        /// <summary>
        /// Creates a link to an API endpoint.
        /// Useful for APIs which return downloads or similar. 
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="path">API path, like `api/vCard?id=27` or `app/api/my?id=27`</param>
        /// <returns></returns>
        string Api(
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string path = null
        );
    }
}
