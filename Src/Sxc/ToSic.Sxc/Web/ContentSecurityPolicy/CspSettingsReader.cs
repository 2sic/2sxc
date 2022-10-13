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
            // if (_settingsOrNull != null) _settingsOrNull.Debug = true;
        }
        private readonly IUser _user;
        private readonly bool _devMode;
        private readonly DynamicStack _settingsOrNull;

        private object GetFromPreferredOrDefaultSource(string field)
        {
            var cLog = Log.Fn<object>(field);

            var pref = SettingPreferred;
            if (pref.Setting?.Get(field) is object result)
                return cLog.Return(result, $"Preferred[{pref.Name}]: {result}");

            if (SettingsDefault?.Get(field) is object result2)
                return cLog.Return(result2, $"Default Source: {result2}");

            return cLog.ReturnNull("not found");
        }

        public bool IsEnabled => GetFromPreferredOrDefaultSource(FieldIsEnabled) as bool? == true;

        public bool IsEnforced => GetFromPreferredOrDefaultSource(FieldIsEnforced) as bool? == true;

        public string Policies => GetFromPreferredOrDefaultSource(FieldPolicies) as string;

        private (string Name, DynamicEntity Setting) SettingPreferred => _preferred.Get(() =>
        {
            Log.A($"Dev: {_devMode}; Super: {_user.IsSystemAdmin}; Admin: {_user.IsSiteAdmin}; Anon: {_user.IsAnonymous}");
            (string, DynamicEntity) GetName(string theName) => (theName, SettingsRoot?.Get(theName) as DynamicEntity);

            if (_devMode) return GetName("Dev");
            if (_user.IsSystemAdmin) return GetName("SystemAdmin");
            if (_user.IsSiteAdmin) return GetName("SiteAdmin");
            if (_user.IsAnonymous) return GetName("Anonymous");
            return ("none", null);
        }, Log, nameof(SettingPreferred));
        private readonly GetOnce<(string Name, DynamicEntity Settings)> _preferred = new GetOnce<(string, DynamicEntity)>();

        /// <summary>
        /// The fallback settings, which will be null if in devMode, because then we shouldn't do a fallback
        /// </summary>
        private DynamicEntity SettingsDefault => _devMode ? null : _default.Get(() => SettingsRoot?.Get("Default") as DynamicEntity);
        private readonly GetOnce<DynamicEntity> _default = new GetOnce<DynamicEntity>();

        private DynamicEntity SettingsRoot => _settingsRoot.Get(() => _settingsOrNull?.Get(FieldCSPs) as DynamicEntity, Log, nameof(SettingsRoot));
        private readonly GetOnce<DynamicEntity> _settingsRoot = new GetOnce<DynamicEntity>();

    }
}
