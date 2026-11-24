using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Web.Sys.ContentSecurityPolicy;

/// <summary>
/// This object reads the CSP settings of an app and passes it to the <see cref="CspOfModule"/>.
/// This is important because a module can have multiple apps in it, so it must merge the Csp Settings
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CspOfApp(
    Ctx<IAppIdentity> ctxAppId,
    Ctx<ICodeContextSettings> ctxAllSettings,
    IUser user,
    CspOfModule moduleCsp)
    : ServiceBase(CspConstants.LogPrefix + ".AppLvl", connect: [])
{
    public int AppId => _appId ??= ctxAppId.ValueOptional?.AppId ?? 0;
    private int? _appId;

    #region Constructor / Post-Setup

    /// <summary>
    /// This must be called upon creation - ATM only in the CodeContext
    /// </summary>
    public void RequiredPostSetup() => moduleCsp.RegisterAppCsp(this);

    #endregion

    #region Read Settings

    public string? AppPolicies => _appPolicies.Get(GetAppPolicies);
    private readonly LazyGet<string?> _appPolicies = new();

    private string? GetAppPolicies()
    {
        var l = Log.Fn<string?>(AppId.ToString());

        // Get Stack
        if (ctxAllSettings.ValueOptional?.AllSettings is not { } stack) 
            return l.ReturnNull("no stack");

        // Enable this for detailed debugging
        //stack.Debug = true;

        // Stack of the App Settings
        var appSettings = stack.GetStack([AppStackConstants.PartAppSystem]);
        l.A($"has {nameof(appSettings)}: {appSettings != null}");

        // CSP Settings Reader from Dynamic Entity for the App
        var cspReader = new CspSettingsReader(appSettings, user, moduleCsp.UrlIsDevMode, Log);
        var policies = cspReader.Policies;
        return l.ReturnAndLog(policies);
    }
    #endregion


}