namespace ToSic.Sxc.Context;

/// <summary>
/// The types of platforms which 2sxc could be running on
/// </summary>
[PublicApi]
[Flags]
public enum PlatformType : long
{
    /// <summary>
    /// Unknown platform - this should never occur
    /// </summary>
    Unknown = 0,
        
    /// <summary>
    /// No platform - this should never occur
    /// </summary>
    None = 1 << 0,
        
    /// <summary>
    /// All platforms / hybrid. This should never be used to publish what a platform is, but to mark things that work on all platforms
    /// </summary>
    Hybrid = 1 << 1,
        
    /// <summary>
    /// Dnn aka. DotNetNuke - see https://dnncommunity.org/
    /// </summary>
    Dnn = 1 << 2,

    /// <summary>
    /// Oqtane using .net Core 5 - see https://oqtane.org/
    /// </summary>
    Oqtane = 1 << 3,

    /// <summary>
    /// NopCommerce using .net Core 5 (not implemented yet) - see https://www.nopcommerce.com/
    /// </summary>
    NopCommerce = 1 << 4,

    /// <summary>
    /// Custom platform - this should never occur in production code but could during automated testing
    /// </summary>
    Custom = 1L << 56,

    /// <summary>
    /// Test platform - this should never occur in production code but could during automated testing
    /// </summary>
    Test = 1L << 57,
}