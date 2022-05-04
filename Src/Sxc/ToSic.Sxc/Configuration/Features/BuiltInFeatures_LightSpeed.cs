using System;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static readonly FeatureDefinition LightSpeedOutputCache = new FeatureDefinition(
            "LightSpeedOutputCache",
            new Guid("61654bca-b76b-4c15-9173-5643de6b4baa"),
            "LightSpeed Output Cache (BETA)",
            false,
            false,
            "High-Performance OutputCache",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Configuration.BuiltInFeatures.ForPatronsPerfectionist
        );

        public static readonly FeatureDefinition LightSpeedOutputCacheAppFileChanges = new FeatureDefinition(
            "LightSpeedOutputCacheAppFileChanges",
            new Guid("3f4c7d29-568d-44de-bd76-77e8572560f7"),
            "LightSpeed Output Cache - Monitor App file changes",
            false,
            false,
            "High-Performance OutputCache - Watch App files and flush cache if any App files change",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Configuration.BuiltInFeatures.ForPatronsPerfectionist
        );
    }
}
