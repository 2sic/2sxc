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
        /// In DNN this is the same as the PortalId
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The site url without protocol. Can be variation of any such examples:
        /// 
        /// - website.org
        /// - www.website.org
        /// - website.org/products
        /// - website.org/en-us
        /// - website.org/products/en-us
        /// </summary>
        string Url { get; }
    }
}
