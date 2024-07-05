using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Models;
using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;
using static System.StringComparison;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class Index : ModuleProBase
{
    #region Injected Services
    [Inject] public IOqtSxcRenderService OqtSxcRenderService { get; set; }
    [Inject] public OqtPageChangeService OqtPageChangeService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IOqtPageChangesOnServerService OqtPageChangesOnServerService { get; set; }
    [Inject] public IOqtPrerenderService OqtPrerenderService { get; set; }
    [Inject] public RenderSpecificLockManager RenderSpecificLockManager { get; set; }
    [Inject] public IRenderInfoService RenderInfoService { get; set; }
    #endregion

    #region Shared Variables

    protected string Content { get; private set; }

    private OqtViewResultsDto _viewResults;
    private string _renderedUri;
    private string _renderedPage;
    private string _renderedQuery;
    private bool _newDataArrived;

    #endregion

    #region Oqtane Properties

    public override List<Resource> Resources => [
        new Resource { ResourceType = ResourceType.Script, Url = $"Modules/{OqtConstants.PackageName}/Module.js", Reload = true },
        new Resource { ResourceType = ResourceType.Script, Url = $"Modules/{OqtConstants.PackageName}/dist/turnOn/turn-on.js", Reload = true }
        ];

    public override string RenderMode => PageState?.RenderMode ?? RenderModes.Static;

    ///// <inheritdoc/>
    //protected override async Task OnInitializedAsync()
    //{
    //    try
    //    {
    //        //if (RenderMode != RenderModes.Static) return;

    //        //Initialize2SxcContentBlock();
    //        //ProcessPageChanges();
    //        //_resources.AddRange(StaticSsrStandardAssets());
    //    }
    //    catch (Exception ex)
    //    {
    //        await logger.LogError(ex, "Error Loading Test512 {Error}", ex.Message);
    //        //AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
    //    }
    //}

    #endregion

    /// <summary>
    /// Lifecycle method is executed for every component parameters change
    /// </summary>
    /// <remarks>
    /// This method is called before the first render of the component and whenever the component's parameters are updated: PageState, ModuleState, ModuleInstance.
    /// Route change will create new PageState in Oqtane Router, etc...
    /// </remarks>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        try
        {
            Log($"1: OnParametersSetAsync(NewDataArrived:{_newDataArrived},RenderedUri:{_renderedUri})");

            // Call 2sxc engine only when is necessary to render control, because it is heavy operation and
            // OnParametersSetAsync is executed more than ounce for single page navigation change.
            // 2sxc module render state depends on AliasId, PageId, ModuleId, Culture, Query...
            // Optimally it should be executed ounce for single page navigation change, but with correct PageId and ModuleId.
            // Still during change of PageState and ModuleState sometimes we get old ModuleId from older page, before we get correct ModuleId from new page.
            if (SxcShouldRender())
            {
                Log($"1.1: RenderUri:{_renderedUri}");

                // Ensure that only one thread is rendering the module at a time.
                // This prevents exception "Some Stream-Wirings were not created" #3291
                using (await RenderSpecificLockManager.LockAsync(ModuleState.RenderId))
                {

                    _viewResults = await Initialize2SxcContentBlock();
                    if (_viewResults != null)
                    {
                        _newDataArrived = true;

                        #region HTML response, part 1
                        if (Content != _viewResults.FinalHtml)
                            Content = _viewResults.FinalHtml;

                        OqtPageChangeService.ProcessPageChanges(_viewResults, SiteState, this);
                        #endregion

                        #region HTML response, part 2 (Static SSR)
                        if (RenderMode == RenderModes.Static)
                            if (!RenderInfoService.IsSsrFraming(RenderMode)) // SSR First load on 2sxc page
                                Content = OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml(_viewResults, SiteState, Content, Theme.Name);
                            else // SSR Partial load after starting on 2sxc page
                                Content = OqtPageChangeService.AttachScriptsAndStylesDynamicallyWithTurnOn(_viewResults, SiteState, Content, Theme.Name);
                        #endregion

                        // convenient place to apply Csp HttpHeaders to response
                        var count = OqtPageChangesOnServerService.ApplyHttpHeaders(_viewResults, this);
                        Log($"1.4: Csp:{count}");
                    }
                }
            }

            Log($"1 end: OnParametersSetAsync(NewDataArrived:{_newDataArrived},RenderedUri:{_renderedUri},RenderedPage:{_renderedPage})");
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // INTERACTIVE mode only
        await base.OnAfterRenderAsync(firstRender);

        try
        {
            Log($"2: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{_newDataArrived},ViewResults:{_viewResults != null})");

            // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
            if (IsSafeToRunJs && _newDataArrived && _viewResults != null)
            {
                _newDataArrived = false;

                #region HTML response, part 2 (Interactive)
                Log($"2.1: NewDataArrived");
                await OqtPageChangeService.AttachScriptsAndStylesForInteractiveRendering(_viewResults, SxcInterop, this);
                #endregion

                StateHasChanged();

                _dotNetObjectReference = DotNetObjectReference.Create(this);
                // Register ReloadModule
                //if (PageState.Runtime is Runtime.WebAssembly /*or Runtime.Auto*/) 
                await JSRuntime.InvokeVoidAsync($"{OqtConstants.PackageName}.registerReloadModule", _dotNetObjectReference, ModuleState.ModuleId);
            }

            Log($"2 end: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{_newDataArrived},ViewResults:{_viewResults != null})");
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }
    private DotNetObjectReference<Index> _dotNetObjectReference;

    // This is called from JS to reload module content from blazor (instead of ajax that breaks blazor)
    // 2024-07-04 stv, looks that is not necessary anymore, we can probably remove it
    // we had different strategy for Interactive and SSR to refresh module content on page after edit using 2sxc Edit UI
    // Interactive use this 'ReloadModule' method to update DOM, but this breaks from Oqtane 5.1.2 because of blazor.web.js
    // so initial fix was just to reload page with window.location.reload()
    // but after more testing it looks that strategy we use for Static SSR is good enough for all cases
    // if this is true we can remove this code in total
    [JSInvokable("ReloadModule")]
    public async Task ReloadModule()
    {
        try
        {
            Log($"3: ReloadModule");
            _viewResults = await Initialize2SxcContentBlock();

            if (_viewResults != null)
            {
                #region HTML response, part 1.
                if (Content != _viewResults.FinalHtml)
                    Content = _viewResults.FinalHtml;

                OqtPageChangeService.ProcessPageChanges(_viewResults, SiteState, this);

                if (RenderMode == RenderModes.Static) // Static SSR
                    Content = OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml(_viewResults, SiteState, Content, Theme.Name);
                else // Interactive
                    await OqtPageChangeService.AttachScriptsAndStylesForInteractiveRendering(_viewResults, SxcInterop, this);
                #endregion

                StateHasChanged();
            }
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
    /// Prepare the html / headers for later rendering
    /// </summary>
    private async Task<OqtViewResultsDto> Initialize2SxcContentBlock()
    {
        Log($"1.2: Initialize2sxcContentBlock");

        #region ViewResults prepare
        var viewResults = await OqtSxcRenderService.PrepareAsync(
            PageState.Alias.AliasId,
            PageState.Page.PageId,
            ModuleState.ModuleId,
            CultureInfo.CurrentUICulture.Name,
            IsPrerendering(), NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query);

        if (!string.IsNullOrEmpty(viewResults?.ErrorMessage))
            LogError(viewResults.ErrorMessage);

        Log($"1.2.1: Html:{viewResults?.Html?.Length ?? -1}");
        #endregion

        #region ViewResults finalization
        if (viewResults != null)
            viewResults.PrerenderHtml = OqtPrerenderService.GetPrerenderHtml(IsPrerendering(), viewResults, SiteState, ThemeType);

        #endregion

        return viewResults;
    }

    /// <summary>
    /// Filter to render the control only when is necessary.
    /// </summary>
    /// <returns></returns>
    private bool SxcShouldRender()
    {
        // 1st criteria
        if (!ShouldRender()) return false;

        // 2nd criteria - detect if uri is new or changed
        var currentUri = NavigationManager.Uri;
        var currentPage = currentUri.RemoveQueryAndFragment();
        var currentQuery = NavigationManager.ToAbsoluteUri(currentUri).Query;

        //// Detect if uri is new or changed, but ignore changes with fragments, hash, etc...
        var isUriNewOrChanged = string.IsNullOrEmpty(_renderedUri) // executed for first time
                                || !currentPage.Equals(_renderedPage, InvariantCultureIgnoreCase) // new page
                                || !currentQuery.Equals(_renderedQuery, InvariantCultureIgnoreCase); // same page but new query;

        //var isUriNewOrChanged = !currentUri.Equals(_renderedUri, InvariantCultureIgnoreCase);

        if (isUriNewOrChanged)
        {
            // preserve values for subsequent detection of uri change
            _renderedUri = currentUri;
            _renderedPage = currentPage;
            _renderedQuery = currentQuery;

            //NavigateUrl(currentUri);
        }
        return isUriNewOrChanged;
    }

    #region Theme
    private Theme Theme => PageState.Site.Themes.FirstOrDefault(theme => theme.Themes.Any(themeControl => themeControl.TypeName == ThemeType));

    private string ThemeType => PageState.Page.ThemeType ?? PageState.Site.DefaultThemeType;

    #endregion
}