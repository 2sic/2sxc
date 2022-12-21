using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.Url;
using BuiltInFeatures = ToSic.Sxc.Configuration.Features.BuiltInFeatures;

namespace ToSic.Sxc.Oqt.Client
{
    internal class PageChangesHelper
    {
        internal static async Task AttachScriptsAndStyles(OqtViewResultsDto viewResults, PageState pageState, SxcInterop sxcInterop, ModuleProBase page)
        {
            var logPrefix = $"{nameof(AttachScriptsAndStyles)}(...) - ";

            // External resources = independent files (so not inline JS in the template)
            var externalResources = viewResults.TemplateResources.Where(r => r.IsExternal).ToArray();

            // 1. Style Sheets, ideally before JS
            var css = externalResources
                .Where(r => r.ResourceType == ResourceType.Stylesheet)
                .Select(a => new
                {
                    id = string.IsNullOrWhiteSpace(a.UniqueId)
                        ? ""
                        : a.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                    rel = "stylesheet",
                    href = a.Url,
                    type = "text/css",
                    integrity = "",
                    crossorigin = "",
                    insertbefore = "" // bug in Oqtane, needs to be an empty string instead of null or undefined
                })
                .Cast<object>()
                .ToArray();

            // Log CSS and then add to page
            page?.Log($"{logPrefix}CSS: {css.Length}", css);
            await sxcInterop.IncludeLinks(css);

            // 2. Scripts - usually libraries etc.
            // Important: the IncludeClientScripts (IncludeScripts) works very different from LoadScript
            // it uses LoadJS and bundles
            var bundleId = "module-bundle-" + pageState.ModuleId;
            var scripts = externalResources
                .Where(r => r.ResourceType == ResourceType.Script)
                .Select(a => new
                {
                    href = a.Url,
                    bundle = "", // not working when bundleId is provided
                    id = string.IsNullOrWhiteSpace(a.UniqueId) ? "" : a.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                    location = a.Location,
                    htmlAttributes = a.HtmlAttributes,
                    integrity = a.Integrity ?? "", // bug in Oqtane, needs to be an empty string to not throw errors
                    crossorigin = a.CrossOrigin ?? "",
                })
                .Cast<object>()
                .ToArray();

            // Log scripts and then add to page
            page?.Log($"{logPrefix}Scripts: {scripts.Length}", scripts);
            if (scripts.Any())
                await sxcInterop.IncludeScriptsWithAttributes(scripts);

            // 3. Inline JS code which was extracted from the template
            var inlineResources = viewResults.TemplateResources.Where(r => !r.IsExternal).ToArray();
            // Log inline
            page?.Log($"{logPrefix}Inline: {inlineResources.Length}", inlineResources);
            foreach (var inline in inlineResources)
                await sxcInterop.IncludeScript(string.IsNullOrWhiteSpace(inline.UniqueId) ? "" : inline.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                    "",
                    "",
                    "",
                    inline.Content,
                    "body");
        }


        public static async Task UpdatePageProperties(OqtViewResultsDto viewResults, PageState pageState, SxcInterop sxcInterop, ModuleProBase page)
        {
            var logPrefix = $"{nameof(UpdatePageProperties)}(...) - ";

            // Go through Page Properties
            foreach (var p in viewResults.PageProperties)
            {
                switch (p.Property)
                {
                    case OqtPageProperties.Title:
                        var title = await sxcInterop.GetTitleValue();
                        page?.Log($"{logPrefix}UpdateTitle:", title);
                        await sxcInterop.UpdateTitle(UpdateProperty(title, p.InjectOriginalInValue(title), page));
                        break;
                    case OqtPageProperties.Keywords:
                        var keywords = await sxcInterop.GetMetaTagContentByName("KEYWORDS");
                        page?.Log($"{logPrefix}Keywords:", keywords);
                        await sxcInterop.IncludeMeta("MetaKeywords", "name", "KEYWORDS",
                            UpdateProperty(keywords, p.InjectOriginalInValue(keywords), page), "id");
                        break;
                    case OqtPageProperties.Description:
                        var description = await sxcInterop.GetMetaTagContentByName("DESCRIPTION");
                        page?.Log($"{logPrefix}Description:", description);
                        await sxcInterop.IncludeMeta("MetaDescription", "name", "DESCRIPTION",
                            UpdateProperty(description, p.InjectOriginalInValue(description), page), "id");
                        break;
                    case OqtPageProperties.Base:
                        // For base - ignore for now as we don't know what side-effects this could have
                        page?.Log($"{logPrefix}Base ignore for now");
                        break;
                    default:
                        page?.Log($"{logPrefix} ArgumentOutOfRangeException");
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static string UpdateProperty(string original, OqtPagePropertyChanges change, ModuleProBase page)
        {
            var logPrefix = $"{nameof(UpdateProperty)}(original:{original}) - ";

            if (string.IsNullOrEmpty(original))
            {
                var result = change.Value ?? original;
                page?.Log($"{logPrefix}is empty, UpdateTitle:{result}");
                return result;
            };

            // 1. Check if we have a replacement token - if yes, try to replace it
            if (!string.IsNullOrEmpty(change.Placeholder))
            {
                var pos = original.IndexOf(change.Placeholder, StringComparison.InvariantCultureIgnoreCase);
                if (pos >= 0)
                {
                    var suffixPos = pos + change.Placeholder.Length;
                    var suffix = (suffixPos < original.Length ? original.Substring(suffixPos) : "");
                    var result2 = original.Substring(0, pos) + change.Value + suffix;
                    page?.Log($"{logPrefix}token replaced, UpdateTitle:{result2}");
                    return result2;
                }

                page?.Log($"{logPrefix}replace token not found, UpdateTitle:{original}");
                if (change.Change == OqtPagePropertyOperation.ReplaceOrSkip) return original;
            }

            // 2. If not, try to prefix / suffix / replace depending on the property
            var result3 = change.Change switch
            {
                OqtPagePropertyOperation.Replace => change.Value ?? original,
                OqtPagePropertyOperation.Suffix => $"{original}{change.Value}",
                OqtPagePropertyOperation.Prefix => $"{change.Value}{original}",
                OqtPagePropertyOperation.ReplaceOrSkip => original,
                _ => throw new ArgumentOutOfRangeException()
            };
            page?.Log($"{logPrefix}{change.Change}, UpdateTitle:{result3}");
            return result3;
        }

        public static int ApplyHttpHeaders(OqtViewResultsDto result, ILazySvc<IFeaturesService> featuresService, IHttpContextAccessor httpContextAccessor, ModuleProBase page)
        {
            var logPrefix = $"{nameof(ApplyHttpHeaders)}(...) - ";

            page?.Log($"{logPrefix}stv#1 poc");
            // Register CSP changes for applying once all modules have been prepared
            // Note that in cached scenarios, CspEnabled is true, but it may have been turned off since
            if (result.CspEnabled && featuresService.Value.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId))
            {
                page?.Log($"{logPrefix}Register CSP changes");
                PageCsp(result.CspEnforced, httpContextAccessor, page).Add(result.CspParameters.Select(p => new CspParameters(UrlHelpers.ParseQueryString(p))).ToList());
            }

            var httpHeaders = result.HttpHeaders;

            if (httpContextAccessor.HttpContext?.Response == null)
            {
                page?.Log($"{logPrefix}error, HttpResponse is null");
                return 0;
            }

            if (httpContextAccessor.HttpContext.Response.HasStarted)
            {
                page?.Log($"{logPrefix}error, to late for adding http headers");
                return 0;
            }

            if (httpHeaders?.Any() != true)
            {
                page?.Log($"{logPrefix}ok, no headers to add");
                return 0;
            }

            // Register event to attach headers
            httpContextAccessor.HttpContext.Response.OnStarting(async () =>
            {
                try
                {
                    foreach (var httpHeader in httpHeaders)
                    {
                        if (string.IsNullOrWhiteSpace(httpHeader.Name)) continue;

                        // TODO: The CSP header can only exist once
                        // So to do this well, we'll need to merge them in future, 
                        // Ideally combining the existing one with any additional ones added here
                        httpContextAccessor.HttpContext.Response.Headers[httpHeader.Name] = httpHeader.Value;
                        page?.Log($"{logPrefix}{httpHeader.Name}={httpHeader.Value}");
                    }
                }
                catch (Exception e)
                {
                    page?.Log($"{logPrefix}Exception: {e.Message}");
                }
            });
            page?.Log($"{logPrefix}httpHeaders.Count: {httpHeaders.Count}");
            return httpHeaders.Count;
        }

        private static CspOfPage PageCsp(bool enforced, IHttpContextAccessor httpContextAccessor, ModuleProBase page)
        {
            var logPrefix = $"{nameof(PageCsp)}(enforced:{enforced}) - ";

            var key = "2sxcPageLevelCsp";

            // If it's already registered, then the add-on-sending has already been added too
            // So we shouldn't repeat it, just return the cache which will be used later
            if (httpContextAccessor.HttpContext.Items.ContainsKey(key))
            {
                var result = (CspOfPage)httpContextAccessor.HttpContext.Items[key];
                page?.Log($"already registered {logPrefix}{key}={result}");
                return result;
            }

            // Not yet registered. Create, and register for on-end of request
            var pageLevelCsp = new CspOfPage();
            httpContextAccessor.HttpContext.Items[key] = pageLevelCsp;

            // Register event to attach headers once the request is done and all Apps have registered their Csp
            if (!httpContextAccessor.HttpContext.Response.HasStarted)
                httpContextAccessor.HttpContext.Response.OnStarting(() =>
                {
                    try
                    {
                        var headers = pageLevelCsp.CspHttpHeader();
                        if (headers != null)
                        {
                            var key = pageLevelCsp.HeaderName(enforced);
                            httpContextAccessor.HttpContext.Response.Headers[key] = headers;
                            page?.Log($"{logPrefix}have headers {key}={headers}");
                        }

                    }
                    catch (Exception e)
                    {
                        page?.Log($"{logPrefix}Exception: {e.Message}");
                    }

                    return Task.CompletedTask;
                });
            page?.Log($"not yet registered {logPrefix}{key}={pageLevelCsp}");
            return pageLevelCsp;
        }
    }
}
