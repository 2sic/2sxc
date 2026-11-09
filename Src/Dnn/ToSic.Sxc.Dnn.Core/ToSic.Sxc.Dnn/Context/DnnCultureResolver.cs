using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using System.Web;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Eav.Context.Sys.ZoneCulture.IZoneCultureResolverExtensions;

namespace ToSic.Sxc.Dnn.Context;

internal class DnnCultureResolver(LazySvc<ISysFeaturesService> featuresSvc) : ServiceBase("Dnn.Cult")
{
    internal string? GetCurrentCultureCode(PortalSettings? portal)
    {
        var l = Log.Fn<string>();
        // First check if we know more about the site
        if (portal == null! /* paranoid */)
            return l.ReturnNull("no portal");
        var aliasCulture = portal.PortalAlias?.CultureCode ?? "";

        if (aliasCulture.HasValue())
        {
            var aliasCult = aliasCulture.ToLowerInvariant();
            return l.Return(aliasCult, $"{nameof(portal.PortalAlias)}: {aliasCult}");
        }

        // if alias is unknown, then we might be in search mode or something
        var result = portal.CultureCode?.ToLowerInvariant();
        return l.Return(result, $"Portal.CultureCode: {result}");
    }

    internal List<string>? GetCultureCodesWithFallbacks(int siteId, string currentCultureCode, string defaultCultureCode)
    {
        var l = Log.Fn<List<string>>();
        // 2023-08-31 2dm - new code, as it could contain risks, use try/catch/null to default
        try
        {
            // If the feature is not enabled, return null so up-stream can handle defaults
            if (!featuresSvc.Value.IsEnabled(BuiltInFeatures.LanguagesAdvancedFallback.Guid))
                return l.ReturnNull("feature not enabled");

            var lc = LocaleController.Instance;
            if (lc == null)
                return l.ReturnNull("no locale controller");
            var list = new List<string>();

            // Top priority is current and fallbacks of it
            // 2025-09-15 2dm: Dnn seems to do something wrong during WebApi calls.
            // Internally it wants to prefer the `language` querystring param,
            // but if it doesn't have it, then in a WebApi call it seems to not correctly take the current portal alias culture.
            // So we can't use `GetCurrentLocale` but need to use `GetLocaleOrCurrent` with the current culture code.
            var languageQuery = HttpContext.Current?.Request?.QueryString?["language"]; // null-safe when no HTTP context
            var current = !string.IsNullOrEmpty(languageQuery)
                ? lc.GetCurrentLocale(siteId) // This will use the querystring param if available, or work in Razor, but not in WebApi
                : lc.GetLocaleOrCurrent(siteId, currentCultureCode ?? defaultCultureCode ?? System.Globalization.CultureInfo.CurrentCulture.Name); // Use known codes or thread culture when no HTTP context
            if (current != null)
            {
                var currentCode = current.Code;
                l.A($"{nameof(currentCode)}: {currentCode}");
                ListBuildAddCodeIfNew(list, currentCode);

                // Try to add fallbacks, and fallbacks of fallbacks...
                var fallback = current.FallBackLocale;
                for (var i = 0; i < 3 && fallback != null; i++)
                {
                    ListBuildAddCodeIfNew(list, fallback.Code);
                    fallback = fallback.FallBackLocale;
                }
            }

            // Always add the defaults as well
            var def = lc.GetDefaultLocale(siteId);
            if (def != null)
            {
                var defCode = def.Code;
                l.A($"{nameof(defCode)}: {defCode}");
                ListBuildAddCodeIfNew(list, defCode);
                // Default should never have another fallback; it's the default!
            }

            // If the list is empty, return null so upstream can fallback
            return list.Any()
                ? l.Return(list, $"got: {list.Count}")
                : l.ReturnNull("no list");
        }
        catch
        {
            return l.ReturnAsError(null);
        }
    }
}
