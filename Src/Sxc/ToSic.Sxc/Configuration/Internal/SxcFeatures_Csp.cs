using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{
    public static readonly Feature ContentSecurityPolicy = new() {
        NameId = "ContentSecurityPolicy",
        Guid = new("e5c99abf-9bc4-4e6c-9cc4-4bda22c0ab85"),
        Name = "Enable Content Security Policy and related APIs. ",
        IsPublic = false,
        Ui = false,
        Description = "If enabled, ContentSecurityPolicy headers will be set. Note that APIs will always work, but not result in http headers if this is disabled. ",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelDisabled
    };

    public static readonly Feature ContentSecurityPolicyTestUrl = new()
    {
        NameId = "ContentSecurityPolicyTestUrl",
        Guid = new("94c3d7e5-5d89-4a68-b710-95bfc29199ba"),
        Name = "Enable the parameter ?csp=true in URL for testing.",
        IsPublic = false,
        Ui = false,
        Description = "If enabled, you can add csp=true to any url to temporarily enable a policy and see if it works. Requires the CSP system to be enabled.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelDisabled
    };
    public static readonly Feature ContentSecurityPolicyEnforceTemp = new() {
        NameId = "ContentSecurityPolicyEnforceTemp",
        Guid = new("ece0943d-f3c0-422c-9d06-c20b9b83df8d"),
        Name = "Enable CSP on all pages (temporary setting).",
        IsPublic = false,
        Ui = false,
        Description = "Enable CSP on all pages. This is a temporary setting, till we have more configuration in normal Settings.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelDisabled
    };

    public static readonly Feature CdnSourcePublic = new()
    {
        NameId = nameof(CdnSourcePublic),
        Guid = new("b8b993d3-a02b-4099-a2a8-c06bf8961a66"),
        Name = "Change CDN source for public Web Resources",
        IsPublic = false,
        Ui = false,
        Description = "Allow reconfigure of the CDN source for public Web Resources.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelEnabled
    };
    public static readonly Feature CdnSourceEdit = new() {
        NameId = nameof(CdnSourceEdit),
        Guid = new("34dce40e-30fc-4d4f-b1ab-8fcface90e61"),
        Name = "Change CDN source for Edit Web Resources",
        IsPublic = false,
        Ui = false,
        Description = "Allow reconfigure of the CDN source for Web Resources used in the Edit UI.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelEnabled
    };

    // Note = not in use ATM
    public static readonly Feature CdnSourceAdmin = new() {
        NameId = nameof(CdnSourceAdmin),
        Guid = new("c799c71e-aa4a-4a30-ae8a-e177e615a36c"),
        Name = "Change CDN source for Admin Web Resources",
        IsPublic = false,
        Ui = false,
        Description = "Allow reconfigure of the CDN source for Web Resources used in the Admin UI.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelEnabled
    };

    // Note = not in use ATM
    public static readonly Feature CdnSourceDev = new() {
        NameId = nameof(CdnSourceDev),
        Guid = new("81a51003-ad55-491e-9749-d74529496465"),
        Name = "Change CDN source for Development Web Resources",
        IsPublic = false,
        Ui = false,
        Description = "Allow reconfigure of the CDN source for Web Resources used in the Developers UIs.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelEnabled
    };


    public static readonly Feature NetworkDataEncryption = new() {
        NameId = nameof(NetworkDataEncryption),
        Guid = new("6c333e6f-d552-431a-b47c-0030764a66f3"),
        Name = "Encrypt forms data before sending to server; prevent CDN snooping.",
        IsPublic = false,
        Ui = false,
        Description = "Encrypt data submitted using forms, so it's unreadable in CDNs.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = BuiltInFeatures.ForPatronsSentinelEnabled,

        ScopedToModule = true,
    };

}