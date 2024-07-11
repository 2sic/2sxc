using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Helpers;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtPageChangeService(IOqtTurnOnService turnOnService, CacheBustingService noCache)
{
    /// <summary>
    /// Ensure standard assets like java-scripts and styles.
    /// </summary>
    /// <param name="viewResults"></param>
    /// <param name="siteState"></param>
    /// <param name="content"></param>
    /// <param name="themeName"></param>
    /// <returns></returns>
    /// <remarks>SSR only</remarks>
    public string AttachScriptsAndStylesStaticallyInHtml(OqtViewResultsDto viewResults, SiteState siteState, string content, string themeName)
    {
        siteState.Properties.HeadContent = HtmlHelper.ManageStyleSheets(siteState.Properties.HeadContent, viewResults, siteState.Alias, themeName);
        //content = HtmlHelper.ManageStyleSheets(content, viewResults, siteState.Alias, themeName);
        //siteState.Properties.HeadContent = HtmlHelper.ManageScripts(siteState.Properties.HeadContent, viewResults, siteState.Alias);
        content = HtmlHelper.ManageScripts(content, viewResults, siteState.Alias);
        return HtmlHelper.ManageInlineScripts(content, viewResults, siteState.Alias);
    }

    public string AttachScriptsAndStylesDynamicallyWithTurnOn(OqtViewResultsDto viewResults, SiteState siteState, string content, string themeName, Guid renderId)
    {
        if (viewResults == null) return content;

        var scripts = new List<object>();
        var links = new List<object>();
        var inlineScripts = new List<object>();

        #region 2sxc Standard Assets and Header

        // Load all 2sxc js dependencies (js / styles)
        // Not done the official Oqtane way, because that asks for the scripts before
        // the razor component reported what it needs
        if (viewResults.SxcScripts != null)
        {
            scripts.AddRange(viewResults.SxcScripts.Select(a => new
            {
                href = noCache.CacheBusting(a, renderId),
                bundle = "", // not working when bundleId is provided
                id = "",
                location = "body",
                //htmlAttributes = null,
                integrity = "", // bug in Oqtane, needs to be an empty string to not throw errors
                crossorigin = "",
            }));
        }

        if (viewResults.SxcStyles != null)
        {
            links.AddRange(viewResults.SxcStyles.Select(link => new
            {
                id = "",
                rel= "stylesheet",
                href = link,
                type = "text/css",
                integrity = "",
                crossorigin = "",
                insertbefore = "app-stylesheet-module"
            }));
        }

        #endregion

        #region External resources requested by the razor template

        if (viewResults.TemplateResources != null)
        {
            // External resources = independent files (so not inline JS in the template)
            var externalResources = viewResults.TemplateResources.Where(r => r.IsExternal).ToArray();

            links.AddRange(externalResources.Where(r => r.ResourceType == ResourceType.Stylesheet).Select(link => new
            {
                id = string.IsNullOrWhiteSpace(link.UniqueId) ? "" : link.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                rel = "stylesheet",
                href = link.Url,
                type = "text/css",
                integrity = "",
                crossorigin = "",
                insertbefore = "app-stylesheet-module" // bug in Oqtane, needs to be an empty string instead of null or undefined
            }));

            // 2. Scripts - usually libraries etc.
            // Important: the IncludeClientScripts (IncludeScripts) works very different from LoadScript
            // it uses LoadJS and bundles

            scripts.AddRange(externalResources.Where(r => r.ResourceType == ResourceType.Script).Select(script => new
            {
                id = string.IsNullOrWhiteSpace(script.UniqueId) ? "" : script.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                href = noCache.CacheBusting(script.Url, renderId),
                bundle = "", // not working when bundleId is provided
                location = "body", // script.Location,
                htmlAttributes = script.HtmlAttributes,
                integrity = script.Integrity ?? "", // bug in Oqtane, needs to be an empty string to not throw errors
                crossorigin = script.CrossOrigin ?? "",
            }));

            // 3. Inline JS code which was extracted from the template
            var inlineResources = viewResults.TemplateResources.Where(r => !r.IsExternal).ToArray();
            inlineScripts.AddRange(inlineResources.Select(inline => new
            {
                id = string.IsNullOrWhiteSpace(inline.UniqueId) ? "" : inline.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                src = "",
                integrity = "",
                crossorigin = "",
                type = "text/javascript",
                content = inline.Content,
                location = "body"
            }));
        }
        #endregion

        //// ensure that tun-on is loaded
        //var turnOnScript = $"<page-script src='./Modules/ToSic.Sxc.Oqtane/dist/turnOn/turn-on.js'></page-script>";

        //content += turnOnScript + Environment.NewLine;

        if (links.Any())
            content += turnOnService.Run("window.Oqtane.Interop.includeLinks()", data: links.ToArray()) + Environment.NewLine;

        if (scripts.Any()) 
            content += turnOnService.Run("window.Oqtane.Interop.includeScripts()", data: scripts.ToArray()) + Environment.NewLine;

        if (inlineScripts.Any())
            content += turnOnService.Run("window.ToSic.Sxc.Oqtane.includeInlineScripts()", data: inlineScripts.ToArray()) + Environment.NewLine;

        //content += turnOnService.Run("window.ToSic.Sxc.Oqtane.reExecuteScripts()") + Environment.NewLine;

        return content;
    }

    /// <summary>
    /// Ensure standard assets like java-scripts and styles.
    /// This is done with interop after the page is rendered.
    /// </summary>
    /// <returns></returns>
    public async Task AttachScriptsAndStylesForInteractiveRendering(OqtViewResultsDto viewResults, SxcInterop sxcInterop, IOqtHybridLog page)
    {
        var logPrefix = $"{nameof(AttachScriptsAndStylesForInteractiveRendering)}(...) - ";

        if (viewResults == null) return;

        // Add Context-Meta first, because it should be available when $2sxc loads
        if (viewResults?.SxcContextMetaName != null)
        {
            page?.Log($"2.2: Context-Meta");
            await sxcInterop.IncludeMeta("", "name", viewResults.SxcContextMetaName, viewResults.SxcContextMetaContents);
        }

        #region 2sxc Standard Assets and Header

        // Lets load all 2sxc js dependencies (js / styles)
        // Not done the official Oqtane way, because that asks for the scripts before
        // the razor component reported what it needs
        if (viewResults.SxcScripts != null)
            foreach (var resource in viewResults.SxcScripts)
            {
                page?.Log($"2.3: IncludeScript:{resource}");
                await sxcInterop.IncludeScript("", resource, "", "", "", "head");
            }

        if (viewResults.SxcStyles != null)
            foreach (var style in viewResults.SxcStyles)
            {
                page?.Log($"2.4: IncludeCss:{style}");
                await sxcInterop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");
            }

        #endregion

        #region External resources requested by the razor template

        if (viewResults.TemplateResources != null)
        {
            page?.Log($"2.5: AttachScriptsAndStylesForInteractiveRendering");
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
            if (css.Any())
                await sxcInterop.IncludeLinks(css);

            // 2. Scripts - usually libraries etc.
            // Important: the IncludeClientScripts (IncludeScripts) works very different from LoadScript
            // it uses LoadJS and bundles
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

        #endregion
    }


    /// <summary>
    /// Process page changes in html for title, keywords, descriptions and other meta or html tags.
    /// Oqtane decide to directly render this changes in page html as part of first request, or later with interop.
    /// </summary>
    /// <param name="viewResults"></param>
    /// <param name="siteState"></param>
    /// <param name="page"></param>
    public string ProcessPageChanges(OqtViewResultsDto viewResults, SiteState siteState, ModuleProBase page)
    {
        page?.Log($"1.3: ProcessPageChanges");

        if (viewResults?.PageProperties?.Any() ?? false)
        {
            page?.Log($"1.3.2: UpdatePageProperties title, keywords, description");
            UpdatePageProperties(siteState, viewResults, page);
        }

        if (viewResults?.HeadChanges?.Any() ?? false)
        {
            page?.Log($"1.3.3: AddHeadChanges:{viewResults.HeadChanges.Count()}");
            siteState.Properties.HeadContent = HtmlHelper.AddHeadChanges(siteState.Properties.HeadContent, viewResults.HeadChanges);
        }

        // Add Context-Meta first, because it should be available when $2sxc loads
        if (viewResults?.SxcContextMetaName != null)
        {
            page?.Log($"1.3.4: Context-Meta");
            siteState.Properties.HeadContent = HtmlHelper.AddOrUpdateMetaTagContent(siteState.Properties.HeadContent, viewResults.SxcContextMetaName, viewResults.SxcContextMetaContents, false);
        }

        //// Lets load all 2sxc js dependencies (js / styles)
        //var index = 0;
        //if (ViewResults?.SxcScripts != null)
        //    foreach (var resource in ViewResults.SxcScripts)
        //    {
        //        Log($"1.3.4.{++index}: IncludeScript:{resource}");
        //        SiteState.Properties.HeadContent = HtmlHelper.AddScript(SiteState.Properties.HeadContent, resource, SiteState.Alias);
        //    }

        page?.Log($"1.3.1: module html content set on page");
        return viewResults?.FinalHtml;
    }

    private void UpdatePageProperties(SiteState siteState, OqtViewResultsDto viewResults, ModuleProBase page)
    {
        var logPrefix = $"{nameof(UpdatePageProperties)}(...) - ";

        // Go through Page Properties
        foreach (var p in viewResults.PageProperties)
        {
            switch (p.Property)
            {
                case OqtPageProperties.Title:
                    var currentTitle = siteState.Properties.PageTitle;
                    var updatedTitle = UpdateProperty(currentTitle, p.InjectOriginalInValue(currentTitle), page);
                    page?.Log($"{logPrefix}UpdateTitle:", updatedTitle);
                    siteState.Properties.PageTitle = updatedTitle;
                    break;
                case OqtPageProperties.Keywords:
                    var currentKeywords = HtmlHelper.GetMetaTagContent(siteState.Properties.HeadContent, "KEYWORDS");
                    var updatedKeywords = UpdateProperty(currentKeywords, p.InjectOriginalInValue(currentKeywords), page);
                    page?.Log($"{logPrefix}Keywords:", updatedKeywords);
                    siteState.Properties.HeadContent = HtmlHelper.AddOrUpdateMetaTagContent(siteState.Properties.HeadContent, "KEYWORDS", updatedKeywords);
                    break;
                case OqtPageProperties.Description:
                    var currentDescription = HtmlHelper.GetMetaTagContent(siteState.Properties.HeadContent, "DESCRIPTION");
                    var updatedDescription = UpdateProperty(currentDescription, p.InjectOriginalInValue(currentDescription), page);
                    page?.Log($"{logPrefix}Description:", updatedDescription);
                    siteState.Properties.HeadContent = HtmlHelper.AddOrUpdateMetaTagContent(siteState.Properties.HeadContent, "DESCRIPTION", updatedDescription);
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

    private string UpdateProperty(string original, OqtPagePropertyChanges change, IOqtHybridLog page)
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
                var suffix = suffixPos < original.Length ? original.Substring(suffixPos) : "";
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
}