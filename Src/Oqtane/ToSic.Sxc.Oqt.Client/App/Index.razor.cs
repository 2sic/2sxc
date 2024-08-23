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

    #region Oqtane Properties

    public override string RenderMode => RenderModes.Static;

    public override List<Resource> Resources => [
        new Resource { ResourceType = ResourceType.Script, Url = $"Modules/{OqtConstants.PackageName}/Module.js", Reload = true },
        new Resource { ResourceType = ResourceType.Script, Url = $"Modules/{OqtConstants.PackageName}/dist/turnOn/turn-on.js", Reload = true }
        ];

    #endregion

    #region Shared Variables

    protected string Content { get; private set; } = "";

    private OqtViewResultsDto _viewResults;
    private RenderParameters _renderedParameters;
    private bool _newDataArrived;

    #endregion

    protected override void OnInitialized()
    {
        Log($"2sxc Blazor Logging Enabled on {OqtDebugStateService.Platform}");  // will only show if it's enabled
        Log($"- ModuleState.RenderMode: {RenderMode}");
        Log($"- PageState.RenderMode: {PageState?.RenderMode}");
        Log($"- PageState.Runtime: {PageState?.Runtime}");
        Log($"- Static SSR: {RenderInfoService.IsStaticSsr(RenderMode)}");
        Log($"- Blazor Enhanced Nav: {RenderInfoService.IsBlazorEnhancedNav(RenderMode)}");
        Log($"- SSR Framing: {RenderInfoService.IsSsrFraming(RenderMode)}");
    }

    //protected override async Task OnInitializedAsync()
    //{
    //    await base.OnInitializedAsync();
    //    Log($"2sxc Blazor Logging Enabled on {OqtDebugStateService.Platform}");  // will only show if it's enabled
    //}

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

        var @params = new RenderParameters()
        {
            AliasId = PageState.Alias.AliasId,
            PageId = PageState.Page.PageId,
            ModuleId = ModuleState.ModuleId,
            Culture = CultureInfo.CurrentUICulture.Name,
            PreRender = IsPrerendering(),
            OriginalParameters = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query
        };

        try
        {
            Log($"OnParametersSetAsync(NewDataArrived:{_newDataArrived})");

            // Call 2sxc engine only when is necessary to render control, because it is heavy operation and
            // OnParametersSetAsync is executed more than ounce for single page navigation change.
            // 2sxc module render state depends on AliasId, PageId, ModuleId, Culture, Query...
            // Optimally it should be executed ounce for single page navigation change, but with correct PageId and ModuleId.
            // Still during change of PageState and ModuleState sometimes we get old ModuleId from older page, before we get correct ModuleId from new page.
            if (ShouldRenderSxcView(@params))
            {
                Log($"Need to render");

                // Ensure that only one thread is rendering the module at a time.
                // This prevents exception "Some Stream-Wirings were not created" #3291
                using (await RenderSpecificLockManager.LockAsync(ModuleState.RenderId))
                {
                    _viewResults = await RenderSxcView(@params);
                    if (_viewResults != null)
                    {
                        _newDataArrived = true;
                        Log($"NewDataArrived:{_newDataArrived}");

                        var newContent = OqtPageChangeService.ProcessPageChanges(_viewResults, SiteState, this);

                        #region Static SSR
                        if (PageState.RenderMode == RenderModes.Static)
                            if (!RenderInfoService.IsSsrFraming(PageState.RenderMode)) // SSR First load on 2sxc page
                            {
                                Log($"SSR First load on 2sxc page - {nameof(OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml)}");
                                newContent = OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml(_viewResults, SiteState, newContent, Theme.Name);
                            }
                            else // SSR Partial load after starting on 2sxc page
                            {
                                Log($"SSR Partial load after starting on 2sxc page - {nameof(OqtPageChangeService.AttachScriptsAndStylesDynamicallyWithTurnOn)}");
                                newContent = OqtPageChangeService.AttachScriptsAndStylesDynamicallyWithTurnOn(_viewResults, SiteState, newContent, Theme.Name, ModuleState.RenderId);
                            }
                        #endregion

                        if (Content != newContent)
                        {
                            Log($"Set Content (old:{Content.Length}; new:{newContent.Length})");
                            Content = newContent;
                        }

                        // convenient place to apply Csp HttpHeaders to response
                        var count = OqtPageChangesOnServerService.ApplyHttpHeaders(_viewResults, this);
                        Log($"Csp:{count}");
                    }
                }
            }
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
            Log($"OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{_newDataArrived},ViewResults:{_viewResults != null})");

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
        var @params = new RenderParameters()
        {
            AliasId = PageState.Alias.AliasId,
            PageId = PageState.Page.PageId,
            ModuleId = ModuleState.ModuleId,
            Culture = CultureInfo.CurrentUICulture.Name,
            PreRender = IsPrerendering(),
            OriginalParameters = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query
        };

        try
        {
            Log($"3: ReloadModule");
            _viewResults = await RenderSxcView(@params);

            if (_viewResults != null)
            {
                var newContent = _viewResults.FinalHtml;

                OqtPageChangeService.ProcessPageChanges(_viewResults, SiteState, this);

                if (PageState.RenderMode == RenderModes.Static) // Static SSR
                    newContent = OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml(_viewResults, SiteState, newContent, Theme.Name);
                else // Interactive
                    await OqtPageChangeService.AttachScriptsAndStylesForInteractiveRendering(_viewResults, SxcInterop, this);

                if (Content != newContent) Content = newContent;

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
    /// Filter to render the control only when is necessary.
    /// </summary>
    /// <remarks>
    /// Call 2sxc engine only when is necessary to render control, because it is heavy operation and
    /// OnParametersSetAsync is executed more than ounce for single page navigation change.
    /// 2sxc module render state depends on AliasId, PageId, ModuleId, Culture, Query...
    /// Optimally it should be executed ounce for single page navigation change, but with correct PageId and ModuleId.
    /// Still during change of PageState and ModuleState sometimes we get old ModuleId from older page, before we get correct ModuleId from new page.
    /// </remarks>
    /// <returns></returns>
    private bool ShouldRenderSxcView(RenderParameters @params)
    {
        // 1st criteria
        if (!ShouldRender()) return false;

        // 2nd criteria - render parameters are not changed from the last render
        if (_renderedParameters != null && _renderedParameters.Equals(@params)) return false;

        // render parameters are changed, store them for the next render
        _renderedParameters = @params.Clone();
        return true;
    }

    /// <summary>
    /// Prepare the html / headers for later rendering
    /// </summary>
    private async Task<OqtViewResultsDto> RenderSxcView(RenderParameters @params)
    {
        Log($"Get html and other view resources from server");
        var viewResults = await OqtSxcRenderService.RenderAsync(@params);

        if (!string.IsNullOrEmpty(viewResults?.ErrorMessage))
            LogError(viewResults.ErrorMessage);

        Log($"Html:{viewResults?.Html?.Length ?? -1}");

        // finalize with prerendered html
        if (viewResults != null)
            viewResults.PrerenderHtml = OqtPrerenderService.GetPrerenderHtml(@params.PreRender, viewResults, SiteState, ThemeType);

        return viewResults;
    }

    #region Theme
    private Theme Theme => PageState.Site.Themes.FirstOrDefault(theme => theme.Themes.Any(themeControl => themeControl.TypeName == ThemeType));

    private string ThemeType => PageState.Page.ThemeType ?? PageState.Site.DefaultThemeType;

    #endregion

}