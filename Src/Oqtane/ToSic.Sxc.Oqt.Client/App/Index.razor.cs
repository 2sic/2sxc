using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Client;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared.Models;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index
    {
        [Inject]
        public IOqtSxcRenderService OqtSxcRenderService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private string RenderedUri { get; set; }
        private string RenderedPage { get; set; }
        private bool NewDataArrived { get; set; }


        public override List<Resource> Resources => new()
        {
            new Resource { ResourceType = ResourceType.Script, Url = "Modules/ToSic.Sxc/Module.js" }
        };

        public OqtViewResultsDto ViewResults { get; set; }

        protected override async Task OnParametersSetAsync()
        {

            // Call 2sxc engine only when is necessary to render control.
            if (string.IsNullOrEmpty(RenderedUri) || (!NavigationManager.Uri.Equals(RenderedUri, StringComparison.InvariantCultureIgnoreCase) && NavigationManager.Uri.StartsWith(RenderedPage, StringComparison.InvariantCultureIgnoreCase)))
            {
                RenderedUri = NavigationManager.Uri;
                var indexOfQuestion = NavigationManager.Uri.IndexOf("?", StringComparison.Ordinal);
                RenderedPage = indexOfQuestion > -1
                    ? NavigationManager.Uri.Substring(0, indexOfQuestion)
                    : NavigationManager.Uri;
                await Initialize2sxcContentBlock();
                NewDataArrived = true;
            }

            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// prepare the html / headers for later rendering
        /// </summary>
        private async Task Initialize2sxcContentBlock()
        {
            var culture = CultureInfo.CurrentUICulture.Name;

            var urlQuery = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
            ViewResults = await OqtSxcRenderService.PrepareAsync(
                PageState.Alias.AliasId,
                PageState.Page.PageId,
                ModuleState.ModuleId,
                culture,
                urlQuery);

            if (!string.IsNullOrEmpty(ViewResults?.ErrorMessage)) AddModuleMessage(ViewResults.ErrorMessage, MessageType.Warning);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);

                // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
                if (NewDataArrived && PageState.Runtime == Oqtane.Shared.Runtime.Server && ViewResults != null)
                {
                    NewDataArrived = false;

                    var interop = new Interop(JSRuntime);

                    #region 2sxc Standard Assets and Header

                    // Add Context-Meta first, because it should be available when $2sxc loads
                    if (ViewResults.SxcContextMetaName != null)
                        await interop.IncludeMeta("sxc-context-meta", "name", ViewResults.SxcContextMetaName, ViewResults.SxcContextMetaContents, "id");

                    // Lets load all 2sxc js dependencies (js / styles)
                    // Not done the official Oqtane way, because that asks for the scripts before
                    // the razor component reported what it needs
                    if (ViewResults.SxcScripts != null)
                        foreach (var resource in ViewResults.SxcScripts)
                            await interop.IncludeScript("", resource, "", "", "", "head", "");

                    if (ViewResults.SxcStyles != null)
                        foreach (var style in ViewResults.SxcStyles)
                            await interop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");

                    #endregion

                    #region External resources requested by the razor template

                    if (ViewResults.TemplateResources != null)
                        await PageChangesHelper.AttachScriptsAndStyles(ViewResults, PageState, interop);

                    if (ViewResults.PageProperties?.Any() ?? false)
                        await PageChangesHelper.UpdatePageProperties(ViewResults, PageState, interop);

                    #endregion
                }
            }
        }
    }
}
