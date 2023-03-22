using System;
using System.Linq;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web.LightSpeed;

namespace ToSic.Sxc.Dnn
{
    public partial class View : PortalModuleBase, IActionable
    {
        #region GetService and Service Provider

        /// <summary>
        /// Get the service provider only once - ideally in Dnn9.4 we will get it from Dnn
        /// If we would get it multiple times, there are edge cases where it could be different each time! #2614
        /// </summary>
        private IServiceProvider ServiceProvider => _serviceProvider.Get(DnnStaticDi.CreateModuleScopedServiceProvider);
        private readonly GetOnce<IServiceProvider> _serviceProvider = new GetOnce<IServiceProvider>();
        private TService GetService<TService>() => ServiceProvider.Build<TService>(Log);

        #endregion

        /// <summary>
        /// Block needs to self-initialize when first requested, because it's used in the Actions-Menu builder
        /// which runs before page-load
        /// </summary>
        private IBlock Block => _blockGetOnce.Get(Log, () => LogTimer.DoInTimer(() => GetService<IModuleAndBlockBuilder>().GetProvider(ModuleConfiguration, null).LoadBlock()), timer: true);
        private readonly GetOnce<IBlock> _blockGetOnce = new GetOnce<IBlock>();

        #region Logging

        private ILog Log { get; } = new Log("Sxc.View");

        protected ILogCall LogTimer => _logTimer.Get(() => Log.Fn(message: $"Page:{TabId} '{Page?.Title}', Module:{ModuleId} '{ModuleConfiguration.ModuleTitle}'"));
        private readonly GetOnce<ILogCall> _logTimer = new GetOnce<ILogCall>();

        #endregion

        /// <summary>
        /// Page Load event
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            LogTimer.DoInTimer(() =>
            {
                // add to insights-history for analytic
                GetService<ILogStore>().Add("module", Log);
                //LogTimer.Timer.Start();

                Log.Do(timer: true, action: () =>
                {
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
                        DnnClientResources = GetService<DnnClientResources>().Init(Page, null, requiresPre1025Behavior == false ? null : block?.BlockBuilder);
                        var needsPre1025Behavior = requiresPre1025Behavior ?? DnnClientResources.NeedsPre1025Behavior();
                        if (needsPre1025Behavior) DnnClientResources.EnforcePre1025Behavior();
                        // #lightspeed
                        if (OutputCache?.Existing != null)
                            OutputCache.Fresh.EnforcePre1025 = needsPre1025Behavior;
                        return true; // dummy result
                    });
                });
            });
        }

        protected DnnClientResources DnnClientResources;


        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e) => Log.Do(() =>
        {
            var finalMessage = "";
            LogTimer.DoInTimer(() =>
            {
                // #lightspeed
                if (OutputCache?.Existing != null) Log.A("Lightspeed hit - will use cached");

                IRenderResult data = null;
                var headersAndScriptsAdded = false;

                // skip this if something before this caused an error
                if (!IsError)
                    TryCatchAndLogToDnn(() =>
                    {
                        // Try to build the html and everything
                        data = OutputCache?.Existing?.Data;

                        finalMessage = OutputCache?.IsEnabled != true ? "" : data != null ? "⚡⚡" : "⚡⏳";

                        data = data ?? RenderViewAndGatherJsCssSpecs();
                        // in this case assets & page settings were not applied
                        try
                        {
                            var pageChanges = GetService<DnnPageChanges>();
                            pageChanges.Apply(Page, data);
                        }
                        catch
                        {
                            /* ignore */
                        }

                        // call this after rendering templates, because the template may change what resources are registered
                        DnnClientResources.AddEverything(data.Features);
                        headersAndScriptsAdded = true; // will be true if we make it this far
                        // If standalone is specified, output just the template without anything else
                        if (RenderNaked)
                            SendStandalone(data.Html);
                        else
                            phOutput.Controls.Add(new LiteralControl(data.Html));

                        // #Lightspeed
                        OutputCache?.Save(data);
                        return true; // dummy result
                    });

                // if we had an error before, or have one now, re-check assets
                if (IsError && !headersAndScriptsAdded)
                    DnnClientResources?.AddEverything(data?.Features);
            });
            LogTimer.Done(IsError ? "⚠️" : finalMessage);
        });

        private IRenderResult RenderViewAndGatherJsCssSpecs() => Log.Func(timer: true, message: $"module {ModuleId} on page {TabId}", func: () =>
        {
            var result = new RenderResult();
            TryCatchAndLogToDnn(() =>
            {
                if (RenderNaked) Block.BlockBuilder.WrapInDiv = false;
                result = Block.BlockBuilder.Run(true) as RenderResult;

                if (result.Errors?.Any() ?? false)
                {
                    var warnings = result.Errors.Select(e =>
                        Block.BlockBuilder.RenderingHelper.DesignError(e));

                    result.Html = string.Join("", warnings) + result.Html;
                }

                result.Html += GetOptionalDetailedLogToAttach();
                return true; // dummy result
            });

            return result;
        });



        protected IOutputCache OutputCache => _oc.Get(Log, () => GetService<IOutputCache>().Init(ModuleId, TabId, Block));
        private readonly GetOnce<IOutputCache> _oc = new GetOnce<IOutputCache>();
    }
}