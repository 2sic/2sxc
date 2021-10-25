using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// The site context of the code - so basically which website / portal it's running on. 
    /// </summary>
    [PublicApi]
    public interface ICmsSite
    {
        /// <summary>
        /// The Id of the site in systems like DNN and Oqtane.
        /// 
        /// 🪒 Use in Razor: `CmsContext.Site.Id`
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
        /// 🪒 Use in Razor: `CmsContext.Site.Url`
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The url root which identifies the current site / portal as is. It does not contain a protocol, but can contain subfolders.
        /// 
        /// This is mainly used to clearly identify a site in a multi-site system or a language-variation in a multi-language setup.
        /// </summary>
        [PrivateApi("WIP 2dm - trying to get a better replacement for url - not public yet, and NAME NOT FINAL!")]
        string UrlRoot { get; }
    }
}
