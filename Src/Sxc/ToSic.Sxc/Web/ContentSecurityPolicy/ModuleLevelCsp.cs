using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Configuration.Features;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class ModuleLevelCsp: HasLog, INeedsDynamicCodeRoot
    {
        #region Constructor

        public ModuleLevelCsp(IUser user, IFeaturesService featuresService): base(CspConstants.LogPrefix + ".ModLvl")
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
            Log.LinkTo(codeRoot.Log);
            _codeRoot = codeRoot;
        }

        private IDynamicCodeRoot _codeRoot;

        internal IParameters PageParameters => _pageParameters.Get(() => _codeRoot?.CmsContext?.Page?.Parameters, Log, nameof(PageParameters));
        private readonly ValueGetOnce<IParameters> _pageParameters = new ValueGetOnce<IParameters>();
        internal DynamicStack PageSettings => _pageSettings.Get(() => _codeRoot?.Settings as DynamicStack, Log, nameof(PageSettings));
        private readonly ValueGetOnce<DynamicStack> _pageSettings = new ValueGetOnce<DynamicStack>();

        #endregion

        #region Read Settings

        /// <summary>
        /// CSP Settings Reader from Dynamic Entity
        /// </summary>
        private CspSettingsReader CspSettings => _cspSettings.Get(()
            => new CspSettingsReader(PageSettings, _user,
                CspUrlParam.EqualsInsensitive(CspConstants.CspUrlDev), Log), Log, nameof(CspServices));
        private readonly ValueGetOnce<CspSettingsReader> _cspSettings = new ValueGetOnce<CspSettingsReader>();

        /// <summary>
        /// Enforce?
        /// </summary>
        internal bool IsEnforced => _cspReportOnly.Get(() => CspSettings.IsEnforced, Log, nameof(IsEnforced));
        private readonly ValueGetOnce<bool> _cspReportOnly = new ValueGetOnce<bool>();

        /// <summary>
        /// Check if enabled based on various criteria like features, url-param, settings etc.
        /// </summary>
        internal bool IsEnabled => _enabled.Get(() =>
        {
            var enabled = _featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId);
            if (!enabled) return false;
            var enforce = _featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyEnforceTemp.NameId);
            if (enforce) return true;

            // Try settings
            if (CspSettings.IsEnabled) return true;

            return CspUrlParam.EqualsInsensitive(CspConstants.CspUrlTrue) || CspUrlParam.EqualsInsensitive(CspConstants.CspUrlDev);
        }, Log, nameof(IsEnabled));
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        private string CspUrlParam => _cspUrlParam.Get(() =>
        {
            if (!_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId))
                return null;
            if (PageParameters == null) return null;
            PageParameters.TryGetValue(CspConstants.CspUrlParameter, out var cspParam);
            return cspParam;
        }, Log, nameof(CspUrlParam));
        private readonly ValueGetOnce<string> _cspUrlParam = new ValueGetOnce<string>();

        internal List<KeyValuePair<string, string>> Policies 
            => _policies.Get(() => new CspPolicyTextProcessor().Parse(CspSettings.Policies), Log, nameof(Policies));
        private readonly ValueGetOnce<List<KeyValuePair<string, string>>> _policies = new ValueGetOnce<List<KeyValuePair<string, string>>>();

        #endregion

        internal void AddCspService(CspServiceBase provider) => CspServices.Add(provider);
        internal readonly List<CspServiceBase> CspServices = new List<CspServiceBase>();

        /// <summary>
        /// Name of the CSP header to be added, based on the report-only aspect
        /// </summary>
        public string HeaderName => IsEnforced ? CspConstants.CspHeaderNamePolicy : CspConstants.CspHeaderNameReport;


        public List<HttpHeader> CspHeaders()
        {
            var wrapLog = Log.Call<List<HttpHeader>>();
            if (!IsEnabled) return wrapLog("disabled", new List<HttpHeader>());
            if (Policies.Any())
            {
                Log.Add("Policies found");
                // Create a CspService which just contains these new policies for merging later on
                var policyCsp = new CspServiceBase();
                foreach (var policy in Policies)
                    policyCsp.Add(policy.Key, policy.Value);
                AddCspService(policyCsp);
            }

            if (!CspServices.Any()) return wrapLog("no services to add", new List<HttpHeader>());
            var header = CspHttpHeader();
            var result = header == null ? new List<HttpHeader>() : new List<HttpHeader> { header };
            return wrapLog("ok", result);
        }

        private HttpHeader CspHttpHeader()
        {
            var wrapLog = Log.Call<HttpHeader>();
            var relevant = CspServices.Where(cs => cs != null).ToList();
            if (!relevant.Any()) return wrapLog("none relevant", null);
            var first = relevant.First();
            var mergedPolicy = first.Policy;

            var finalizer = new CspParameterFinalizer().Init(Log);

            if (relevant.Count == 1)
                return wrapLog("found 1", new HttpHeader(HeaderName, finalizer.Finalize(mergedPolicy).ToString()));

            // If many, merge the settings of each additional policy list
            foreach (var cspS in relevant.Skip(1))
                mergedPolicy.Add(cspS.Policy);

            return wrapLog("merged", new HttpHeader(HeaderName, finalizer.Finalize(mergedPolicy).ToString()));
        }
    }
}
