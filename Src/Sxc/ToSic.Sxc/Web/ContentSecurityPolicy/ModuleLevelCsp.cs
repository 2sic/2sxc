using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Configuration.Features;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class ModuleLevelCsp: INeedsDynamicCodeRoot
    {
        public const string CspHeaderNamePolicy = "Content-Security-Policy";
        public const string CspHeaderNameReport = "Content-Security-Policy-Report-Only";
        public const string CspUrlParameter = "csp";
        public const string CspUrlTrue = "true";
        public const string CspUrlDev = "dev";

        #region Constructor

        public ModuleLevelCsp(IUser user, IFeaturesService featuresService)
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
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRoot = codeRoot;
        private IDynamicCodeRoot _codeRoot;

        internal IParameters PageParameters => _pageParameters.Get(() => _codeRoot?.CmsContext?.Page?.Parameters);
        private readonly ValueGetOnce<IParameters> _pageParameters = new ValueGetOnce<IParameters>();
        internal DynamicStack PageSettings => _pageSettings.Get(() => _codeRoot?.Settings as DynamicStack);
        private readonly ValueGetOnce<DynamicStack> _pageSettings = new ValueGetOnce<DynamicStack>();

        #endregion

        #region Read Settings

        /// <summary>
        /// CSP Settings Reader from Dynamic Entity
        /// </summary>
        private CspSettingsReader CspSettings => _cspSettings.Get(()
            => new CspSettingsReader(PageSettings, _user,
                CspUrlParam.EqualsInsensitive(CspUrlDev)));
        private readonly ValueGetOnce<CspSettingsReader> _cspSettings = new ValueGetOnce<CspSettingsReader>();

        /// <summary>
        /// Enforce?
        /// </summary>
        internal bool IsEnforced => _cspReportOnly.Get(() => CspSettings.IsEnforced);
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

            //if (!UrlEnabled) return false;
            //if (_pageSvcShared.PageParameters == null) return false;

            return CspUrlParam.EqualsInsensitive(CspUrlTrue) || CspUrlParam.EqualsInsensitive(CspUrlDev);
        });
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        //private bool UrlEnabled => _urlEnabled.Get(() =>
        //    _pageSvcShared.FeaturesService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId));
        //private readonly ValueGetOnce<bool> _urlEnabled = new ValueGetOnce<bool>();

        private string CspUrlParam => _cspUrlParam.Get(() =>
        {
            if (!_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId))
                return null;
            if (PageParameters == null) return null;
            PageParameters.TryGetValue(CspUrlParameter, out var cspParam);
            return cspParam;
        });
        private readonly ValueGetOnce<string> _cspUrlParam = new ValueGetOnce<string>();

        internal List<KeyValuePair<string, string>> Policies 
            => _policies.Get(() => new CspPolicyTextProcessor().Parse(CspSettings.Policies));
        private readonly ValueGetOnce<List<KeyValuePair<string, string>>> _policies = new ValueGetOnce<List<KeyValuePair<string, string>>>();

        #endregion

        internal void AddCspService(CspServiceBase provider) => CspServices.Add(provider);
        internal readonly List<CspServiceBase> CspServices = new List<CspServiceBase>();

        /// <summary>
        /// Name of the CSP header to be added, based on the report-only aspect
        /// </summary>
        public string HeaderName => IsEnforced ? CspHeaderNamePolicy : CspHeaderNameReport;


        public List<HttpHeader> CspHeaders()
        {
            if (!IsEnabled) return new List<HttpHeader>();
            if (Policies.Any())
            {
                // Create a CspService which just contains these new policies for merging later on
                var policyCsp = new CspServiceBase();
                foreach (var policy in Policies)
                    policyCsp.Add(policy.Key, policy.Value);
                AddCspService(policyCsp);
            }
            if (!CspServices.Any()) return new List<HttpHeader>();
            var header = CspHttpHeader();
            return header == null ? new List<HttpHeader>() : new List<HttpHeader> { header };
        }

        private HttpHeader CspHttpHeader()
        {
            var relevant = CspServices.Where(cs => cs != null).ToList();
            if (!relevant.Any()) return null;
            var first = relevant.First();
            var mergedPolicy = first.Policy;

            var finalizer = new CspParameterFinalizer();

            if (relevant.Count == 1)
                return new HttpHeader(HeaderName, finalizer.Finalize(mergedPolicy).ToString());

            // If many, merge the settings of each additional policy list
            foreach (var cspS in relevant.Skip(1))
                mergedPolicy.Add(cspS.Policy);

            return new HttpHeader(HeaderName, finalizer.Finalize(mergedPolicy).ToString());
        }
    }
}
