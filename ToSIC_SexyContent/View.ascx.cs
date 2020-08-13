using System;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        private BlockBuilder _blockBuilder;
        private bool _cmsBlockLoaded;

        protected BlockBuilder BlockBuilder
        {
            get
            {
                if (_cmsBlockLoaded) return _blockBuilder;
                _cmsBlockLoaded = true;
                _blockBuilder = new BlockFromModule(
                        new DnnContainer(ModuleConfiguration, Log),
                        Log,
                        new DnnTenant(new PortalSettings(ModuleConfiguration.OwnerPortalID)))
                    .BlockBuilder as BlockBuilder;
                return _blockBuilder;
            }
        }

        private ILog Log { get; } = new Log("Sxc.View");

        /// <summary>
        /// Page Load event
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // add to insights-history for analytic
            History.Add("module", Log);

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
                DnnClientResources = new DnnClientResources(Page, BlockBuilder, Log);
                DnnClientResources.EnsurePre1025Behavior();
            });
        }

        protected DnnClientResources DnnClientResources;


        /// <summary>s
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // skip this if something before this caused an error
            if (IsError) return;

            // Try to build the html
            TryCatchAndLogToDnn(() =>
            {
                var html = RenderViewAndGatherJsCssSpecs();
                // call this after rendering templates, because the template may change what resources are registered
                DnnClientResources.AddEverything();
                // If standalone is specified, output just the template without anything else
                if (RenderNaked)
                    SendStandalone(html);
                else
                    phOutput.Controls.Add(new LiteralControl(html));
            });
        }

        private string RenderViewAndGatherJsCssSpecs()
        {
            var renderedTemplate = "";
            var timerWrap = Log.Call(message: $"module {ModuleId} on page {TabId}", useTimer: true);
            TryCatchAndLogToDnn(() =>
            {
                if (RenderNaked) BlockBuilder.RenderWithDiv = false;
                renderedTemplate = BlockBuilder.Render()
                    + GetOptionalDetailedLogToAttach();
            }, timerWrap);

            return renderedTemplate;
        }
    }
}