using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Security;
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
using Interop = ToSic.Sxc.Oqt.Client.Interop;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public partial class Index: ModuleBase
    {
        #region Injected Services

        [Inject] public IOqtSxcRenderService OqtSxcRenderService { get; set; }
        
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public IOqtPrerenderService OqtPrerenderService { get; set; }

        [Inject] public Lazy<IFeaturesService> FeaturesService { get; set; }

        [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }

        #endregion

        #region Shared Variables

        public bool Debug;
        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(PageState.User, RoleNames.Host);
        private bool? _isSuperUser;
        private bool _isSafeToRunJs;

        private string RenderedUri { get; set; }
        private string RenderedPage { get; set; }
        private bool NewDataArrived { get; set; }
        public OqtViewResultsDto ViewResults { get; set; }

        /// <summary>
        /// JS Interop, will be initialized in OnInitializedAsync
        /// </summary>
        public Interop Interop;

        #endregion

        #region Oqtane Properties

        public override List<Resource> Resources => new()
        {
            new Resource { ResourceType = ResourceType.Script, Url = "Modules/ToSic.Sxc/Module.js" }
        };

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Interop ??= new Interop(JSRuntime);
        }


        protected override async Task OnParametersSetAsync()
        {
            NavigationManager.TryGetQueryString("debug", out Debug);
            await Log($"1: OnParametersSetAsync(Debug:{Debug},NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");
            // Call 2sxc engine only when is necessary to render control.
            if (string.IsNullOrEmpty(RenderedUri) || (!NavigationManager.Uri.Equals(RenderedUri, InvariantCultureIgnoreCase) && NavigationManager.Uri.StartsWith(RenderedPage, InvariantCultureIgnoreCase)))
            {
                RenderedUri = NavigationManager.Uri;
                await Log($"1.1: RenderUri:{RenderedUri}");
                var indexOfQuestion = NavigationManager.Uri.IndexOf("?", Ordinal);
                RenderedPage = indexOfQuestion > -1
                    ? NavigationManager.Uri.Substring(0, indexOfQuestion)
                    : NavigationManager.Uri;
                await Log($"1.2: Initialize2sxcContentBlock");
                await Initialize2SxcContentBlock();
                NewDataArrived = true;
                ViewResults.SystemHtml = OqtPrerenderService.Init(PageState, logger).GetSystemHtml();
                Csp();
                await Log($"1.3: Csp");
            }

            await base.OnParametersSetAsync();

            await Log($"1 end: OnParametersSetAsync(Debug:{Debug},NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");
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
                urlQuery);

            if (!string.IsNullOrEmpty(ViewResults?.ErrorMessage))
            {
                await Log($"1.2.: ErrorMessage:{ViewResults.ErrorMessage}");
                AddModuleMessage(ViewResults.ErrorMessage, MessageType.Warning);
            }

            await Log($"1.2.1: Html:{ViewResults?.Html.Length}", ViewResults);
        }

        #region CSP

        public bool PrerenderingEnabled() => PageState.Site.RenderMode == "ServerPrerendered"; // The render mode for the site.
        public bool ApplyCsp = true;

        private void Csp()
        {
            if (PrerenderingEnabled() && ApplyCsp // executed only in prerender
                && (HttpContextAccessor?.HttpContext?.Request?.Path.HasValue == true)
                && !HttpContextAccessor.HttpContext.Request.Path.Value.Contains("/_blazor"))
                if (ViewResults?.CspParameters?.Any() ?? false)
                    PageChangesHelper.ApplyHttpHeaders(ViewResults, FeaturesService, HttpContextAccessor);

            ApplyCsp = false; // flag to ensure that code is executed only first time in prerender
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) _isSafeToRunJs = true; // now we are safe to have Interop and run js

            await Log($"2: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");
            await base.OnAfterRenderAsync(firstRender);

            // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
            if (NewDataArrived && PageState.Runtime == Runtime.Server && ViewResults != null)
            {
                await Log($"2.1: NewDataArrived");
                NewDataArrived = false;

                #region 2sxc Standard Assets and Header

                // Add Context-Meta first, because it should be available when $2sxc loads
                if (ViewResults.SxcContextMetaName != null)
                {
                    await Log($"2.2: RenderUri:{RenderedUri}");
                    await Interop.IncludeMeta("sxc-context-meta", "name", ViewResults.SxcContextMetaName, ViewResults.SxcContextMetaContents, "id");
                }

                // Lets load all 2sxc js dependencies (js / styles)
                // Not done the official Oqtane way, because that asks for the scripts before
                // the razor component reported what it needs
                if (ViewResults.SxcScripts != null)
                    foreach (var resource in ViewResults.SxcScripts)
                    {
                        await Log($"2.3: IncludeScript:{resource}");
                        await Interop.IncludeScript("", resource, "", "", "", "head");
                    }

                if (ViewResults.SxcStyles != null)
                    foreach (var style in ViewResults.SxcStyles)
                    {
                        await Log($"2.4: IncludeCss:{style}");
                        await Interop.IncludeLink("", "stylesheet", style, "text/css", "", "", "");
                    }

                #endregion

                #region External resources requested by the razor template

                if (ViewResults.TemplateResources != null)
                {
                    await Log($"2.5: AttachScriptsAndStyles");
                    await PageChangesHelper.AttachScriptsAndStyles(ViewResults, PageState, Interop);
                }

                if (ViewResults.PageProperties?.Any() ?? false)
                {
                    await Log($"2.6: UpdatePageProperties");
                    await PageChangesHelper.UpdatePageProperties(ViewResults, PageState, Interop);
                }

                StateHasChanged();

                #endregion
            }
            await Log($"2 end: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");
        }

        #region Log Helpers

        /// <summary>
        /// console.log
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Log(params object[] message)
        {
            // If the url has a debug=true and we are the super-user
            if (message == null || !message.Any() || !Debug || !IsSuperUser) return;
            
            _logPrefix ??= $"2sxc:Page({PageState?.Page?.PageId}):Mod({ModuleState?.ModuleId}):";
            try
            {
                // log on web server
                Console.WriteLine($"{_logPrefix} {message.FirstOrDefault()}");
                // log to browser console
                if (_isSafeToRunJs)
                {
                    var data = new List<object> { _logPrefix }.Concat(message);
                    await JSRuntime.InvokeVoidAsync(ConsoleLogJs, data.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{_logPrefix}:{ex.Message}");
                if (_isSafeToRunJs)
                    await JSRuntime.InvokeVoidAsync(ConsoleLogJs, "Error:", _logPrefix, ex.Message);
            }
        }
        private string _logPrefix;
        private const string ConsoleLogJs = "console.log";

        #endregion
    }
}
