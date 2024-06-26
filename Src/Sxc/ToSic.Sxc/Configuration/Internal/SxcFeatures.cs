﻿using ToSic.Eav.Internal.Features;

namespace ToSic.Sxc.Configuration.Internal;

/// <summary>
/// Internal - built-in features.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[PrivateApi]
public partial class SxcFeatures
{
    public static void Register(FeaturesCatalog cat) =>
        cat.Register(
            // CorePlus
            RazorThrowPartial,
            RenderThrowPartialSystemAdmin,
            PermissionPrioritizeModuleContext,

            // CSP
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