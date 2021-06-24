using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Helpers to create links with parameters or base-tag links (important for SPAs)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface ILinkHelper
    {
        [PrivateApi("Internal")]
        void Init(IContextOfBlock context, IApp app);
        
        /// <summary>
        /// returns a link to the current page with parameters resolved in a way that DNN wants it
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
        /// <param name="parameters">the parameters either as "/id/47/name/daniel" or "id=47&amp;name=daniel"</param>
        /// <param name="api">optional api url "api/my?id=something"</param>
        /// <returns></returns>
        string To(
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            int? pageId = null,
            string parameters = null,
            string api = null
        );
        
        /// <summary>
        /// A base url for the current page, for use in html-base tags
        /// </summary>
        /// <returns></returns>
        string Base();


        [PrivateApi]
        string Img(
            string url = null,
            object settings = null,
            object factor = null,
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string mode = null,
            string scale = null,
            string format = null,
            object maxWidth = null,
            object maxHeight = null,
            object aspectRatio = null);

    }
}
