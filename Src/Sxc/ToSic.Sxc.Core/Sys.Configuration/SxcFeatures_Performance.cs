using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Sys.Configuration;

public partial class SxcFeatures
{

    public static readonly Feature SmartDataCache = new()
    {
        NameId = nameof(SmartDataCache),
        Guid = new("8e4f0ea6-6574-4341-89ac-21629584dc1d"),
        Name = "Smart Data Cache",
        IsPublic = false,
        Ui = false,
        Description = "High-Performance Smart Data Cache",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = BuiltInFeatures.ForPatronPerformanceAutoEnabled,
    };

    public static readonly Feature RazorCacheCompiledToDisk = new()
    {
        NameId = nameof(RazorCacheCompiledToDisk),
        Guid = new("d022bf2e-0e0c-4c61-b653-c2be0213e323"),
        Name = "Razor - Cache Compiled Razor to Disk",
        IsPublic = false,
        Ui = false,
        Description = "Cache compiled razor for faster restart and code less accessed.",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = BuiltInFeatures.ForPatronPerformanceNotEnabled,
    };
}