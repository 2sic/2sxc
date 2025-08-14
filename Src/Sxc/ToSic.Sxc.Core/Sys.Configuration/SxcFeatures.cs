using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Sys.Configuration;

/// <summary>
/// Internal - built-in features.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
[PrivateApi]
public partial class SxcFeatures
{
    public static void Register(FeaturesCatalog cat) =>
        cat.Register(
            // CorePlus
            RazorThrowPartial,
            RenderThrowPartialSystemAdmin,
            PermissionPrioritizeModuleContext,

            // Sentinel CSP
            ContentSecurityPolicy,
            ContentSecurityPolicyTestUrl,
            ContentSecurityPolicyEnforceTemp,

            // Sentinel New 15.04
            CdnSourcePublic,
            CdnSourceEdit,
            // Not yet available
            //CdnSourceAdmin,
            //CdnSourceDev,

            // Sentinel new v18.05
            NetworkDataEncryption,

            // Patrons Perfectionist
            ImageServiceMultiFormat, // v13
            ImageServiceMultipleSizes,
            ImageServiceSetSizes,
            ImageServiceUseFactors,

            LightSpeedOutputCache,
            LightSpeedOutputCacheAppFileChanges,
            LightSpeedOutputCachePartials, // v20
            SmartDataCache      // v19.01
        );
}