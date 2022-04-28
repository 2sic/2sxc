using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Configuration.Features;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.ContentSecurityPolicy;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        //public PageLevelCsp Csp => _csp ?? (_csp = new PageLevelCsp(this));
        //private PageLevelCsp _csp;

        //internal bool CspEnabled => _enabled.Get(() =>
        //    {
        //        var enabled = FeaturesService.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId);
        //        if (!enabled) return false;
        //        var enforce = FeaturesService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyEnforceTemp.NameId);
        //        if (enforce) return true;

        //        // Try settings
        //        if (CspSettings.IsEnabled) return true;

        //        var urlEnabled = FeaturesService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId);
        //        if (!urlEnabled) return false;
        //        if (PageParameters == null) return false;

        //        return PageParameters.TryGetValue(CspService.CspUrlParameter, out var cspParam) 
        //               && string.Equals("true", cspParam, StringComparison.InvariantCultureIgnoreCase);
        //    });
        //private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        //internal bool CspEnforce => _cspReportOnly.Get(() => !CspSettings.IsEnforced);
        //private readonly ValueGetOnce<bool> _cspReportOnly = new ValueGetOnce<bool>();

        //private CspSettingsReader CspSettings => _cspSettings.Get(() => new CspSettingsReader(PageSettings));
        //private readonly ValueGetOnce<CspSettingsReader> _cspSettings = new ValueGetOnce<CspSettingsReader>();

        //internal void AddCspService(CspService provider) => _cspServices.Add(provider);
        //private readonly List<CspService> _cspServices = new List<CspService>();



        //public List<HttpHeader> CspHeaders
        //{
        //    get
        //    {
        //        // This would group the headers by report-only and normal csp
        //        // probably disable in future, as the setting should be global
        //        var byType = _cspServices;
        //            //?
        //            //.Where(cs => cs != null)
        //            //.GroupBy(cs => cs.Name)
        //            //.ToList();

        //        if (byType == null || !byType.Any()) return new List<HttpHeader>();

        //        //return byType.Select(list => CspHttpHeader(list.ToList())).ToList();
        //        var header = CspHttpHeader(_cspServices);
        //        return header == null ? new List<HttpHeader>() : new List<HttpHeader> { header };
        //    }
        //}

        //private static HttpHeader CspHttpHeader(IReadOnlyCollection<CspService> servicesOfThisTypeOrNull)
        //{
        //    if (servicesOfThisTypeOrNull == null) return null;

        //    var relevant = servicesOfThisTypeOrNull.Where(cs => cs != null).ToList();
        //    if (relevant?.Any() != true) return null;
        //    var first = relevant.First();
        //    var mergedPolicy = first.Policy;

        //    var finalizer = new CspParameterFinalizer();

        //    if (relevant.Count == 1)
        //        return new HttpHeader(first.Name, finalizer.Finalize(mergedPolicy).ToString());

        //    // If many, merge the settings of each additional policy list
        //    foreach (var cspS in relevant.Skip(1)) 
        //        mergedPolicy.Add(cspS.Policy);

        //    return new HttpHeader(first.Name, finalizer.Finalize(mergedPolicy).ToString());
        //}


    }
}
