using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    /// <summary>
    /// The types of platforms which 2sxc could be running on
    /// </summary>
    [PublicApi]
    public enum PlatformType
    {
        Unknown = 0,
        None = 1,
        Other = 2,
        Testing = 3,
        Dnn = 11,
        Oqtane = 12,
        //NopCommerce = 21,
    }
}
