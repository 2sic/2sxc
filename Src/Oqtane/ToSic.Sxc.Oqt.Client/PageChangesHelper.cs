using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        internal static async Task AttachScriptsAndStyles(OqtViewResultsDto viewResults, PageState pageState, Interop interop, ModuleProBase page)
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
            await Log(page, $"{logPrefix}CSS: {css.Length}", css);
            await interop.IncludeLinks(css);

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
            await Log(page, $"{logPrefix}Scripts: {scripts.Length}", scripts);
            if (scripts.Any())
                await interop.IncludeScriptsWithAttributes(scripts);

            // 3. Inline JS code which was extracted from the template
            var inlineResources = viewResults.TemplateResources.Where(r => !r.IsExternal).ToArray();
            // Log inline
            await Log(page, $"{logPrefix}Inline: {inlineResources.Length}", inlineResources);
            foreach (var inline in inlineResources)
                await interop.IncludeScript(string.IsNullOrWhiteSpace(inline.UniqueId) ? "" : inline.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                    "",
                    "",
                    "",
                    inline.Content,
                    "body");
        }

        /// <summary>
        /// Log something to the page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="data"></param>
        private static async Task Log(ModuleProBase page, params object[] data)
        {
            if (page != null)
                await page.Log(data);
        }


        public static async Task UpdatePageProperties(OqtViewResultsDto viewResults, PageState pageState, Interop interop, ModuleProBase page)
        {
            var logPrefix = $"{nameof(UpdatePageProperties)}(...) - ";

            // Go through Page Properties
            foreach (var p in viewResults.PageProperties)
            {
                switch (p.Property)
                {
                    case OqtPageProperties.Title:
                        var title = await interop.GetTitleValue();
                        await Log(page, $"{logPrefix}UpdateTitle:", title);
                        await interop.UpdateTitle(await UpdateProperty(title, p.InjectOriginalInValue(title), page));
                        break;
                    case OqtPageProperties.Keywords:
                        var keywords = await interop.GetMetaTagContentByName("KEYWORDS");
                        await Log(page, $"{logPrefix}Keywords:", keywords);
                        await interop.IncludeMeta("MetaKeywords", "name", "KEYWORDS",
                            await UpdateProperty(keywords, p.InjectOriginalInValue(keywords), page), "id");
                        break;
                    case OqtPageProperties.Description:
                        var description = await interop.GetMetaTagContentByName("DESCRIPTION");
                        await Log(page, $"{logPrefix}Description:", description);
                        await interop.IncludeMeta("MetaDescription", "name", "DESCRIPTION",
                            await UpdateProperty(description, p.InjectOriginalInValue(description), page), "id");
                        break;
                    case OqtPageProperties.Base:
                        // For base - ignore for now as we don't know what side-effects this could have
                        await Log(page, $"{logPrefix}Base ignore for now");
                        break;
                    default:
                        await Log(page, $"{logPrefix} ArgumentOutOfRangeException");
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static async Task<string> UpdateProperty(string original, OqtPagePropertyChanges change, ModuleProBase page)
        {
            var logPrefix = $"{nameof(UpdateProperty)}(original:{original}) - ";

            if (string.IsNullOrEmpty(original))
            {
                var result = change.Value ?? original;
                await Log(page, $"{logPrefix}is empty, UpdateTitle:{result}");
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
                    await Log(page, $"{logPrefix}token replaced, UpdateTitle:{result2}");
                    return result2;
                }

                await Log(page, $"{logPrefix}replace token not found, UpdateTitle:{original}");
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
            await Log(page, $"{logPrefix}{change.Change}, UpdateTitle:{result3}");
            return result3;
        }

        public static async Task<int> ApplyHttpHeaders(OqtViewResultsDto result, Lazy<IFeaturesService> featuresService, IHttpContextAccessor httpContextAccessor, ModuleProBase page)
        {
            var logPrefix = $"{nameof(ApplyHttpHeaders)}(...) - ";

            // Register CSP changes for applying once all modules have been prepared
            // Note that in cached scenarios, CspEnabled is true, but it may have been turned off since
            if (result.CspEnabled && featuresService.Value.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId))
            {
                await Log(page, $"{logPrefix}Register CSP changes");
                PageCsp(result.CspEnforced, httpContextAccessor, page).Add(result.CspParameters.Select(p => new CspParameters(UrlHelpers.ParseQueryString(p))).ToList());
            }

            var httpHeaders = result.HttpHeaders;

            if (httpContextAccessor.HttpContext?.Response == null)
            {
                await Log(page, $"{logPrefix}error, HttpResponse is null");
                return 0;
            }

            if (httpContextAccessor.HttpContext.Response.HasStarted)
            {
                await Log(page, $"{logPrefix}error, to late for adding http headers");
                return 0;
            }

            if (httpHeaders?.Any() != true)
            {
                await Log(page, $"{logPrefix}ok, no headers to add");
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
                        await Log(page, $"{logPrefix}{httpHeader.Name}={httpHeader.Value}");
                    }
                }
                catch (Exception e)
                {
                    await Log(page, $"{logPrefix}Exception: {e.Message}");
                }
            });
            await Log(page, $"{logPrefix}httpHeaders.Count: {httpHeaders.Count}");
            return httpHeaders.Count;
        }

        private static CspOfPage PageCsp(bool enforced, IHttpContextAccessor httpContextAccessor, ModuleProBase page)
        {
            //var logPrefix = $"{nameof(PageCsp)}(enforced:{enforced}) - ";

            var key = "2sxcPageLevelCsp";

            // If it's already registered, then the add-on-sending has already been added too
            // So we shouldn't repeat it, just return the cache which will be used later
            if (httpContextAccessor.HttpContext.Items.ContainsKey(key))
            {
                var result = (CspOfPage)httpContextAccessor.HttpContext.Items[key];
                //Log(page, $"already registered {logPrefix}{key}={result}").ConfigureAwait(false);
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
                            //Log(page, $"{logPrefix}have headers {key}={headers}").ConfigureAwait(false);
                        }

                    }
                    catch (Exception e)
                    {
                        //Log(page, $"{logPrefix}Exception: {e.Message}").ConfigureAwait(false);
                    }

                    return Task.CompletedTask;
                });
            //Log(page, $"not yet registered {logPrefix}{key}={pageLevelCsp}").ConfigureAwait(false);
            return pageLevelCsp;
        }
    }
}
