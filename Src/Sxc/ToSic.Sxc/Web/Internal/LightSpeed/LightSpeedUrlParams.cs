using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

internal class LightSpeedUrlParams
{
    internal static (bool CachingAllowed, string Extension) GetUrlParams(LightSpeedDecorator lsConfig, IParameters pageParameters, ILog log)
    {
        var l = log.Fn<(bool, string)>();

        if (lsConfig == null) return l.Return((false, ""), "no config");
        if (lsConfig.IsEnabledNullable == false) return l.Return((false, ""), "Disabled at view level");
        if (!lsConfig.ByUrlParameters) return l.Return((true, ""), "Enabled without url parameters");

        var paramNames = !string.IsNullOrWhiteSpace(lsConfig.UrlParameterNames)
            ? lsConfig.UrlParameterNames
                .SplitNewLine()
                .Select(line => (line.Before("//") ?? line).TrimEnd())
                .ToList()
            : null;

        // Get the params, filter them new v17.10 and return the string - or exit
        var namesCsv = paramNames == null ? "" : string.Join(",", paramNames).Trim();
        if (namesCsv.Length > 0)
        {
            l.A($"View UrlParams: '{namesCsv}'");
            if (namesCsv == "*")
                l.A("All url parameters allowed - this is probably a bad idea!");
            else
            {
                var before = pageParameters;
                pageParameters = pageParameters.Filter(namesCsv);
                if (pageParameters.Count != before.Count && lsConfig.UrlParametersOthersDisableCache)
                    return l.Return((false, ""), $"Disabled because View has UrlParams, but others are not allowed. Original: {before}");
            }
        }
        else
        {
            // No names given, so we must check if there were parameters which would trigger no-cache
            if (pageParameters.Count == 0)
                return l.Return((true, ""), "Enabled since no UrlParams to respect");

            if (lsConfig.UrlParametersOthersDisableCache)
                return l.Return((false, ""), "Disabled because View has no UrlParams, so none are respected");
        }

        // Finalize URL parameters
        var urlParams = pageParameters.ToString();
        if (string.IsNullOrWhiteSpace(urlParams))
            return l.Return((true, ""), "no url params found");

        return lsConfig.UrlParametersCaseSensitive
            ? l.ReturnAndLog((true, urlParams), "case sensitive")
            : l.ReturnAndLog((true, urlParams.ToLowerInvariant()), "case insensitive");
    }
}