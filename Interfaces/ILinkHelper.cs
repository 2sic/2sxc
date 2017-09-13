namespace ToSic.SexyContent.Interfaces
{
    public interface ILinkHelper
    {
        /// <summary>
        /// returns a link to the current page with parameters resolved in a way that DNN wants it
        /// </summary>
        /// <param name="requiresNamedParameters">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&name=42"</param>
        /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
        /// <param name="parameters">the parameters either as "/id/47/name/daniel" or "id=47&name=daniel"</param>
        /// <returns></returns>
        string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null);

        /// <summary>
        /// A base url for the current page, for use in html-base tags
        /// </summary>
        /// <returns></returns>
        string Base();
    }
}
