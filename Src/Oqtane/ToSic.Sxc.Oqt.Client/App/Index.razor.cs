using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
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
    [Inject] public IConfiguration Configuration { get; set; }
    #endregion

    #region Shared Variables
    public OqtViewResultsDto ViewResults { get; set; }
    public string Content { get; set; }

    private string RenderedUri { get; set; }
    private string RenderedPage { get; set; }
    private bool NewDataArrived { get; set; }

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
            Log($"1: OnParametersSetAsync(NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");

            // Call 2sxc engine only when is necessary to render control, because it is heavy operation and
            // OnParametersSetAsync is executed more than ounce for single page navigation change.
            // 2sxc module render state depends on AliasId, PageId, ModuleId, Culture, Query...
            // Optimally it should be executed ounce for single page navigation change, but with correct PageId and ModuleId.
            // Still during change of PageState and ModuleState sometimes we get old ModuleId from older page, before we get correct ModuleId from new page.
            if (ShouldRender() && IsUriNewOrChanged())
            {
                Log($"1.1: RenderUri:{RenderedUri}");

                // Ensure that only one thread is rendering the module at a time.
                // This prevents exception "Some Stream-Wirings were not created" #3291
                using (await RenderSpecificLockManager.LockAsync(ModuleState.RenderId))
                {

                    ViewResults = await Initialize2SxcContentBlock();
                    if (ViewResults != null)
                    {
                        NewDataArrived = true;

                        #region HTML response
                        Content = OqtPageChangeService.ProcessPageChanges(ViewResults, SiteState, this);

                        if (RenderMode == RenderModes.Static)
                            if (!RenderInfoService.IsSsrFraming(RenderMode)) // SSR First load on 2sxc page
                                Content = OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml(ViewResults, SiteState, Content, Theme.Name);
                            else // SSR Partial load after starting on 2sxc page
                                Content = OqtPageChangeService.AttachScriptsAndStylesDynamicallyWithTurnOn(ViewResults, SiteState, Content, Theme.Name);
                        //else
                        //    if (_firstPageLoad)
                        //    {
                        //        _firstPageLoad = false;
                        //        Content = OqtPageChangeService.AttachScriptsAndStyles(ViewResults, SiteState, Content, Theme.Name);
                        //    }

                        // convenient place to apply Csp HttpHeaders to response
                        var count = OqtPageChangesOnServerService.ApplyHttpHeaders(ViewResults, this);
                        Log($"1.4: Csp:{count}");
                        #endregion
                    }
                }
            }

            Log($"1 end: OnParametersSetAsync(NewDataArrived:{NewDataArrived},RenderedUri:{RenderedUri},RenderedPage:{RenderedPage})");
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }
    private bool _firstPageLoad = true;


    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // INTERACTIVE mode only
        await base.OnAfterRenderAsync(firstRender);

        try
        {
            Log($"2: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");

            // 2sxc part should be executed only if new 2sxc data arrived from server (ounce per view)
            if (IsSafeToRunJs && NewDataArrived && ViewResults != null)
            {
                NewDataArrived = false;

                #region HTML response, part 1.
                Log($"2.1: NewDataArrived");
                await OqtPageChangeService.AttachScriptsAndStylesForInteractiveRendering(ViewResults, SxcInterop, this);




                //StateHasChanged();
                // Register ReloadModule
                _dotNetObjectReference = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync($"{OqtConstants.PackageName}.registerReloadModule", _dotNetObjectReference, ModuleState.ModuleId);
                #endregion
            }

            Log($"2 end: OnAfterRenderAsync(firstRender:{firstRender},NewDataArrived:{NewDataArrived},ViewResults:{ViewResults != null})");
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }
    private DotNetObjectReference<Index> _dotNetObjectReference;

    // This is called from JS to reload module content from blazor (instead of ajax that breaks blazor)
    [JSInvokable("ReloadModule")]
    public async Task ReloadModule()
    {
        try
        {
            Log($"3: ReloadModule");
            ViewResults = await Initialize2SxcContentBlock();

            #region HTML response, part 1.
            Content = OqtPageChangeService.ProcessPageChanges(ViewResults, SiteState, this);
            if (RenderMode == RenderModes.Static)
                Content = OqtPageChangeService.AttachScriptsAndStylesStaticallyInHtml(ViewResults, SiteState, Content, Theme.Name);
            else
                await OqtPageChangeService.AttachScriptsAndStylesForInteractiveRendering(ViewResults, SxcInterop, this);
            #endregion

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

    public bool IsDev => Configuration["Environment"] == "Development";

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
    private bool IsUriNewOrChanged()
    {
        var isUriNewOrChanged = (string.IsNullOrEmpty(RenderedUri)
                                 || (!NavigationManager.Uri.StartsWith(RenderedPage, InvariantCultureIgnoreCase))
                                 || (!NavigationManager.Uri.Equals(RenderedUri, InvariantCultureIgnoreCase) && NavigationManager.Uri.StartsWith(RenderedPage, InvariantCultureIgnoreCase)));

        if (isUriNewOrChanged)
        {
            // preserve values for subsequent detection of uri change
            RenderedUri = NavigationManager.Uri;
            RenderedPage = NavigationManager.Uri.RemoveQueryAndFragment();
        }
        return isUriNewOrChanged;
    }

    #region Theme
    private Theme Theme => PageState.Site.Themes.FirstOrDefault(theme => theme.Themes.Any(themeControl => themeControl.TypeName == ThemeType));

    private string ThemeType => PageState.Page.ThemeType ?? PageState.Site.DefaultThemeType;

    #endregion
}