using ToSic.Eav.Context;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Stack;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// Helper class to read the dynamic settings for the current site or global to be used in CSP
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CspSettingsReader(DynamicStack settingsOrNull, IUser user, bool devMode, ILog parentLog)
    : HelperBase(parentLog, $"{CspConstants.LogPrefix}.Setting")
{
    private const string FieldIsEnabled = "IsEnabled";
    private const string FieldIsEnforced = "IsEnforced";
    private const string FieldPolicies = "Policies";
    private const string FieldCSPs = "ContentSecurityPolicies";

    // Enable this for edge cases where we must debug deeply into each settings-stack
    // if (_settingsOrNull != null) _settingsOrNull.Debug = true;

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

    private (string Name, DynamicEntity Setting) SettingPreferred => _preferred.Get(Log, () =>
    {
        Log.A($"Dev: {devMode}; Super: {user.IsSystemAdmin}; Admin: {user.IsSiteAdmin}; Anon: {user.IsAnonymous}");
        (string, DynamicEntity) GetName(string theName) => (theName, SettingsRoot?.Get(theName) as DynamicEntity);

        if (devMode) return GetName("Dev");
        if (user.IsSystemAdmin) return GetName("SystemAdmin");
        if (user.IsSiteAdmin) return GetName("SiteAdmin");
        if (user.IsAnonymous) return GetName("Anonymous");
        return ("none", null);
    });
    private readonly GetOnce<(string Name, DynamicEntity Settings)> _preferred = new();

    /// <summary>
    /// The fallback settings, which will be null if in devMode, because then we shouldn't do a fallback
    /// </summary>
    private DynamicEntity SettingsDefault => devMode ? null : _default.Get(() => SettingsRoot?.Get("Default") as DynamicEntity);
    private readonly GetOnce<DynamicEntity> _default = new();

    private DynamicEntity SettingsRoot => _settingsRoot.Get(Log, () => settingsOrNull?.Get(FieldCSPs) as DynamicEntity);
    private readonly GetOnce<DynamicEntity> _settingsRoot = new();

}