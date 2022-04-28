using ToSic.Sxc.Data;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class CspSettingsReader
    {
        //private const string EnabledField = "Enabled";
        //private const string EnforceField = "Enforce";
        //private const string PolicyField = "Policy";

        public DynamicStack SettingsOrNull { get; }

        public CspSettingsReader(DynamicStack settingsOrNull) => SettingsOrNull = settingsOrNull;

        public bool Enabled => (SettingsOrNull as dynamic)?.ContentSecurityPolicies?.All?.IsEnabled == true;

        public bool Enforce => (SettingsOrNull as dynamic)?.ContentSecurityPolicies?.All?.IsEnforced == true;

        public string Policy => (SettingsOrNull as dynamic)?.ContentSecurityPolicies?.All?.Policy as string;
    }
}
