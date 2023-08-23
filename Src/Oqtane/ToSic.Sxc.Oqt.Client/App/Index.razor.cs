using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Models;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client.Helpers;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;
using static System.StringComparison;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index : ModuleProBase
    {
        #region Injected Services
        [Inject] public OqtSxcRenderService OqtSxcRenderService { get; set; }
        [Inject] public OqtPageChangeService OqtPageChangeService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IOqtPageChangesOnServerService OqtPageChangesOnServerService { get; set; }
        [Inject] public IOqtPrerenderService OqtPrerenderService { get; set; }

        #endregion

        #region Shared Variables

        private string RenderedUri { get; set; }
        private string RenderedPage { get; set; }
        private bool NewDataArrived { get; set; }
        public OqtViewResultsDto ViewResults { get; set; }

        public string Content { get; set; }
        
        #endregion

        #region Oqtane Properties

        public override List<Resource> Resources => new()
        {
            new Resource { ResourceType = ResourceType.Script, Url = $"Modules/{OqtConstants.PackageName}/Module.js" }
        };

        #endregion

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await base.OnParametersSetAsync();

                Log($"1: OnParametersSetAsync(NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");

                // Call 2sxc engine only when is necessary to render control.
                if (string.IsNullOrEmpty(RenderedUri) || (!NavigationManager.Uri.Equals(RenderedUri, InvariantCultureIgnoreCase) && NavigationManager.Uri.StartsWith(RenderedPage, InvariantCultureIgnoreCase)))
                {
                    RenderedUri = NavigationManager.Uri;
                    Log($"1.1: RenderUri:{RenderedUri}");
                    RenderedPage = NavigationManager.Uri.RemoveQueryAndFragment();
                    Log($"1.2: Initialize2sxcContentBlock");
                    await Initialize2SxcContentBlock();
                    NewDataArrived = true;
                }

                Log($"1 end: OnParametersSetAsync(NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await base.OnAfterRenderAsync(firstRender);

                Log($"2: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");

                // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
                if (IsSafeToRunJs && NewDataArrived && ViewResults != null)
                {
                    Log($"2.1: NewDataArrived");
                    NewDataArrived = false;

                    await StandardAssets();

                    StateHasChanged();

                    // Register ReloadModule
                    _dotNetObjectReference = DotNetObjectReference.Create(this);
                    await JSRuntime.InvokeVoidAsync($"{OqtConstants.PackageName}.registerReloadModule", _dotNetObjectReference, ModuleState.ModuleId);
                }

                Log($"2 end: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
        private DotNetObjectReference<Index> _dotNetObjectReference = null;

        // This is called from JS to reload module content from blazor instead of ajax that breaks blazor
        [JSInvokable("ReloadModule")]
        public async Task ReloadModule()
        {
            try
            {
                Log($"3: ReloadModule");
                await Initialize2SxcContentBlock();
                await StandardAssets();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public void Dispose()
        {
            _dotNetObjectReference?.Dispose();
        }
 
        /// <summary>
        /// prepare the html / headers for later rendering
        /// </summary>
        private async Task Initialize2SxcContentBlock()
        {
            var culture = CultureInfo.CurrentUICulture.Name;

            var urlQuery = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
            ViewResults = await OqtSxcRenderService.PrepareAsync(
                PageState.Alias.AliasId,
                PageState.Page.PageId,
                ModuleState.ModuleId,
                culture,
                urlQuery,
                IsPreRendering());

            if (!string.IsNullOrEmpty(ViewResults?.ErrorMessage))
                LogError(ViewResults.ErrorMessage);

            Log($"1.2.2: Html:{ViewResults?.Html?.Length ?? -1}", ViewResults);

            if (ViewResults != null)
            {
                ViewResults.SystemHtml = IsPreRendering() ? OqtPrerenderService?.GetSystemHtml() : string.Empty;
                OqtPageChangesOnServerService.ApplyHttpHeaders(ViewResults, this);
                Log($"1.3: Csp");
            }

            if (ViewResults?.PageProperties?.Any() ?? false)
            {
                Log($"1.4: UpdatePageProperties");
                HtmlHelper.UpdatePageProperties(SiteState, ViewResults, this);
            }

            // Add Context-Meta first, because it should be available when $2sxc loads
            if (ViewResults?.SxcContextMetaName != null)
            {
                Log($"1.5: Context-Meta RenderUri:{RenderedUri}");
                SiteState.Properties.HeadContent = HtmlHelper.AddOrUpdateMetaTagContent(SiteState.Properties.HeadContent,
                    ViewResults.SxcContextMetaName, ViewResults.SxcContextMetaContents);
            }

            //// Lets load all 2sxc js dependencies (js / styles)
            //var index = 0;
            //if (ViewResults?.SxcScripts != null)
            //    foreach (var resource in ViewResults.SxcScripts)
            //    {
            //        Log($"1.6.{++index}: IncludeScript:{resource}");
            //        SiteState.Properties.HeadContent = HtmlHelper.AddScript(SiteState.Properties.HeadContent, resource, SiteState.Alias);
            //    }

            Content = ViewResults?.FinalHtml;
        }

        private async Task StandardAssets()
        {
            if (ViewResults == null) return;

            #region 2sxc Standard Assets and Header

            // Lets load all 2sxc js dependencies (js / styles)
            // Not done the official Oqtane way, because that asks for the scripts before
            // the razor component reported what it needs
            if (ViewResults.SxcScripts != null)
                foreach (var resource in ViewResults.SxcScripts)
                {
                    Log($"2.3: IncludeScript:{resource}");
                    await SxcInterop.IncludeScript("", resource, "", "", "", "head");
                }

            if (ViewResults.SxcStyles != null)
                foreach (var style in ViewResults.SxcStyles)
                {
                    Log($"2.4: IncludeCss:{style}");
                    await SxcInterop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");
                }

            #endregion

            #region External resources requested by the razor template

            if (ViewResults.TemplateResources != null)
            {
                Log($"2.5: AttachScriptsAndStyles");
                await OqtPageChangeService.AttachScriptsAndStyles(ViewResults, SxcInterop, this);
            }


            #endregion
        }
    }
}
