using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationStack;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class CspOfApp : HasLog, INeedsDynamicCodeRoot
    {
        public int AppId => _codeRoot?.Block?.AppId ?? 0;

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
        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            var wrapLog = Log.Call2();
            _codeRoot = codeRoot;

            // Also connect upstream CspOfModule in case it's not yet connected
            _moduleCsp.ConnectToRoot(codeRoot);
            wrapLog.Done();
        }

        private IDynamicCodeRoot _codeRoot;

        #endregion

        #region 

        //private DynamicStack CodeRootSettings()
        //{
        //    return stack;
        //}

        #endregion

        #region Read Settings

        public string AppPolicies => _appPolicies.Get(GetAppPolicies);
        private readonly ValueGetOnce<string> _appPolicies = new ValueGetOnce<string>();

        private string GetAppPolicies()
        {
            var cLog = Log.Call2<string>(AppId.ToString());

            // Get Stack
            var stack = _codeRoot?.Settings as DynamicStack;
            // Enable this for detailed debugging
            //if (stack != null) stack.Debug = true;

            // Dynamic Stack of the App Settings
            var appSettings = stack?.GetStack(PartAppSystem) as DynamicStack;
            Log.Add($"{nameof(stack)}: {stack != null}; {nameof(appSettings)}: {appSettings != null}");

            // CSP Settings Reader from Dynamic Entity for the App
            var cspReader = new CspSettingsReader(appSettings, _user, _moduleCsp.UrlIsDevMode, Log);
            var policies = cspReader.Policies;
            return cLog.Done(policies, policies);
        }
        #endregion


    }
}
