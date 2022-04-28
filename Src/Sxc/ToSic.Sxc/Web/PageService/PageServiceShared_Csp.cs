using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Configuration.Features;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        internal bool CspEnabled => _enabled.Get(() =>
            {
                var enabled = _featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId);
                if (!enabled) return false;
                var enforce = _featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyEnforceTemp.NameId);
                if (enforce) return true;

                // Try settings
                if (CspSettings.Enabled) return true;

                var urlEnabled = _featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId);
                if (!urlEnabled) return false;
                if (_pageParameters == null) return false;

                return _pageParameters.TryGetValue(CspService.CspUrlParameter, out var cspParam) 
                       && string.Equals("true", cspParam, StringComparison.InvariantCultureIgnoreCase);
            });
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        internal bool CspEnforce => _cspReportOnly.Get(() => !CspSettings.Enforce);
        private readonly ValueGetOnce<bool> _cspReportOnly = new ValueGetOnce<bool>();

        internal void AddCspService(CspService provider) => _cspServices.Add(provider);
        private readonly List<CspService> _cspServices = new List<CspService>();

        private CspSettingsReader CspSettings => _cspSettings.Get(() => new CspSettingsReader(_pageSettings));
        private readonly ValueGetOnce<CspSettingsReader> _cspSettings = new ValueGetOnce<CspSettingsReader>();

        public List<HttpHeader> CspHeaders
        {
            get
            {
                // This would group the headers by report-only and normal csp
                // probably disable in future, as the setting should be global
                var byType = _cspServices;
                    //?
                    //.Where(cs => cs != null)
                    //.GroupBy(cs => cs.Name)
                    //.ToList();

                if (byType == null || !byType.Any()) return new List<HttpHeader>();

                //return byType.Select(list => CspHttpHeader(list.ToList())).ToList();
                var header = CspHttpHeader(_cspServices);
                if (header == null) return new List<HttpHeader>();
                return new List<HttpHeader>() { header };
            }
        }

        private static HttpHeader CspHttpHeader(IReadOnlyCollection<CspService> servicesOfThisTypeOrNull)
        {
            if (servicesOfThisTypeOrNull == null) return null;

            var relevant = servicesOfThisTypeOrNull.Where(cs => cs != null).ToList();
            if (relevant?.Any() != true) return null;
            var first = relevant.First();
            var mergedPolicy = first.Policy;

            var finalizer = new CspParameterFinalizer();

            if (relevant.Count == 1)
                return new HttpHeader(first.Name, finalizer.Finalize(mergedPolicy).ToString());

            // If many, merge the settings of each additional policy list
            foreach (var cspS in relevant.Skip(1)) 
                mergedPolicy.Add(cspS.Policy);

            return new HttpHeader(first.Name, finalizer.Finalize(mergedPolicy).ToString());
        }


    }
}
