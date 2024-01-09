using System;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{
    public static readonly Feature ContentSecurityPolicy = new(
        "ContentSecurityPolicy",
        new Guid("e5c99abf-9bc4-4e6c-9cc4-4bda22c0ab85"),
        "Enable Content Security Policy and related APIs. ",
        false,
        false,
        "If enabled, ContentSecurityPolicy headers will be set. Note that APIs will always work, but not result in http headers if this is disabled. ",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelDisabled
    );
        
    public static readonly Feature ContentSecurityPolicyTestUrl = new(
        "ContentSecurityPolicyTestUrl",
        new Guid("94c3d7e5-5d89-4a68-b710-95bfc29199ba"),
        "Enable the parameter ?csp=true in URL for testing.",
        false,
        false,
        "If enabled, you can add csp=true to any url to temporarily enable a policy and see if it works. Requires the CSP system to be enabled.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelDisabled
    );
    public static readonly Feature ContentSecurityPolicyEnforceTemp = new(
        "ContentSecurityPolicyEnforceTemp",
        new Guid("ece0943d-f3c0-422c-9d06-c20b9b83df8d"),
        "Enable CSP on all pages (temporary setting).",
        false,
        false,
        "Enable CSP on all pages. This is a temporary setting, till we have more configuration in normal Settings.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelDisabled
    );

    public static readonly Feature CdnSourcePublic = new(
        nameof(CdnSourcePublic),
        new Guid("b8b993d3-a02b-4099-a2a8-c06bf8961a66"),
        "Change CDN source for public Web Resources",
        false,
        false,
        "Allow reconfigure of the CDN source for public Web Resources.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelEnabled
    );
    public static readonly Feature CdnSourceEdit = new(
        nameof(CdnSourceEdit),
        new Guid("34dce40e-30fc-4d4f-b1ab-8fcface90e61"),
        "Change CDN source for Edit Web Resources",
        false,
        false,
        "Allow reconfigure of the CDN source for Web Resources used in the Edit UI.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelEnabled
    );

    // Note: not in use ATM
    public static readonly Feature CdnSourceAdmin = new(
        nameof(CdnSourceAdmin),
        new Guid("c799c71e-aa4a-4a30-ae8a-e177e615a36c"),
        "Change CDN source for Admin Web Resources",
        false,
        false,
        "Allow reconfigure of the CDN source for Web Resources used in the Admin UI.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelEnabled
    );

    // Note: not in use ATM
    public static readonly Feature CdnSourceDev = new(
        nameof(CdnSourceDev),
        new Guid("81a51003-ad55-491e-9749-d74529496465"),
        "Change CDN source for Development Web Resources",
        false,
        false,
        "Allow reconfigure of the CDN source for Web Resources used in the Developers UIs.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsSentinelEnabled
    );


}