using DotNetNuke.Entities.Modules;
using System.Linq;
using System.Web.UI;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Dnn.Features;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web.Internal.LightSpeed;

namespace ToSic.Sxc.Dnn;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class View : PortalModuleBase, IActionable
{
    private static bool _loggedToBootLog;

    public View()
    {
        if (_loggedToBootLog) return;
        _loggedToBootLog = true;
        BootLog.Log.A("Dnn: First moment where View was used.");
    }

    #region GetService and Service Provider

    /// <summary>
    /// Get the service provider only once - ideally in Dnn9.4 we will get it from Dnn
    /// If we would get it multiple times, there are edge cases where it could be different each time! #2614
    /// </summary>
    private IServiceProvider ServiceProvider => field ??= DnnStaticDi.CreateModuleScopedServiceProvider();

    private TService GetService<TService>() => ServiceProvider.Build<TService>(Log);

    #endregion

    /// <summary>
    /// Block needs to self-initialize when first requested, because it's used in the Actions-Menu builder
    /// which runs before page-load
    /// </summary>
    private IBlock Block => _blockGetOnce.Get(Log,
        () => LogTimer.DoInTimer(()
            => GetService<IModuleAndBlockBuilder>().BuildBlock(ModuleConfiguration, null)
        ), timer: true
    );
    private readonly GetOnce<IBlock> _blockGetOnce = new();

    private IBlockBuilder BlockBuilder => field ??= GetService<IBlockBuilder>().Setup(Block);

    #region Logging

    private ILog Log => _log ??= new Log("Sxc.View"); // delay creating until first use to really just track our time
    private ILog _log;
    private LogStoreEntry _logInStore;

    protected ILogCall LogTimer => _logTimer.Get(() => Log.Fn(message: $"Module: '{ModuleConfiguration.ModuleTitle}'"));
    private readonly GetOnce<ILogCall> _logTimer = new();

    #endregion

    /// <summary>
    /// Page Load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e) =>
        LogTimer.DoInTimer(() =>
        {
            // add to insights-history for analytic
            _logInStore = GetService<ILogStore>().Add("module", Log);

            var l = Log.Fn(message: nameof(Page_Load), timer: true);
            // todo: this should be dynamic at some future time, because normally once it's been checked, it wouldn't need checking again
            var checkPortalIsReady = true;
            bool? requiresPre1025Behavior = null; // null = auto-detect, true/false

            // get the block early, to see any errors separately - before accessing cache (which also uses the block)
            var block = TryCatchAndLogToDnn(() => Block);

            #region Lightspeed

            try
            {
                if (OutputCache?.Existing != null)
                {
                    checkPortalIsReady = false;
                    requiresPre1025Behavior = OutputCache.Existing.EnforcePre1025;
                }
            }
            catch { /* ignore */ }

            #endregion

            // Always do this, part of the guarantee that everything will work
            // new mechanism in 10.25
            // this must happen in Page-Load, so we know what supporting scripts to add
            // at this stage of the lifecycle
            // We moved this to Page_Load because RequestAjaxAntiForgerySupport didn't work in later events
            // ensure everything is ready and that we know if we should activate the client-dependency
            TryCatchAndLogToDnn(() =>
            {
                if (checkPortalIsReady)
                    if (!DnnReadyCheckTurbo.QuickCheckSiteAndAppFoldersAreReady(this, Log))
                        GetService<DnnReadyCheckTurbo>().EnsureSiteAndAppFoldersAreReady(this, block);
                var blockBuilder = requiresPre1025Behavior == false ? null : BlockBuilder;
                _dnnClientResources = GetService<DnnClientResources>().Init(Page, null, blockBuilder);
                _enforcePre1025JQueryLoading = requiresPre1025Behavior ?? _dnnClientResources.NeedsPre1025Behavior();
                if (_enforcePre1025JQueryLoading) _dnnClientResources.EnforcePre1025Behavior();
                return true;
            });
            l.Done();
        });

    private DnnClientResources _dnnClientResources;
    private bool _enforcePre1025JQueryLoading;


    /// <summary>
    /// Process View if a Template has been set
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        var l = Log.Fn();
        var finalMessage = "";
        LogTimer.DoInTimer(() =>
        {
            // #lightspeed
            if (OutputCache?.Existing != null)
                l.A("Lightspeed hit - will use cached");

            IRenderResult renderResult = null;
            var headersAndScriptsAdded = false;

            // skip this if something before this caused an error
            if (!IsError)
                TryCatchAndLogToDnn(() =>
                {
                    // Try to build the html and everything
                    renderResult = OutputCache?.Existing?.Data;

                    var useLightspeed = OutputCache?.IsEnabled ?? false;
                    finalMessage = !useLightspeed ? "" : renderResult != null ? "⚡⚡" : "⚡⏳";

                    // Generate Render Result if not already provided by cache
                    renderResult ??= RenderViewAndGatherJsCssSpecs(useLightspeed);

                    // Apply page settings, assets, resources etc. from Render Result
                    try
                    {
                        GetService<DnnPageChanges>().Apply(Page, renderResult);
                    }
                    catch
                    {
                        /* ignore */
                    }

                    // Try to add page specs about the request to the log (new v16.02)
                    try
                    {
                        _logInStore?.UpdateSpecs(new SpecsForLogHistory().BuildSpecsForLogHistory(Block));
                    }
                    catch
                    {
                        /* ignore */
                    }

                    // call this after rendering templates, because the template may change what resources are registered
                    _dnnClientResources.AddEverything(renderResult.Features);
                    headersAndScriptsAdded = true; // will be true if we make it this far

                    // If standalone is specified, output just the template without anything else
                    if (RenderNaked)
                        SendStandalone(renderResult.Html);
                    else
                        phOutput.Controls.Add(new LiteralControl(renderResult.Html));

                    // #Lightspeed
                    var lLightSpeed = Log.Fn(message: "Lightspeed", timer: true);
                    OutputCache?.Save(renderResult, _enforcePre1025JQueryLoading);
                    lLightSpeed.Done();

                    return true; // dummy result for TryCatchAndLogToDnn
                });

            // if we had an error before, or have one now, re-check assets
            if (IsError && !headersAndScriptsAdded)
                _dnnClientResources?.AddEverything(renderResult?.Features);
        });
        LogTimer.Done(IsError ? "⚠️" : finalMessage);
        l.Done();
    }

    private IRenderResult RenderViewAndGatherJsCssSpecs(bool useLightspeed)
    {
        var l = Log.Fn<IRenderResult>(message: $"module {ModuleId} on page {TabId}", timer: true);

        var result = new RenderResult();
        TryCatchAndLogToDnn(() =>
        {
            if (RenderNaked)
                BlockBuilder.WrapInDiv = false;
            result = (RenderResult)BlockBuilder.Run(
                true,
                specs: new()
                {
                    UseLightspeed = useLightspeed,
                    RenderEngineResult = GetService<DnnRequirements>().GetMessageForRequirements()
                }
            );

            if (result.Errors?.Any() ?? false)
            {
                var warnings = result.Errors
                    .Select(e => BlockBuilder.RenderingHelper.DesignError(e));

                result = result with { Html = string.Join("", warnings) + result.Html };
            }

            result = result with { Html = result.Html + GetOptionalDetailedLogToAttach() };
            return true; // dummy result for TryCatchAndLogToDnn
        });

        return l.ReturnAsOk(result);
    }

    protected IOutputCache OutputCache => field ??= GetService<IOutputCache>().Init(ModuleId, TabId, Block);
}