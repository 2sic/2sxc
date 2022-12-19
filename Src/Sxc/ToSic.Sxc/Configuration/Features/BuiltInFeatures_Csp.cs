using System;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static readonly FeatureDefinition ContentSecurityPolicy = new FeatureDefinition(
            "ContentSecurityPolicy",
            new Guid("e5c99abf-9bc4-4e6c-9cc4-4bda22c0ab85"),
            "Enable Content Security Policy and related APIs. ",
            false,
            false,
            "If enabled, ContentSecurityPolicy headers will be set. Note that APIs will always work, but not result in http headers if this is disabled. ",
            FeaturesCatalogRules.Security0Improved,
            Eav.Configuration.BuiltInFeatures.ForPatronsSentinel
        );
        
        public static readonly FeatureDefinition ContentSecurityPolicyTestUrl = new FeatureDefinition(
            "ContentSecurityPolicyTestUrl",
            new Guid("94c3d7e5-5d89-4a68-b710-95bfc29199ba"),
            "Enable the parameter ?csp=true in URL for testing.",
            false,
            false,
            "If enabled, you can add csp=true to any url to temporarily enable a policy and see if it works. Requires the CSP system to be enabled.",
            FeaturesCatalogRules.Security0Improved,
            Eav.Configuration.BuiltInFeatures.ForPatronsSentinel
        );
        public static readonly FeatureDefinition ContentSecurityPolicyEnforceTemp = new FeatureDefinition(
            "ContentSecurityPolicyEnforceTemp",
            new Guid("ece0943d-f3c0-422c-9d06-c20b9b83df8d"),
            "Enable CSP on all pages (temporary setting).",
            false,
            false,
            "Enable CSP on all pages. This is a temporary setting, till we have more configuration in normal Settings.",
            FeaturesCatalogRules.Security0Improved,
            Eav.Configuration.BuiltInFeatures.ForPatronsSentinel
        );
    }
}
