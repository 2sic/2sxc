using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Stack;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// This object reads the CSP settings of an app and passes it to the <see cref="CspOfModule"/>.
/// This is important because a module can have multiple apps in it, so it must merge the Csp Settings
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CspOfApp : ServiceForDynamicCode
{
    public int AppId => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block?.AppId ?? 0;

    #region Constructor

    public CspOfApp(IUser user, CspOfModule moduleCsp) : base(CspConstants.LogPrefix + ".AppLvl")
    {
        _user = user;
        _moduleCsp = moduleCsp;
        moduleCsp.RegisterAppCsp(this); // Attach to parent which is scoped for the entire module
    }

    private readonly IUser _user;
    private readonly CspOfModule _moduleCsp;

    /// <summary>
    /// Connect to code root, so page-parameters and settings will be available later on.
    /// Important: page-parameters etc. are not available at this time, so don't try to get them until needed
    /// </summary>
    /// <param name="codeRoot"></param>
    public override void ConnectToRoot(ICodeApiService codeRoot)
    {
        var l = Log.Fn();
        base.ConnectToRoot(codeRoot);
        // Also connect upstream CspOfModule in case it's not yet connected
        _moduleCsp.ConnectToRoot(codeRoot);
        l.Done();
    }

    #endregion

    #region Read Settings

    public string AppPolicies => _appPolicies.Get(GetAppPolicies);
    private readonly GetOnce<string> _appPolicies = new();

    private string GetAppPolicies()
    {
        var cLog = Log.Fn<string>(AppId.ToString());

        // Get Stack
        if (_CodeApiSvc?.Settings is not DynamicStack stack) 
            return cLog.ReturnNull("no stack");

        // Enable this for detailed debugging
        //stack.Debug = true;

        // Dynamic Stack of the App Settings
        var appSettings = stack.GetStack(AppStackConstants.PartAppSystem) as DynamicStack;
        Log.A($"has {nameof(appSettings)}: {appSettings != null}");

        // CSP Settings Reader from Dynamic Entity for the App
        var cspReader = new CspSettingsReader(appSettings, _user, _moduleCsp.UrlIsDevMode, Log);
        var policies = cspReader.Policies;
        return cLog.ReturnAndLog(policies);
    }
    #endregion


}