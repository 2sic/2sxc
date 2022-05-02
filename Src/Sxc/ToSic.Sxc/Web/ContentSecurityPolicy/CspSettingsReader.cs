using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    /// <summary>
    /// Helper class to read the dynamic settings for the current site or global to be used in CSP
    /// </summary>
    public class CspSettingsReader: HasLog
    {
        public CspSettingsReader(DynamicStack settingsOrNull, IUser user, bool devMode, ILog parentLog): base(CspConstants.LogPrefix + ".Setting", parentLog)
        {
            _user = user;
            _devMode = devMode;
            _settingsOrNull = settingsOrNull;
            
            // Enable this for edge cases where we must debug deeply into each settings-stack
            //if (_settingsOrNull != null) _settingsOrNull.Debug = true;
        }
        private readonly IUser _user;
        private readonly bool _devMode;
        private readonly DynamicStack _settingsOrNull;


        public bool IsEnabled => SettingPreferred?.IsEnabled ?? SettingsDefault?.IsEnabled == true;

        public bool IsEnforced => SettingPreferred?.IsEnforced ?? SettingsDefault?.IsEnforced == true;

        public string Policies => SettingPreferred?.Policies ?? SettingsDefault?.Policies as string;

        private dynamic SettingsRoot => _settingsRoot.Get(() => (_settingsOrNull as dynamic)?.ContentSecurityPolicies, Log, nameof(SettingsRoot));
        private readonly ValueGetOnce<dynamic> _settingsRoot = new ValueGetOnce<dynamic>();

        private dynamic SettingPreferred => _preferred.Get(() =>
        {
            Log.Add($"Dev: {_devMode}; Super: {_user.IsSuperUser}; Admin: {_user.IsAdmin}; Anon: {_user.IsAnonymous}");
            if (_devMode) return SettingsRoot?.Dev;
            if (_user.IsSuperUser) return SettingsRoot?.SystemAdmin;
            if (_user.IsAdmin) return SettingsRoot?.SiteAdmin;
            if (_user.IsAnonymous) return SettingsRoot?.Anonymous;
            return null;
        }, Log, nameof(SettingPreferred));
        private readonly ValueGetOnce<dynamic> _preferred = new ValueGetOnce<dynamic>();

        /// <summary>
        /// The fallback settings, which will be null if in devMode, because then we shouldn't do a fallback
        /// </summary>
        private dynamic SettingsDefault => _devMode ? null : _default.Get(() => SettingsRoot?.Default);
        private readonly ValueGetOnce<dynamic> _default = new ValueGetOnce<dynamic>();

    }
}
