using Oqtane.Models;
using Oqtane.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index
    {
        [Inject]
        public ISxcOqtane SxcEngine { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        string urlparameters;
        string querystring;
        string anchor;

        public override List<Resource> Resources => new List<Resource>();

        protected override async Task OnInitializedAsync()
        {
            (urlparameters, querystring, anchor) = Utilities.ParseParameters(NavigationManager.Uri);
            // Subscribe to LocationChanged event.
            NavigationManager.LocationChanged += HandleLocationChanged;

            // prepare the html / headers
            SxcEngine.Prepare(PageState.Site, PageState.Page, ModuleState);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= HandleLocationChanged;
        }


        /// <summary>
        /// Handle router LocationChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>

        private void HandleLocationChanged(object sender, LocationChangedEventArgs args)
        {
            (urlparameters, querystring, anchor) = Utilities.ParseParameters(args.Location);

            // prepare the html / headers
            SxcEngine.Prepare(PageState.Site, PageState.Page, ModuleState);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);

                if (PageState.Runtime == Runtime.Server)
                {
                    var interop = new Interop(JSRuntime);

                    #region 2sxc Standard Assets and Header

                    // Add Context-Meta first, because it should be available when $2sxc loads
                    var aAndH = SxcEngine.AssetsAndHeaders;
                    if (aAndH.AddContextMeta)
                        await interop.IncludeMeta("sxc-tmp-context-id", "name", aAndH.ContextMetaName, aAndH.ContextMetaContents(), "id");

                    // Lets load all 2sxc js dependencies (js / styles)
                    // Not done the official Oqtane way, because that asks for the scripts before
                    // the razor component reported what it needs
                    foreach (var resource in aAndH.Scripts())
                        await interop.IncludeScript("", resource, "", "", "", "head", "");

                    foreach (var style in aAndH.Styles())
                        await interop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");

                    #endregion

                    #region External resources requested by the razor template

                    // External resources = independent files (so not inline JS in the template)
                    var externalResources = SxcEngine.Resources.Where(r => r.IsExternal).ToArray();

                    // 1. Style Sheets, ideally before JS
                    await interop.IncludeLinks(externalResources
                        .Where(r => r.ResourceType == ResourceType.Stylesheet)
                        .Select(a => new { rel = "stylesheet", href = a.Url, type = "text/css" })
                        .Cast<object>()
                        .ToArray());

                    // 2. Scripts - usually libraries etc.
                    await interop.IncludeScripts(externalResources
                        .Where(r => r.ResourceType == ResourceType.Script)
                        .Select(a => new { href = a.Url, location = a.Location })
                        .Cast<object>()
                        .ToArray());

                    // 3. Inline JS code which was extracted from the template
                    var inlineResources = SxcEngine.Resources.Where(r => !r.IsExternal).ToArray();
                    foreach (var inline in inlineResources)
                        await interop.IncludeScript("", "", "", "", inline.Content, "body", "");


                    #endregion
                }
            }
        }
    }
}
