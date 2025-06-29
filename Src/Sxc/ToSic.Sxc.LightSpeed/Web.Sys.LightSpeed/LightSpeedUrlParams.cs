﻿using ToSic.Razor.Blade;
using ToSic.Sxc.Context;
using ToSic.Sys.Caching.PiggyBack;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

internal class LightSpeedUrlParams
{
    internal static (bool CachingAllowed, string Extension) GetUrlParams(LightSpeedDecorator? lsConfig, IParameters? pageParameters, ILog log, bool usePiggyBack = true)
    {
        var l = log.Fn<(bool, string)>();

        // Preflight exit checks
        if (lsConfig == null)
            return l.Return((false, ""), "no config");
        if (lsConfig.IsEnabledNullable == false)
            return l.Return((false, ""), "Disabled at view level");
        if (!lsConfig.ByUrlParameters)
            return l.Return((true, ""), "Enabled without url parameters");
        if (pageParameters == null)
            return l.Return((false, ""), "No page parameters / context, probably an error, certainly don't cache.");

        // Get the parameter names from the config - use piggyback if possible
        var namesCsv = usePiggyBack && lsConfig.Entity is IHasPiggyBack withCache
            ? withCache.GetPiggyBack(nameof(LightSpeedUrlParams), () => ExtractConfigCsv(lsConfig))
            : ExtractConfigCsv(lsConfig);

        var result = ParseParameters(lsConfig, namesCsv, pageParameters, log);
        return l.Return(result);
    }

    private static (bool CachingAllowed, string Extension) ParseParameters(LightSpeedDecorator lsConfig, string namesCsv, IParameters pageParameters, ILog log)
    {
        var l = log.Fn<(bool, string)>();

        if (pageParameters == null)
            return l.Return((false, ""), "No page parameters / context, probably an error, certainly don't cache.");

        if (namesCsv.Length == 0)
        {
            // No names given, so we must check if there were parameters which would trigger no-cache
            if (pageParameters.Count == 0)
                return l.Return((true, ""), "Enabled since no UrlParams to respect");

            if (lsConfig.UrlParametersOthersDisableCache)
                return l.Return((false, ""), "Disabled because View has no UrlParams, so none are respected");
        }
        else
        {
            l.A($"View UrlParams: '{namesCsv}'");
            if (namesCsv == "*")
                l.A("All url parameters allowed - this is probably a bad idea!");
            else
            {
                var before = pageParameters;
                pageParameters = pageParameters.Filter(namesCsv);
                if (pageParameters.Count != before.Count && lsConfig.UrlParametersOthersDisableCache)
                    return l.Return((false, ""),
                        $"Disabled because View has UrlParams, but others are not allowed. Original: {before}");
            }
        }

        // Finalize URL parameters
        var urlParams = pageParameters.ToString(sort: true);
        if (string.IsNullOrWhiteSpace(urlParams))
            return l.Return((true, ""), "no url params found");

        return lsConfig.UrlParametersCaseSensitive
            ? l.ReturnAndLog((true, urlParams!), "case sensitive")
            : l.ReturnAndLog((true, urlParams!.ToLowerInvariant()), "case insensitive");
    }

    private static string ExtractConfigCsv(LightSpeedDecorator lsConfig)
    {
        var paramNames = string.IsNullOrWhiteSpace(lsConfig.UrlParameterNames)
            ? null
            : lsConfig.UrlParameterNames
                .SplitNewLine()
                .Select(line => (line.Before("//") ?? line).TrimEnd())
                .ToList();

        // Get the params, filter them new v17.10 and return the string - or exit
        var namesCsv = paramNames == null
            ? ""
            : string.Join(",", paramNames).Trim();
        return namesCsv;
    }
}