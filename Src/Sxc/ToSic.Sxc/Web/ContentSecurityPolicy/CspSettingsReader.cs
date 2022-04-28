using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    /// <summary>
    /// Helper class to read the dynamic settings for the current site or global to be used in CSP
    /// </summary>
    public class CspSettingsReader
    {
        public CspSettingsReader(DynamicStack settingsOrNull, IUser user)
        {
            _user = user;
            _settingsOrNull = settingsOrNull;
        }
        private readonly IUser _user;
        private readonly DynamicStack _settingsOrNull;


        public bool IsEnabled => SettingPreferred?.IsEnabled ?? SettingsAss?.IsEnabled == true;

        public bool IsEnforced => SettingPreferred?.IsEnforced ?? SettingsAss?.IsEnforced == true;

        public string Policies => SettingPreferred?.Policies ?? SettingsAss?.Policies as string;

        private dynamic SettingsRoot => _settingsRoot.Get(() => (_settingsOrNull as dynamic)?.ContentSecurityPolicies);
        private readonly ValueGetOnce<dynamic> _settingsRoot = new ValueGetOnce<dynamic>();

        private dynamic SettingPreferred => _preferred.Get(() =>
        {
            if (_user.IsSuperUser) return SettingsRoot?.SystemAdmin;
            if (_user.IsAdmin) return SettingsRoot?.SiteAdmin;
            if (_user.IsAnonymous) return SettingsRoot?.Anonymous;
            return null;
        });
        private readonly ValueGetOnce<dynamic> _preferred = new ValueGetOnce<dynamic>();

        private dynamic SettingsAss => _all.Get(() => SettingsRoot?.All);
        private readonly ValueGetOnce<dynamic> _all = new ValueGetOnce<dynamic>();

    }
}
