using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// This object reads the CSP settings of an app and passes it to the <see cref="CspOfModule"/>.
/// This is important because a module can have multiple apps in it, so it must merge the Csp Settings
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CspOfApp : ServiceWithContext
{
    public int AppId => _appId ??= ExCtx.GetState<IBlock>()?.AppId ?? 0;
    private int? _appId;

    #region Constructor

    public CspOfApp(IUser user, CspOfModule moduleCsp) : base(CspConstants.LogPrefix + ".AppLvl", connect: [/* nothing everything is already connected */])
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
    /// <param name="exCtx"></param>
    public override void ConnectToRoot(IExecutionContext exCtx)
    {
        var l = Log.Fn();
        base.ConnectToRoot(exCtx);
        // Also connect upstream CspOfModule in case it's not yet connected
        _moduleCsp.ConnectToRoot(exCtx);
        l.Done();
    }

    #endregion

    #region Read Settings

    public string? AppPolicies => _appPolicies.Get(GetAppPolicies);
    private readonly GetOnce<string?> _appPolicies = new();

    private string? GetAppPolicies()
    {
        var l = Log.Fn<string?>(AppId.ToString());

        // Get Stack
        if (ExCtxOrNull?.GetState<IDynamicStack>(ExecutionContextStateNames.Settings) is not { } stack) 
            return l.ReturnNull("no stack");

        // Enable this for detailed debugging
        //stack.Debug = true;

        // Dynamic Stack of the App Settings
        var appSettings = stack.GetStack(AppStackConstants.PartAppSystem);
        Log.A($"has {nameof(appSettings)}: {appSettings != null}");

        // CSP Settings Reader from Dynamic Entity for the App
        var cspReader = new CspSettingsReader(appSettings, _user, _moduleCsp.UrlIsDevMode, Log);
        var policies = cspReader.Policies;
        return l.ReturnAndLog(policies);
    }
    #endregion


}