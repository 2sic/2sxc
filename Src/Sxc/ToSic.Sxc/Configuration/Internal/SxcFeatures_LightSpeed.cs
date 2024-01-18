using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{
    public static readonly Feature LightSpeedOutputCache = new(
        "LightSpeedOutputCache",
        new("61654bca-b76b-4c15-9173-5643de6b4baa"),
        "LightSpeed Output Cache",
        false,
        false,
        "High-Performance OutputCache",
        FeaturesCatalogRules.Security0Neutral,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    );

    public static readonly Feature LightSpeedOutputCacheAppFileChanges = new(
        "LightSpeedOutputCacheAppFileChanges",
        new("3f4c7d29-568d-44de-bd76-77e8572560f7"),
        "LightSpeed Output Cache - Monitor App file changes",
        false,
        false,
        "High-Performance OutputCache - Watch App files and flush cache if any App files change",
        FeaturesCatalogRules.Security0Neutral,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    );
}