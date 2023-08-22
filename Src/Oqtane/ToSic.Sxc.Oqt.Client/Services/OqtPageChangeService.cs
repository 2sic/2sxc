using Oqtane.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;
using Oqtane.UI;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
  public class OqtPageChangeService
  {
      private readonly SiteState _siteState;

      public OqtPageChangeService(SiteState siteState)
      {
          _siteState = siteState;
      }
      
      public async Task AttachScriptsAndStyles(OqtViewResultsDto viewResults, SxcInterop sxcInterop, IOqtHybridLog page)
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
        public async Task UpdatePageProperties(OqtViewResultsDto viewResults, SxcInterop sxcInterop, ModuleProBase page)
        {
            var logPrefix = $"{nameof(UpdatePageProperties)}(...) - ";

            // Go through Page Properties
            foreach (var p in viewResults.PageProperties)
            {
                switch (p.Property)
                {
                    case OqtPageProperties.Title:
                        //var title = await sxcInterop.GetTitleValue();
                        //page?.Log($"{logPrefix}UpdateTitle:", title);
                        //// await sxcInterop.UpdateTitle(UpdateProperty(title, p.InjectOriginalInValue(title), page));
                        //_siteState.Properties.PageTitle = UpdateProperty(title, p.InjectOriginalInValue(title), page);
                        break;
                    case OqtPageProperties.Keywords:
                        var keywords = await sxcInterop.GetMetaTagContentByName("KEYWORDS");
                        page?.Log($"{logPrefix}Keywords:", keywords);
                        await sxcInterop.IncludeMeta("MetaKeywords", "name", "KEYWORDS",
                            UpdateProperty(keywords, p.InjectOriginalInValue(keywords), page)/*, "id"*/);// Oqtane.client 3.3.1
                        break;
                    case OqtPageProperties.Description:
                        var description = await sxcInterop.GetMetaTagContentByName("DESCRIPTION");
                        page?.Log($"{logPrefix}Description:", description);
                        await sxcInterop.IncludeMeta("MetaDescription", "name", "DESCRIPTION",
                            UpdateProperty(description, p.InjectOriginalInValue(description), page)/*, "id"*/);// Oqtane.client 3.3.1
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
        public string UpdateProperty(string original, OqtPagePropertyChanges change, IOqtHybridLog page)
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
}
