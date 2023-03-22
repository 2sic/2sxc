using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static void Register(FeaturesCatalog cat) =>
            cat.Register(
                RazorThrowPartial,
                RenderThrowPartialSystemAdmin,
                ContentSecurityPolicy,
                ContentSecurityPolicyTestUrl,
                ContentSecurityPolicyEnforceTemp,

                // New 15.04
                CdnSourcePublic,
                CdnSourceEdit,
                // Not yet available
                //CdnSourceAdmin,
                //CdnSourceDev,

                // Patrons Perfectionist
                ImageServiceMultiFormat, // v13
                ImageServiceMultipleSizes,
                ImageServiceSetSizes,
                ImageServiceUseFactors,

                LightSpeedOutputCache,
                LightSpeedOutputCacheAppFileChanges
            );


    }
}
