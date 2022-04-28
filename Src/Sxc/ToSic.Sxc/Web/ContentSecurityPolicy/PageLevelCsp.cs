using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Configuration.Features;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class PageLevelCsp
    {
        public const string CspHeaderNamePolicy = "Content-Security-Policy";
        public const string CspHeaderNameReport = "Content-Security-Policy-Report-Only";
        public const string CspUrlParameter = "csp";

        #region Constructor

        public PageLevelCsp(PageServiceShared parentServiceShared) => _parentServiceShared = parentServiceShared;
        private readonly PageServiceShared _parentServiceShared;

        #endregion

        #region Read Settings

        /// <summary>
        /// CSP Settings Reader from Dynamic Entity
        /// </summary>
        private CspSettingsReader CspSettings => _cspSettings.Get(() => new CspSettingsReader(_parentServiceShared.PageSettings, _parentServiceShared.User));
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
            var featuresService = _parentServiceShared.FeaturesService;

            var enabled = featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId);
            if (!enabled) return false;
            var enforce = featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyEnforceTemp.NameId);
            if (enforce) return true;

            // Try settings
            if (CspSettings.IsEnabled) return true;

            var urlEnabled = featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId);
            if (!urlEnabled) return false;
            if (_parentServiceShared.PageParameters == null) return false;

            return _parentServiceShared.PageParameters.TryGetValue(CspUrlParameter, out var cspParam)
                   && string.Equals("true", cspParam, StringComparison.InvariantCultureIgnoreCase);
        });
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        internal List<KeyValuePair<string, string>> Policies => _policies.Get(() => new CspPolicyTextProcessor().Parse(CspSettings.Policies));
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
