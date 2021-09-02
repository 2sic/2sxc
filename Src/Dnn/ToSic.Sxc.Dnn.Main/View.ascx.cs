using System;
using System.Diagnostics;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Beta.LightSpeed;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn
{
    public partial class View : PortalModuleBase, IActionable
    {
        protected IBlock Block
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var newCtx = Eav.Factory.StaticBuild<IContextOfBlock>().Init(ModuleConfiguration, Log);
                return _block = Eav.Factory.StaticBuild<BlockFromModule>().Init(newCtx, Log);
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

        private ILog Log { get; } = new Log("Sxc.View");
        private Stopwatch _stopwatch;
        private Action<string> _entireLog;
        /// <summary>
        /// Page Load event
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // add to insights-history for analytic
            History.Add("module", Log);
            _stopwatch = Stopwatch.StartNew();
            _entireLog = Log.Call(message: $"Page:{TabId} '{Page?.Title}', Instance:{ModuleId} '{ModuleConfiguration.ModuleTitle}'", useTimer: true);
            var callLog = Log.Call(useTimer: true);

            // todo: this should be dynamic at some future time, because normally once it's been checked, it wouldn't need checking again
            var checkPortalIsReady = true;
            bool? requiresPre1025Behavior = null; // null = auto-detect, true/false

            #region Lightspeed - very experimental - deactivate before distribution
            if (Lightspeed.HasCache(ModuleId))
            {
                Log.Add("Lightspeed enabled, has cache");
                PreviousCache = Lightspeed.Get(ModuleId);
                if (PreviousCache != null)
                {
                    checkPortalIsReady = false;
                    requiresPre1025Behavior = PreviousCache.EnforcePre1025;
                }
            }
            if(PreviousCache == null) NewCache = new OutputCacheItem();
            #endregion

            // always do this, part of the guarantee that everything will work
            // 2020-01-06 2sxc 10.25 - moved away to DnnRenderingHelpers
            // to only load when we're actually activating the JS.
            // might be a breaking change for some code that "just worked" before
            //ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            // new mechanism in 10.25
            // this must happen in Page-Load, so we know what supporting scripts to add
            // at this stage of the lifecycle
            // We moved this to Page_Load because RequestAjaxAntiForgerySupport didn't work in later events
            // ensure everything is ready and that we know if we should activate the client-dependency
            TryCatchAndLogToDnn(() =>
            {
                if (checkPortalIsReady) new DnnReadyCheckTurbo(this, Log).EnsureSiteAndAppFoldersAreReady(Block);
                DnnClientResources = Eav.Factory.StaticBuild<DnnClientResources>()
                    .Init(Page, requiresPre1025Behavior == false ? null : Block?.BlockBuilder, Log);
                var needsPre1025Behavior = requiresPre1025Behavior ?? DnnClientResources.NeedsPre1025Behavior();
                if (needsPre1025Behavior) DnnClientResources.EnforcePre1025Behavior();
                // #lightspeed
                if(NewCache != null) NewCache.EnforcePre1025 = needsPre1025Behavior;
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
            if (PreviousCache != null) Log.Add("Lightspeed hit - will use cached");

            RenderResultWIP data = null;
            var headersAndScriptsAdded = false;
            // skip this if something before this caused an error
            if (!IsError)
                TryCatchAndLogToDnn(() =>
                {
                    // Try to build the html and everything
                    data = PreviousCache?.Data ?? RenderViewAndGatherJsCssSpecs();
                    // in this case assets & page settings were not applied
                    try
                    {
                        var pageChanges = Eav.Factory.StaticBuild<DnnPageChanges>();
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
                    {
                        phOutput.Controls.Add(new LiteralControl(data.Html));

                        // #Lightspeed
                        if (NewCache != null)
                        {
                            Log.Add("Adding to lightspeed");
                            NewCache.Data = data;
                            Lightspeed.Add(ModuleId, NewCache);
                        }
                    }
                });

            // if we had an error before, or have one now, re-check assets
            if (IsError && !headersAndScriptsAdded)
                DnnClientResources?.AddEverything(data?.Features);

            callLog(null);
            _stopwatch?.Stop();
            _entireLog?.Invoke("✔");
        }

        private RenderResultWIP RenderViewAndGatherJsCssSpecs()
        {
            var timerWrap = Log.Call(message: $"module {ModuleId} on page {TabId}", useTimer: true);
            var result = new RenderResultWIP();
            TryCatchAndLogToDnn(() =>
            {
                if (RenderNaked) Block.BlockBuilder.WrapInDiv = false;
                result = Block.BlockBuilder.Run();
                result.Html += GetOptionalDetailedLogToAttach();
            }, timerWrap);

            return result;
        }


        private OutputCacheManager Lightspeed => _lightspeed ?? (_lightspeed = new OutputCacheManager());
        private OutputCacheManager _lightspeed;
        private OutputCacheItem PreviousCache;
        private OutputCacheItem NewCache;
    }
}