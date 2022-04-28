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
                ContentSecurityPolicyEnforceTemp
            );


    }
}
