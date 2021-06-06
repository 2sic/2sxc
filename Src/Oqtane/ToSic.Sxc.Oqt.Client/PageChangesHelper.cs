using System.Linq;
using System.Threading.Tasks;
using Oqtane.Shared;
using Oqtane.UI;
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
                    id = string.IsNullOrWhiteSpace(a.UniqueId) ? null : a.UniqueId,
                    rel = "stylesheet",
                    href = a.Url,
                    type = "text/css"
                })
                .Cast<object>()
                .ToArray());

            // 2. Scripts - usually libraries etc.
            // Important: the IncludeScripts works very different from LoadScript - it uses LoadJS and bundles
            var bundleId = "module-bundle-" + pageState.ModuleId;
            var includeScripts = externalResources
                .Where(r => r.ResourceType == ResourceType.Script)
                .Select(a => new
                {
                    bundle = bundleId,
                    id = string.IsNullOrWhiteSpace(a.UniqueId) ? null : a.UniqueId,
                    href = a.Url,
                    location = a.Location,
                    integrity = "", // bug in Oqtane, needs to be an empty string to not throw errors
                })
                .Cast<object>()
                .ToArray();
            if (includeScripts.Any()) await interop.IncludeScripts(includeScripts);

            // 3. Inline JS code which was extracted from the template
            var inlineResources = viewResults.TemplateResources.Where(r => !r.IsExternal).ToArray();
            foreach (var inline in inlineResources)
                await interop.IncludeScript(string.IsNullOrWhiteSpace(inline.UniqueId) ? null : inline.UniqueId,
                    "",
                    "",
                    "",
                    inline.Content,
                    "body",
                    "");
        }

        public static async Task UpdatePageProperties(OqtViewResultsDto viewResults, PageState pageState, Interop interop)
        {
            // TODO: STV
            // 1. Go through Page Properties
            // 2. For base - ignore for now as we don't know what side-effects this could have
            // 3. For Title/Keywords/Description try to
            //    1. Check if we have a replacement token - if yes, try to replace it
            //    2. If not, try to prefix / suffix / replace depending on the property
            
        }
    }
}
