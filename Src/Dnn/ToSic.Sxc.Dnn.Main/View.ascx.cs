using System;
using System.Diagnostics;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;


namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        protected IBlock Block
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var newCtx = Factory.StaticBuild<IContextOfBlock>().Init(ModuleConfiguration, Log);
                return _block = Factory.StaticBuild<BlockFromModule>().Init(newCtx, Log);
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
                EnsureCmsBlockAndPortalIsReady();
                DnnClientResources = Factory.StaticBuild<DnnClientResources>().Init(Page, Block?.BlockBuilder, Log);
                DnnClientResources.EnsurePre1025Behavior();
            }, callLog);
            _stopwatch.Stop();
        }

        protected DnnClientResources DnnClientResources;


        /// <summary>s
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            _stopwatch?.Start();
            var callLog = Log.Call(useTimer: true);
            var headersAndScriptsAdded = false;
            // skip this if something before this caused an error
            if (!IsError)
                TryCatchAndLogToDnn(() =>
                {
                    // Try to build the html
                    var html = RenderViewAndGatherJsCssSpecs();
                    // call this after rendering templates, because the template may change what resources are registered
                    headersAndScriptsAdded = DnnClientResources.AddEverything();
                    // If standalone is specified, output just the template without anything else
                    if (RenderNaked)
                        SendStandalone(html);
                    else
                        phOutput.Controls.Add(new LiteralControl(html));
                });

            // if we had an error before, or have one now, re-check assets
            if (IsError && !headersAndScriptsAdded)
                DnnClientResources?.AddEverything();
            callLog(null);
            _stopwatch?.Stop();
            _entireLog?.Invoke("✔"); //$"⌚ {_stopwatch?.ElapsedMilliseconds:##.##}ms");
        }

        private string RenderViewAndGatherJsCssSpecs()
        {
            var timerWrap = Log.Call(message: $"module {ModuleId} on page {TabId}", useTimer: true);
            var renderedTemplate = "";
            TryCatchAndLogToDnn(() =>
            {
                if (RenderNaked) Block.BlockBuilder.WrapInDiv = false;
                renderedTemplate = Block.BlockBuilder.Render()
                    + GetOptionalDetailedLogToAttach();
            }, timerWrap);

            return renderedTemplate;
        }
    }
}