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
        private const string FieldIsEnabled = "IsEnabled";
        private const string FieldIsEnforced = "IsEnforced";
        private const string FieldPolicies = "Policies";
        private const string FieldCSPs = "ContentSecurityPolicies";

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


        public bool IsEnabled => SettingPreferred.Setting?.Get(FieldIsEnabled) ?? SettingsDefault?.Get(FieldIsEnabled) == true;

        public bool IsEnforced => SettingPreferred.Setting?.Get(FieldIsEnforced) ?? SettingsDefault?.Get(FieldIsEnforced) == true;

        public string Policies
        {
            get
            {
                var pref = SettingPreferred;
                if (pref.Setting?.Get(FieldPolicies) is string preferred)
                    return $"// Preferred for {pref.Name} \n" + preferred;

                if (SettingsDefault?.Get(FieldPolicies) is string defPolicies)
                    return $"// Default (not found on {pref.Name}) \n" + defPolicies;
                
                return null;
            }
        }

        private DynamicEntity SettingsRoot => _settingsRoot.Get(() => _settingsOrNull?.Get(FieldCSPs) as DynamicEntity, Log, nameof(SettingsRoot));
        private readonly ValueGetOnce<DynamicEntity> _settingsRoot = new ValueGetOnce<DynamicEntity>();

        private (string Name, DynamicEntity Setting) SettingPreferred => _preferred.Get(() =>
        {
            Log.Add($"Dev: {_devMode}; Super: {_user.IsSuperUser}; Admin: {_user.IsAdmin}; Anon: {_user.IsAnonymous}");
            (string, DynamicEntity) GetName(string theName) => (theName, SettingsRoot?.Get(theName) as DynamicEntity);

            if (_devMode) return GetName("Dev");
            if (_user.IsSuperUser) return GetName("SystemAdmin");
            if (_user.IsAdmin) return GetName("Admin");
            if (_user.IsAnonymous) return GetName("Anonymous");
            return ("none", null);
        }, Log, nameof(SettingPreferred));
        private readonly ValueGetOnce<(string Name, DynamicEntity Settings)> _preferred = new ValueGetOnce<(string, DynamicEntity)>();

        /// <summary>
        /// The fallback settings, which will be null if in devMode, because then we shouldn't do a fallback
        /// </summary>
        private DynamicEntity SettingsDefault => _devMode ? null : _default.Get(() => SettingsRoot?.Get("Default") as DynamicEntity);
        private readonly ValueGetOnce<DynamicEntity> _default = new ValueGetOnce<DynamicEntity>();

    }
}
