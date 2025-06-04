﻿using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CspOfModule(IUser user, IFeaturesService featuresService)
    : ServiceWithContext($"{CspConstants.LogPrefix}.ModLvl")
{
    #region App Level CSP Providers

    /// <summary>
    /// Each App will register itself here to be added to the CSP list
    /// </summary>
    private List<CspOfApp> AppCsps = [];

    internal bool RegisterAppCsp(CspOfApp appCsp)
    {
        var cLog = Log.Fn<bool>("appId: not yet known, not yet attached");
        if (appCsp == null)
            return cLog.ReturnFalse("null");

        // Note: We tried not-adding duplicates but this doesn't work
        // Because at the moment of registration, the AppId is often not known yet
        // Do not delete this comment, as others will attempt this too
        //if (AppCsps.Any(a => a.AppId == appCsp.AppId)) 
        //    return cLog.Done($"app {appCsp.AppId} exists", false);
        AppCsps.Add(appCsp);
        return cLog.ReturnTrue("added");
    }

    #endregion

    #region Url Parameters to Detect Dev / True

    public bool UrlIsDevMode => _urlDevMode.Get(() => CspUrlParam.EqualsInsensitive(CspConstants.CspUrlDev));
    private readonly GetOnce<bool> _urlDevMode = new();

    private string CspUrlParam => _cspUrlParam.Get(Log, () =>
    {
        if (!featuresService.IsEnabled(SxcFeatures.ContentSecurityPolicyTestUrl.NameId))
            return null;
        var pageParameters = ExCtx?.GetState<ICmsContext>()?.Page?.Parameters;
        if (pageParameters == null) return null;
        pageParameters.TryGetValue(CspConstants.CspUrlParameter, out var cspParam);
        return cspParam;
    });
    private readonly GetOnce<string> _cspUrlParam = new();

    #endregion

    #region Read Settings

    /// <summary>
    /// CSP Settings Reader from Dynamic Entity for the Site
    /// </summary>
    private CspSettingsReader SiteCspSettings => _siteCspSettings.Get(Log, () =>
    {
        var pageSettings = ExCtxOrNull?.GetState<IDynamicStack>(ExecutionContextStateNames.Settings)
            ?.GetStack(AppStackConstants.PartSiteSystem, AppStackConstants.PartGlobalSystem, AppStackConstants.PartPresetSystem);
        return new CspSettingsReader(pageSettings, user, UrlIsDevMode, Log);
    });
    private readonly GetOnce<CspSettingsReader> _siteCspSettings = new();

    #endregion

    #region Enabled / Enforced

    /// <summary>
    /// Enforce?
    /// </summary>
    internal bool IsEnforced => _cspReportOnly.Get(Log, () => SiteCspSettings.IsEnforced);
    private readonly GetOnce<bool> _cspReportOnly = new();


    /// <summary>
    /// Check if enabled based on various criteria like features, url-param, settings etc.
    /// </summary>
    internal bool IsEnabled => _enabled.Get(Log, () =>
    {
        // Check features
        if (!featuresService.IsEnabled(SxcFeatures.ContentSecurityPolicy.NameId))
            return false;
        if(featuresService.IsEnabled(SxcFeatures.ContentSecurityPolicyEnforceTemp.NameId))
            return true;

        // Try settings
        if (SiteCspSettings.IsEnabled) 
            return true;

        // Check URL Parameters - they are null if the feature is not enabled
        return CspUrlParam.EqualsInsensitive(CspConstants.CspUrlTrue) || UrlIsDevMode;
    });
    private readonly GetOnce<bool> _enabled = new();


    #endregion


    private List<KeyValuePair<string, string>> Policies => _policies.Get(Log, () =>
    {
        var sitePolicies = SiteCspSettings.Policies;
        Log.A($"Site.Policies: {sitePolicies}");

        var appPolicies = GetAppPolicies();
        var merged = $"{sitePolicies}\n{appPolicies}";
        Log.A($"Merged: {merged}");
        return new CspPolicyTextProcessor(Log).Parse(merged);
    });
    private readonly GetOnce<List<KeyValuePair<string, string>>> _policies = new();

    private string GetAppPolicies()
    {
        var cLog = Log.Fn<string>();

        var deduplicate = AppCsps
            .GroupBy(ac => ac.AppId)
            .Select(g => g.First())
            .ToList();


        var appPolicySets = deduplicate
            .Select(ac =>
            {
                var p = ac.AppPolicies;
                Log.A($"App[{ac.AppId}]: {p}");
                return p.NullIfNoValue();
            })
            .Where(p => p.HasValue())
            .ToList();

        var appPolicies = string.Join("\n", appPolicySets);

        return cLog.Return(appPolicies, $"Total: {AppCsps.Count}; Distinct: {deduplicate.Count}; With Value: {appPolicySets.Count}");
    }



    internal void AddCspService(IContentSecurityPolicyService provider) => CspServices.Add(provider);
    internal readonly List<IContentSecurityPolicyService> CspServices = [];

    public List<CspParameters> CspParameters()
    {
        var l = Log.Fn<List<CspParameters>>();
        if (!IsEnabled)
            return l.Return([], "disabled");

        if (Policies.Any())
        {
            Log.A("Policies found");
            // Create a CspService which just contains these new policies for merging later on
            var policyCsp = new ContentSecurityPolicyServiceBase();
            foreach (var policy in Policies)
                policyCsp.Add(policy.Key, policy.Value);
            AddCspService(policyCsp);
        }

        if (!CspServices.Any())
            return l.Return([], "no services to add");
        var result = CspServices
            .Select(c => c?.Policy)
            .Where(c => c != null)
            .ToList();
        return l.ReturnAsOk(result);

    }
        
}