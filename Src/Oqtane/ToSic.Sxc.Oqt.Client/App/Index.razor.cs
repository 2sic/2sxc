using System;
using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Client.Shared;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;

using static System.StringComparison;
using Microsoft.AspNetCore.Components.Rendering;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index : ModuleProBase
    {
        #region Injected Services

        [Inject] public IOqtSxcRenderService OqtSxcRenderService { get; set; }
        [Inject] public IOqtPrerenderService OqtPrerenderService { get; set; }
        [Inject] public IOqtPageChangeService OqtPageChangeService { get; set; }
        

        #endregion

        #region Shared Variables

        private string RenderedUri { get; set; }
        private string RenderedPage { get; set; }
        private bool NewDataArrived { get; set; }
        public OqtViewResultsDto ViewResults { get; set; }

        public string Content { get; set; }

        public ElementReference ParentElement { get; set; }
        public ElementReference ChildElement { get; set; }

        public RenderFragment ParentFragment => builder =>
        {
            //var childComponent = new MyComponent
            //{
            //    Content = Content
            //};

            builder.OpenElement(0, "span");
            builder.AddElementReferenceCapture(1, elementReference => ParentElement = elementReference);
            builder.AddContent(2, ChildFragment);
            //builder.AddComponentReferenceCapture(3, instance => { });
            builder.CloseElement();
        };

        public RenderFragment ChildFragment => builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddElementReferenceCapture(1, elementReference => ChildElement = elementReference);
            builder.AddMarkupContent(2, Content);
            builder.CloseElement();
        };

        //public class MyComponent : ComponentBase
        //{
        //    [Parameter]
        //    public string Content { get; set; }
        //    protected override void BuildRenderTree(RenderTreeBuilder builder)
        //    {
        //        builder.OpenElement(0, "span");
        //        builder.AddMarkupContent(1, Content);
        //        builder.CloseElement();
        //    }
        //}

        #endregion

        #region Oqtane Properties

        public override List<Resource> Resources => new()
        {
            new Resource { ResourceType = ResourceType.Script, Url = $"Modules/{OqtConstants.PackageName}/Module.js" }
        };

        #endregion

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var hash = NewHash();
            Log($"1: OnParametersSetAsync(NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})", $"hash:{hash}");

            // Call 2sxc engine only when is necessary to render control.
            if (string.IsNullOrEmpty(RenderedUri) || (!NavigationManager.Uri.Equals(RenderedUri, InvariantCultureIgnoreCase) && NavigationManager.Uri.StartsWith(RenderedPage, InvariantCultureIgnoreCase)))
            {
                RenderedUri = NavigationManager.Uri;
                Log($"1.1: RenderUri:{RenderedUri}", $"hash:{hash}");
                RenderedPage = NavigationManager.Uri.RemoveQueryAndFragment();
                Log($"1.2: Initialize2sxcContentBlock", $"hash:{hash}");
                var dummy = await GetViewResults(LogHash);
                NewDataArrived = true;

                Csp();
                Log($"1.3: Csp", $"hash:{hash}");
            }
            
            Log($"1 end: OnParametersSetAsync(NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})", $"hash:{hash}");
        }

        /// <summary>
        /// prepare the html / headers for later rendering
        /// </summary>
        private async Task<string> GetViewResults(string hash)
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
                Log($"1.2.1: ErrorMessage:{ViewResults.ErrorMessage}", $"hash:{hash}");
                AddModuleMessage(ViewResults.ErrorMessage, MessageType.Warning);
            }

            Log($"1.2.2: Html:{ViewResults?.Html.Length}", ViewResults, $"hash:{hash}");
            if (!string.IsNullOrEmpty(ViewResults?.Html))
                ViewResults.SystemHtml = OqtPrerenderService.Init(PageState, logger).GetSystemHtml();
            
            return string.IsNullOrEmpty(ViewResults?.Html) ? string.Empty : ViewResults?.FinalHtml;
        }

        #region CSP

        public bool ApplyCsp = true;

        private void Csp()
        {
            if (IsPreRendering() && ApplyCsp) // executed only in prerender
                OqtPageChangeService.ApplyHttpHeaders(ViewResults, this);

            ApplyCsp = false; // flag to ensure that code is executed only first time in prerender
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var hash = LogHash;

            Log($"2: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})", $"hash:{hash}");

            // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
            if (IsSafeToRunJs && NewDataArrived && ViewResults != null)
            {
                Log($"2.1: NewDataArrived", $"hash:{hash}");
                NewDataArrived = false;

                try
                {
                    Content = ViewResults.FinalHtml;



                    await StandardPageAssets(hash);

                    StateHasChanged();
                }
                catch (Exception e)
                {
                    Log($"2.1.1: Error:{e.Message}", $"hash:{hash}");
                    AddModuleMessage(e.Message, MessageType.Error);
                    NavigationManager.NavigateTo(NavigateUrl(), true);
                }

            }

            Log($"2 end: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})", $"hash:{hash}");
        }

        private async Task StandardPageAssets(string hash)
        {
            var stateIsChanged = false;

            #region 2sxc Standard Assets and Header

            // Add Context-Meta first, because it should be available when $2sxc loads
            if (ViewResults.SxcContextMetaName != null)
            {
                Log($"2.2: RenderUri:{RenderedUri}", $"hash:{hash}");
                await SxcInterop.IncludeMeta("sxc-context-meta", "name", ViewResults.SxcContextMetaName,
                    ViewResults.SxcContextMetaContents /*, "id"*/); // Oqtane.client 3.3.1
                stateIsChanged = true;
            }

            // Lets load all 2sxc js dependencies (js / styles)
            // Not done the official Oqtane way, because that asks for the scripts before
            // the razor component reported what it needs
            if (ViewResults.SxcScripts != null)
                foreach (var resource in ViewResults.SxcScripts)
                {
                    Log($"2.3: IncludeScript:{resource}", $"hash:{hash}");
                    await SxcInterop.IncludeScript("", resource, "", "", "", "head");
                    stateIsChanged = true;
                }

            if (ViewResults.SxcStyles != null)
                foreach (var style in ViewResults.SxcStyles)
                {
                    Log($"2.4: IncludeCss:{style}", $"hash:{hash}");
                    await SxcInterop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");
                    stateIsChanged = true;
                }

            #endregion

            #region External resources requested by the razor template

            if (ViewResults.TemplateResources != null)
            {
                Log($"2.5: AttachScriptsAndStyles", $"hash:{hash}");
                await OqtPageChangeService.AttachScriptsAndStyles(ViewResults, PageState, SxcInterop, this);
                stateIsChanged = true;
            }

            if (ViewResults.PageProperties?.Any() ?? false)
            {
                Log($"2.6: UpdatePageProperties", $"hash:{hash}");
                await OqtPageChangeService.UpdatePageProperties(ViewResults, PageState, SxcInterop, this);
                stateIsChanged = true;
            }
            
            #endregion

            if (stateIsChanged)
                StateHasChanged();

            // NavigationManager.NavigateTo(NavigateUrl(), true);
        }

        // generate 4 chars random hash
    }
}
