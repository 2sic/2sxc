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
        /// The site url without protocol. Can be variation of any such examples:
        /// 
        /// - website.org
        /// - www.website.org
        /// - website.org/products
        /// - website.org/en-us
        /// - website.org/products/en-us
        /// 
        /// 🪒 Use in Razor: `CmsContext.Site.Url`
        /// </summary>
        string Url { get; }

        [PrivateApi("WIP 2dm - trying to get a better replacement for url")]
        string UrlRoot { get; }
    }
}
