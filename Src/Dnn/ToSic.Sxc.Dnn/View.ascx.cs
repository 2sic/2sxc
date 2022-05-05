using System;
using System.Diagnostics;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web.LightSpeed;

namespace ToSic.Sxc.Dnn
{
    public partial class View : PortalModuleBase, IActionable
    {
        /// <summary>
        /// Get the service provider only once - ideally in Dnn9.4 we will get it from Dnn
        /// If we would get it multiple times, there are edge cases where it could be different each time! #2614
        /// </summary>
        private IServiceProvider ServiceProvider => _serviceProvider ?? (_serviceProvider = DnnStaticDi.CreateModuleScopedServiceProvider());
        private IServiceProvider _serviceProvider;
        private TService GetService<TService>() => ServiceProvider.Build<TService>();

        /// <summary>
        /// Block needs to self-initialize when first requested, because it's used in the Actions-Menu builder
        /// which runs before page-load
        /// </summary>
        private IBlock Block => _block ?? (_block = GetService<IModuleAndBlockBuilder>().Init(Log).GetBlock(ModuleConfiguration));
        private IBlock _block;

        private ILog Log { get; } = new Log("Sxc.View");
        private Stopwatch _stopwatch;
        private Action<string> _entireLog;


        /// <summary>
        /// Page Load event
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // add to insights-history for analytic
            GetService<LogHistory>().Add("module", Log);
            _stopwatch = Stopwatch.StartNew();
            _entireLog = Log.Call(message: $"Page:{TabId} '{Page?.Title}', Module:{ModuleId} '{ModuleConfiguration.ModuleTitle}'", useTimer: true);
            var callLog = Log.Call(useTimer: true);

            // todo: this should be dynamic at some future time, because normally once it's been checked, it wouldn't need checking again
            var checkPortalIsReady = true;
            bool? requiresPre1025Behavior = null; // null = auto-detect, true/false

            #region Lightspeed
            try
            {
                if (OutputCache.Existing != null)
                {
                    checkPortalIsReady = false;
                    requiresPre1025Behavior = OutputCache.Existing.EnforcePre1025;
                }
            }
            catch
            {
                /* ignore */
            }
            #endregion

            // Always do this, part of the guarantee that everything will work
            // new mechanism in 10.25
            // this must happen in Page-Load, so we know what supporting scripts to add
            // at this stage of the lifecycle
            // We moved this to Page_Load because RequestAjaxAntiForgerySupport didn't work in later events
            // ensure everything is ready and that we know if we should activate the client-dependency
            TryCatchAndLogToDnn(() =>
            {
                if (checkPortalIsReady) DnnReadyCheckTurbo
                    .EnsureSiteAndAppFoldersAreReady(this, Block, GetService<Lazy<AppFolderInitializer>>(), Log);
                DnnClientResources = GetService<DnnClientResources>()
                    .Init(Page, requiresPre1025Behavior == false ? null : Block?.BlockBuilder, Log);
                var needsPre1025Behavior = requiresPre1025Behavior ?? DnnClientResources.NeedsPre1025Behavior();
                if (needsPre1025Behavior) DnnClientResources.EnforcePre1025Behavior();
                // #lightspeed
                try
                {
                    OutputCache.Fresh.EnforcePre1025 = needsPre1025Behavior;
                } catch { /* ignore */ }
            }, callLog);
            _stopwatch.Stop();
        }

        protected DnnClientResources DnnClientResources;


        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            _stopwatch?.Start();
            var callLog = Log.Call(useTimer: true);

            // #lightspeed
            if (OutputCache?.Existing != null) Log.Add("Lightspeed hit - will use cached");

            IRenderResult data = null;
            var headersAndScriptsAdded = false;

            var finalMessage = "";
            // skip this if something before this caused an error
            if (!IsError)
                TryCatchAndLogToDnn(() =>
                {
                    // Try to build the html and everything
                    data = OutputCache?.Existing?.Data;

                    finalMessage = OutputCache?.IsEnabled != true ? "" :  data != null ? "⚡⚡" : "⚡⏳";

                    data = data ?? RenderViewAndGatherJsCssSpecs();
                    // in this case assets & page settings were not applied
                    try
                    {
                        var pageChanges = GetService<DnnPageChanges>();
                        pageChanges.Apply(Page, data); // note: if Assets == null, it will take the default
                    }
                    catch{ /* ignore */ }

                    // todo: #Lightspeed Page property changes!

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
                });

            // if we had an error before, or have one now, re-check assets
            if (IsError && !headersAndScriptsAdded)
                DnnClientResources?.AddEverything(data?.Features);

            callLog(null);
            _stopwatch?.Stop();
            _entireLog?.Invoke(IsError ? "⚠️" : finalMessage);
        }

        private IRenderResult RenderViewAndGatherJsCssSpecs()
        {
            var timerWrap = Log.Call(message: $"module {ModuleId} on page {TabId}", useTimer: true);
            var result = new RenderResult();
            TryCatchAndLogToDnn(() =>
            {
                if (RenderNaked) Block.BlockBuilder.WrapInDiv = false;
                result = Block.BlockBuilder.Run(true) as RenderResult;
                result.Html += GetOptionalDetailedLogToAttach();
            }, timerWrap);

            return result;
        }



        protected IOutputCache OutputCache => _oc.Get(() => GetService<IOutputCache>().Init(Log).Init(ModuleId, Block));
        private readonly ValueGetOnce<IOutputCache> _oc = new ValueGetOnce<IOutputCache>();
    }
}