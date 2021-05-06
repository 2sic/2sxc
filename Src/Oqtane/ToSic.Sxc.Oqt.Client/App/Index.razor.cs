using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Oqtane.Modules;
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


        public override List<Resource> Resources => new List<Resource>();

        public OqtViewResultsDto ViewResults { get; set; }

        //protected override async Task OnInitializedAsync()
        //{
        //    await base.OnInitializedAsync();

        //    // Subscribe to LocationChanged event.
        //    NavigationManager.LocationChanged += HandleLocationChanged;
        //}

        protected override async Task OnParametersSetAsync()
        {

            // Call 2sxc engine only when is necesary to render control.
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
            //if (string.IsNullOrEmpty(culture)) culture = await GetUserSelectedCultureFromCookie();

            var urlQuery = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
            ViewResults = await OqtSxcRenderService.PrepareAsync(
                PageState.Alias.AliasId,
                PageState.Page.PageId,
                ModuleState.ModuleId,
                culture,
                urlQuery);

            if (!string.IsNullOrEmpty(ViewResults.ErrorMessage)) AddModuleMessage(ViewResults.ErrorMessage, MessageType.Warning);
        }

        //private async Task<string> GetUserSelectedCultureFromCookie()
        //{
        //    var interop = new Interop(JSRuntime);
        //    var localizationCookie = await interop.GetCookie(CookieRequestCultureProvider.DefaultCookieName);
        //    return CookieRequestCultureProvider.ParseCookieValue(localizationCookie).UICultures[0].Value;
        //}

        //public void Dispose() => NavigationManager.LocationChanged -= HandleLocationChanged;


        /// <summary>
        /// Handle router LocationChanged event.
        /// This is important if the url changes like /product --> /product?details=27
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //private void HandleLocationChanged(object sender, LocationChangedEventArgs args)
        //{
        //    var log = $"{sender} {args}";
        //} //Initialize2sxcContentBlock(); //.RunSynchronously();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);

                // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
                if (NewDataArrived && PageState.Runtime == Oqtane.Shared.Runtime.Server && ViewResults != null/* && 1 == 0*/)
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
                    {
                        // External resources = independent files (so not inline JS in the template)
                        var externalResources = ViewResults.TemplateResources.Where(r => r.IsExternal).ToArray();

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
                        var bundleId = "module-bundle-" + PageState.ModuleId;
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
                        var inlineResources = ViewResults.TemplateResources.Where(r => !r.IsExternal).ToArray();
                        foreach (var inline in inlineResources)
                            await interop.IncludeScript(string.IsNullOrWhiteSpace(inline.UniqueId) ? null : inline.UniqueId,
                                "",
                                "",
                                "",
                                inline.Content,
                                "body",
                                "");
                    }

                    #endregion
                }
            }
        }
    }
}
