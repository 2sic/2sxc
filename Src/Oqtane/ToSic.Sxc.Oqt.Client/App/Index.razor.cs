using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Services;
using static System.StringComparison;
using Runtime = Oqtane.Shared.Runtime;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index : ModuleProBase
    {
        #region Injected Services

        [Inject] public IOqtSxcRenderService OqtSxcRenderService { get; set; }
        [Inject] public IOqtPrerenderService OqtPrerenderService { get; set; }
        [Inject] public Lazy<IFeaturesService> FeaturesService { get; set; }

        #endregion

        #region Shared Variables

        private string RenderedUri { get; set; }
        private string RenderedPage { get; set; }
        private bool NewDataArrived { get; set; }
        public OqtViewResultsDto ViewResults { get; set; }

        #endregion

        #region Oqtane Properties

        public override List<Resource> Resources => new()
        {
            new Resource { ResourceType = ResourceType.Script, Url = "Modules/ToSic.Sxc/Module.js" }
        };

        #endregion

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            Log($"1: OnParametersSetAsync(Debug:{Debug},NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");
            
            // Call 2sxc engine only when is necessary to render control.
            if (string.IsNullOrEmpty(RenderedUri) || (!NavigationManager.Uri.Equals(RenderedUri, InvariantCultureIgnoreCase) && NavigationManager.Uri.StartsWith(RenderedPage, InvariantCultureIgnoreCase)))
            {
                RenderedUri = NavigationManager.Uri;
                Log($"1.1: RenderUri:{RenderedUri}");
                var indexOfQuestion = NavigationManager.Uri.IndexOf("?", Ordinal);
                RenderedPage = indexOfQuestion > -1
                    ? NavigationManager.Uri.Substring(0, indexOfQuestion)
                    : NavigationManager.Uri;
                Log($"1.2: Initialize2sxcContentBlock");
                await Initialize2SxcContentBlock();
                NewDataArrived = true;
                ViewResults.SystemHtml = OqtPrerenderService.Init(PageState, logger).GetSystemHtml();
                Csp();
                Log($"1.3: Csp");
            }
            
            Log($"1 end: OnParametersSetAsync(Debug:{Debug},NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");
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
            {
                Log($"1.2.1: ErrorMessage:{ViewResults.ErrorMessage}");
                AddModuleMessage(ViewResults.ErrorMessage, MessageType.Warning);
            }

            Log($"1.2.2: Html:{ViewResults?.Html.Length}", ViewResults);
        }

        #region CSP

        public bool ApplyCsp = true;

        private void Csp()
        {
            if (IsPreRendering() && ApplyCsp // executed only in prerender
                && (HttpContextAccessor?.HttpContext?.Request?.Path.HasValue == true)
                && !HttpContextAccessor.HttpContext.Request.Path.Value.Contains("/_blazor"))
                if (ViewResults?.CspParameters?.Any() ?? false)
                    PageChangesHelper.ApplyHttpHeaders(ViewResults, FeaturesService, HttpContextAccessor, this);

            ApplyCsp = false; // flag to ensure that code is executed only first time in prerender
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            
            Log($"2: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");

            // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
            if (IsSafeToRunJs && NewDataArrived && PageState.Runtime == Runtime.Server && ViewResults != null)
            {
                Log($"2.1: NewDataArrived");
                NewDataArrived = false;


                #region 2sxc Standard Assets and Header

                // Add Context-Meta first, because it should be available when $2sxc loads
                if (ViewResults.SxcContextMetaName != null)
                {
                    Log($"2.2: RenderUri:{RenderedUri}");
                    await SxcInterop.IncludeMeta("sxc-context-meta", "name", ViewResults.SxcContextMetaName, ViewResults.SxcContextMetaContents, "id");
                }

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
                    await PageChangesHelper.AttachScriptsAndStyles(ViewResults, PageState, SxcInterop, this);
                }

                if (ViewResults.PageProperties?.Any() ?? false)
                {
                    Log($"2.6: UpdatePageProperties");
                    await PageChangesHelper.UpdatePageProperties(ViewResults, PageState, SxcInterop, this);
                }

                StateHasChanged();

                #endregion
            }
            
            Log($"2 end: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");
        }
    }
}
