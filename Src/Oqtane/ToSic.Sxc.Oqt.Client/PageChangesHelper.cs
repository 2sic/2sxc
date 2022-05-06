using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client
{
    internal class PageChangesHelper
    {
        internal static async Task AttachScriptsAndStyles(OqtViewResultsDto viewResults, PageState pageState, Interop interop)
        {
            // External resources = independent files (so not inline JS in the template)
            var externalResources = viewResults.TemplateResources.Where(r => r.IsExternal).ToArray();

            // 1. Style Sheets, ideally before JS
            await interop.IncludeLinks(externalResources
                .Where(r => r.ResourceType == ResourceType.Stylesheet)
                .Select(a => new
                {
                    id = string.IsNullOrWhiteSpace(a.UniqueId) ? "" : a.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                    rel = "stylesheet",
                    href = a.Url,
                    type = "text/css",
                    integrity = "",
                    crossorigin = "",
                    insertbefore = "" // bug in Oqtane, needs to be an empty string instead of null or undefined
                })
                .Cast<object>()
                .ToArray());

            // 2. Scripts - usually libraries etc.
            // Important: the IncludeClientScripts (IncludeScripts) works very different from LoadScript
            // it uses LoadJS and bundles
            var bundleId = "module-bundle-" + pageState.ModuleId;
            var includeScripts = externalResources
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
            if (includeScripts.Any()) await interop.IncludeScriptsWithAttributes(includeScripts);

            // 3. Inline JS code which was extracted from the template
            var inlineResources = viewResults.TemplateResources.Where(r => !r.IsExternal).ToArray();
            foreach (var inline in inlineResources)
                await interop.IncludeScript(string.IsNullOrWhiteSpace(inline.UniqueId) ? "" : inline.UniqueId, // bug in Oqtane, needs to be an empty string instead of null or undefined
                    "",
                    "",
                    "",
                    inline.Content,
                    "body");
        }

        public static async Task UpdatePageProperties(OqtViewResultsDto viewResults, PageState pageState, Interop interop)
        {
            // Go through Page Properties
            foreach (var p in viewResults.PageProperties)
            {
                switch (p.Property)
                {
                    case OqtPageProperties.Title:
                        var title = await interop.GetTitleValue();
                        await interop.UpdateTitle(UpdateProperty(title, p.InjectOriginalInValue(title)));
                        break;
                    case OqtPageProperties.Keywords:
                        var keywords = await interop.GetMetaTagContentByName("KEYWORDS");
                        await interop.IncludeMeta("MetaKeywords", "name", "KEYWORDS", 
                            UpdateProperty(keywords, p.InjectOriginalInValue(keywords)), "id");
                        break;
                    case OqtPageProperties.Description:
                        var description = await interop.GetMetaTagContentByName("DESCRIPTION");
                        await interop.IncludeMeta("MetaDescription", "name", "DESCRIPTION", 
                            UpdateProperty(description, p.InjectOriginalInValue(description)), "id");
                        break;
                    case OqtPageProperties.Base:
                        // For base - ignore for now as we don't know what side-effects this could have
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static string UpdateProperty(string original, OqtPagePropertyChanges change)
        {
            if (string.IsNullOrEmpty(original)) return change.Value ?? original;

            // 1. Check if we have a replacement token - if yes, try to replace it
            if (!string.IsNullOrEmpty(change.Placeholder))
            {
                var pos = original.IndexOf(change.Placeholder, StringComparison.InvariantCultureIgnoreCase);
                if (pos >= 0)
                {
                    var suffixPos = pos + change.Placeholder.Length;
                    var suffix = (suffixPos < original.Length ? original.Substring(suffixPos) : "");
                    return original.Substring(0, pos) + change.Value + suffix;
                }

                if (change.Change == OqtPagePropertyOperation.ReplaceOrSkip) return original;
            }

            // 2. If not, try to prefix / suffix / replace depending on the property
            return change.Change switch
            {
                OqtPagePropertyOperation.Replace => change.Value ?? original,
                OqtPagePropertyOperation.Suffix => $"{original}{change.Value}",
                OqtPagePropertyOperation.Prefix => $"{change.Value}{original}",
                OqtPagePropertyOperation.ReplaceOrSkip => original,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
