using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationStack;
using BuiltInFeatures = ToSic.Sxc.Configuration.Features.BuiltInFeatures;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public  class CspOfModule: HasLog, INeedsDynamicCodeRoot
    {
        #region Constructor

        public CspOfModule(IUser user, IFeaturesService featuresService): base(CspConstants.LogPrefix + ".ModLvl")
        {
            _user = user;
            _featuresService = featuresService;
        }

        private readonly IUser _user;
        private readonly IFeaturesService _featuresService;

        /// <summary>
        /// Connect to code root, so page-parameters and settings will be available later on.
        /// Important: page-parameters etc. are not available at this time, so don't try to get them until needed
        /// </summary>
        /// <param name="codeRoot"></param>
        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            if (_alreadyConnected) return;
            _alreadyConnected = true;
            Log.LinkTo(codeRoot.Log);
            _codeRoot = codeRoot;
            Log.Call2().Done();
        }

        private bool _alreadyConnected;

        private IDynamicCodeRoot _codeRoot;
        private DynamicStack CodeRootSettings()
        {
            var stack = _codeRoot?.Settings as DynamicStack;
            // Enable this for detailed debugging
            //if (stack != null) stack.Debug = true;
            return stack;
        }

        #endregion

        #region App Level CSP Providers

        /// <summary>
        /// Each App will register itself here to be added to the CSP list
        /// </summary>
        private List<CspOfApp> AppCsps = new List<CspOfApp>();

        internal bool RegisterAppCsp(CspOfApp appCsp)
        {
            var cLog = Log.Call2<bool>($"{appCsp?.AppId}");
            if (appCsp == null) return cLog.Done("null", false);
            if (AppCsps.Any(a => a.AppId == appCsp.AppId)) return cLog.Done($"app {appCsp.AppId} exists", false);
            AppCsps.Add(appCsp);
            return cLog.Done("added", true);
        }

        #endregion

        #region Url Parameters to Detect Dev / True

        public bool UrlIsDevMode => _urlDevMode.Get(() => CspUrlParam.EqualsInsensitive(CspConstants.CspUrlDev));
        private readonly ValueGetOnce<bool> _urlDevMode = new ValueGetOnce<bool>();

        private string CspUrlParam => _cspUrlParam.Get(() =>
        {
            if (!_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId))
                return null;
            var pageParameters = _codeRoot?.CmsContext?.Page?.Parameters;
            if (pageParameters == null) return null;
            pageParameters.TryGetValue(CspConstants.CspUrlParameter, out var cspParam);
            return cspParam;
        }, Log, nameof(CspUrlParam));
        private readonly ValueGetOnce<string> _cspUrlParam = new ValueGetOnce<string>();

        #endregion

        #region Read Settings

        /// <summary>
        /// CSP Settings Reader from Dynamic Entity for the Site
        /// </summary>
        private CspSettingsReader SiteCspSettings => _siteCspSettings.Get(() =>
        {
            var pageSettings = CodeRootSettings()?.GetStack(PartSiteSystem, PartGlobalSystem, PartPresetSystem);
            return new CspSettingsReader(pageSettings, _user, UrlIsDevMode, Log);
        }, Log, nameof(SiteCspSettings));
        private readonly ValueGetOnce<CspSettingsReader> _siteCspSettings = new ValueGetOnce<CspSettingsReader>();

        #endregion

        #region Enabled / Enforced

        /// <summary>
        /// Enforce?
        /// </summary>
        internal bool IsEnforced => _cspReportOnly.Get(() => SiteCspSettings.IsEnforced, Log, nameof(IsEnforced));
        private readonly ValueGetOnce<bool> _cspReportOnly = new ValueGetOnce<bool>();


        /// <summary>
        /// Check if enabled based on various criteria like features, url-param, settings etc.
        /// </summary>
        internal bool IsEnabled => _enabled.Get(() =>
        {
            // Check features
            if (!_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId))
                return false;
            if(_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyEnforceTemp.NameId))
                return true;

            // Try settings
            if (SiteCspSettings.IsEnabled) 
                return true;

            // Check URL Parameters - they are null if the feature is not enabled
            return CspUrlParam.EqualsInsensitive(CspConstants.CspUrlTrue) || UrlIsDevMode;
        }, Log, nameof(IsEnabled));
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();


        #endregion


        internal List<KeyValuePair<string, string>> Policies 
            => _policies.Get(() =>
            {
                var sitePolicies = SiteCspSettings.Policies;

                var appPolicies2 = AppCsps
                    .Select(ac =>
                    {
                        var p = ac.AppPolicies;
                        return p.HasValue() ? $"// AppId: {ac.AppId}\n{p}" : null;
                    })
                    .Where(p => p.HasValue())
                    .ToList();

                var appPolicies = string.Join("\n", appPolicies2);

                Log.Add("site:" + sitePolicies);
                Log.Add("app:" + appPolicies + $", from {AppCsps.Count} apps of which {appPolicies2.Count} had values");
                return new CspPolicyTextProcessor(Log).Parse($"{sitePolicies}\n{appPolicies}");
            }, Log, nameof(Policies));
        private readonly ValueGetOnce<List<KeyValuePair<string, string>>> _policies = new ValueGetOnce<List<KeyValuePair<string, string>>>();


        internal void AddCspService(ContentSecurityPolicyServiceBase provider) => CspServices.Add(provider);
        internal readonly List<ContentSecurityPolicyServiceBase> CspServices = new List<ContentSecurityPolicyServiceBase>();

        public List<CspParameters> CspParameters()
        {
            var wrapLog = Log.Call<List<CspParameters>>();
            if (!IsEnabled) return wrapLog("disabled", new List<CspParameters>());
            if (Policies.Any())
            {
                Log.Add("Policies found");
                // Create a CspService which just contains these new policies for merging later on
                var policyCsp = new ContentSecurityPolicyServiceBase();
                foreach (var policy in Policies)
                    policyCsp.Add(policy.Key, policy.Value);
                AddCspService(policyCsp);
            }

            if (!CspServices.Any()) return wrapLog("no services to add", new List<CspParameters>());
            var result = CspServices.Select(c => c?.Policy).Where(c => c != null).ToList();
            return wrapLog("ok", result);

        }
        
    }
}
