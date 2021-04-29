using Oqtane.Models;
using Oqtane.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared.Models;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index
    {
        [Inject]
        public ISxcOqtaneService SxcOqtaneService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public override List<Resource> Resources => new List<Resource>();

        public SxcOqtaneDto SxcOqtaneDto { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Subscribe to LocationChanged event.
            NavigationManager.LocationChanged += HandleLocationChanged;

            await Initialize2sxcContentBlock();
        }

        //protected override async Task OnParametersSetAsync()
        //{
        //    await Initialize2sxcContentBlock();

        //    //SxcOqtaneDto = SxcOqtaneService.Prepare(PageState.Alias.AliasId, PageState.Site.SiteId, PageState.Page.PageId, ModuleState.ModuleId);

        //    await base.OnParametersSetAsync();
        //}

        /// <summary>
        /// prepare the html / headers for later rendering
        /// </summary>
        private async Task Initialize2sxcContentBlock()
        {
            var urlQuery = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
            SxcOqtaneDto = await SxcOqtaneService.PrepareAsync(
                PageState.Alias.AliasId, 
                PageState.Site.SiteId, 
                PageState.Page.PageId, 
                ModuleState.ModuleId,
                QueryHelpers.ParseQuery(urlQuery));
        }

        public void Dispose() => NavigationManager.LocationChanged -= HandleLocationChanged;


        /// <summary>
        /// Handle router LocationChanged event.
        /// This is important if the url changes like /product --> /product?details=27
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleLocationChanged(object sender, LocationChangedEventArgs args) => Initialize2sxcContentBlock(); //.RunSynchronously();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);

                if (PageState.Runtime == Oqtane.Shared.Runtime.Server && SxcOqtaneDto != null/* && 1 == 0*/)
                {
                    var interop = new Interop(JSRuntime);

                    #region 2sxc Standard Assets and Header

                    // Add Context-Meta first, because it should be available when $2sxc loads

                    if (SxcOqtaneDto.AddContextMeta)
                        await interop.IncludeMeta("sxc-tmp-context-id", "name", SxcOqtaneDto.ContextMetaName, SxcOqtaneDto.ContextMetaContents, "id");

                    // Lets load all 2sxc js dependencies (js / styles)
                    // Not done the official Oqtane way, because that asks for the scripts before
                    // the razor component reported what it needs
                    foreach (var resource in SxcOqtaneDto.Scripts)
                        await interop.IncludeScript("", resource, "", "", "", "head", "");

                    foreach (var style in SxcOqtaneDto.Styles)
                        await interop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");

                    #endregion

                    #region External resources requested by the razor template

                    // External resources = independent files (so not inline JS in the template)
                    var externalResources = SxcOqtaneDto.Resources.Where(r => r.IsExternal).ToArray();

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
                    var inlineResources = SxcOqtaneDto.Resources.Where(r => !r.IsExternal).ToArray();
                    foreach (var inline in inlineResources)
                        await interop.IncludeScript("", "", "", "", inline.Content, "body", "");


                    #endregion
                }
            }
        }
    }
}
