using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{
    public static readonly Feature LightSpeedOutputCache = new()
    {
        NameId = "LightSpeedOutputCache",
        Guid = new("61654bca-b76b-4c15-9173-5643de6b4baa"),
        Name = "LightSpeed Output Cache",
        IsPublic = false,
        Ui = false,
        Description = "High-Performance OutputCache",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    };

    public static readonly Feature LightSpeedOutputCacheAppFileChanges = new()
    {
        NameId = "LightSpeedOutputCacheAppFileChanges",
        Guid = new("3f4c7d29-568d-44de-bd76-77e8572560f7"),
        Name = "LightSpeed Output Cache - Monitor App file changes",
        IsPublic = false,
        Ui = false,
        Description = "High-Performance OutputCache - Watch App files and flush cache if any App files change",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    };
}