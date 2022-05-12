using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// The site context of the code - so basically which website / portal it's running on. 
    /// </summary>
    [PublicApi]
    public interface ICmsSite: IHasMetadata
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
        /// <remarks>
        /// introduced in 2sxc 13
        /// </remarks>
        string UrlRoot { get; }

        [PrivateApi("WIP")]
#pragma warning disable CS0108, CS0114
        IDynamicMetadata Metadata { get; }
#pragma warning restore CS0108, CS0114

    }
}
