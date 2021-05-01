using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// The types of platforms which 2sxc could be running on
    /// </summary>
    [PublicApi]
    public enum PlatformType
    {
        /// <summary>
        /// Unknown platform - this should never occur
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// No platform - this should never occur
        /// </summary>
        None = 1,
        
        /// <summary>
        /// Not one of the here defined platforms - this should never occur
        /// </summary>
        Other = 2,
        
        /// <summary>
        /// Test platform - this should never occur in production code but could during automated testing
        /// </summary>
        Testing = 3,

        /// <summary>
        /// Dnn aka. DotNetNuke - see https://dnncommunity.org/
        /// </summary>
        Dnn = 11,

        /// <summary>
        /// Oqtane using .net Core 5 - see https://oqtane.org/
        /// </summary>
        Oqtane = 12,

        /// <summary>
        /// NopCommerce using .net Core 5 (not implemented yet) - see https://www.nopcommerce.com/
        /// </summary>
        NopCommerce = 21,
    }
}
