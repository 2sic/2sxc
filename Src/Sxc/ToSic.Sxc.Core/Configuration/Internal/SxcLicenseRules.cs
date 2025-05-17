using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Licenses;

namespace ToSic.Sxc.Configuration.Internal;
internal class SxcLicenseRules
{
    public static List<FeatureLicenseRule> ForPatronsPerfectionist = BuiltInLicenseRules.BuildRule(BuiltInLicenses.PatronPerfectionist, true);
    public static List<FeatureLicenseRule> ForPatronsSentinelDisabled = BuiltInLicenseRules.BuildRule(BuiltInLicenses.PatronSentinel, false);
    public static List<FeatureLicenseRule> ForPatronsSentinelEnabled = BuiltInLicenseRules.BuildRule(BuiltInLicenses.PatronSentinel, true);

}
