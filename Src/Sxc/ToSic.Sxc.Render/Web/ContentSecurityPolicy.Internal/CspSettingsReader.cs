using ToSic.Eav.Context;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// Helper class to read the dynamic settings for the current site or global to be used in CSP
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CspSettingsReader(ICanGetByName settingsStackOrNull, IUser user, bool devMode, ILog parentLog)
    : HelperBase(parentLog, $"{CspConstants.LogPrefix}.Setting")
{
    private const string FieldIsEnabled = "IsEnabled";
    private const string FieldIsEnforced = "IsEnforced";
    private const string FieldPolicies = "Policies";
    private const string FieldContentSecurityPolicies = "ContentSecurityPolicies";

    // Enable this for edge cases where we must debug deeply into each settings-stack
    // if (_settingsOrNull != null) _settingsOrNull.Debug = true;

    public bool IsEnabled => GetFromPreferredOrDefaultSource(FieldIsEnabled) as bool? == true;

    public bool IsEnforced => GetFromPreferredOrDefaultSource(FieldIsEnforced) as bool? == true;

    public string Policies => GetFromPreferredOrDefaultSource(FieldPolicies) as string;

    /// <summary>
    /// Get bool / string settings from the preferred settings or the default settings
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private object GetFromPreferredOrDefaultSource(string field)
    {
        var cLog = Log.Fn<object>(field);

        var pref = SettingPreferred;
        if (pref.Entity?.Get(field) is { } result)
            return cLog.Return(result, $"Preferred[{pref.Name}]: {result}");

        if (SettingsDefault?.Get(field) is { } result2)
            return cLog.Return(result2, $"Default Source: {result2}");

        return cLog.ReturnNull("not found");
    }

    private (string Name, /*DynamicEntity Setting,*/ IEntity Entity) SettingPreferred => _preferred.Get(Log, () =>
    {
        Log.A($"Dev: {devMode}; Super: {user.IsSystemAdmin}; Admin: {user.IsSiteAdmin}; Anon: {user.IsAnonymous}");

        if (devMode) return GetName("Dev");
        if (user.IsSystemAdmin) return GetName("SystemAdmin");
        if (user.IsSiteAdmin) return GetName("SiteAdmin");
        if (user.IsAnonymous) return GetName("Anonymous");
        return ("none", /*null,*/ null);

        (string Name, /*DynamicEntity Setting,*/ IEntity Entity) GetName(string theName)
            => (theName, (SettingsRoot?.Get(theName) as ICanBeEntity /*DynamicEntity*/)?.Entity);
    });
    private readonly GetOnce<(string Name, /*DynamicEntity Setting,*/ IEntity Entity)> _preferred = new();

    /// <summary>
    /// The fallback settings, which will be null if in devMode, because then we shouldn't do a fallback
    /// </summary>
    private ICanGetByName SettingsDefault => devMode
        ? null
        : _default.Get(() => SettingsRoot?.Get("Default") as ICanGetByName /*DynamicEntity*/);
    private readonly GetOnce<ICanGetByName> _default = new();

    private ICanGetByName SettingsRoot => _settingsRoot.Get(Log,
        () => settingsStackOrNull?.Get(FieldContentSecurityPolicies) as ICanGetByName /*DynamicEntity*/
    );
    private readonly GetOnce<ICanGetByName> _settingsRoot = new();

}