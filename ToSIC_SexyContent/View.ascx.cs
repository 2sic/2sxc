using System;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        private BlockBuilder _blockBuilder;
        private bool _cmsBlockLoaded;
        internal bool IsError;

        protected BlockBuilder BlockBuilder
        {
            get
            {
                if (_cmsBlockLoaded) return _blockBuilder;
                _cmsBlockLoaded = true;
                _blockBuilder = new BlockFromModule(
                        new DnnContainer(ModuleConfiguration),
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
            // add to insights-history for analytics
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

        public bool RenderNaked 
            => _renderNaked ?? (_renderNaked = Request.QueryString["standalone"] == "true").Value;
        private bool? _renderNaked;

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

        /// <summary>
        /// optional detailed logging
        /// </summary>
        /// <returns></returns>
        private string GetOptionalDetailedLogToAttach()
        {
            try
            {
                // if in debug mode and is super-user (or it's been enabled for all), then add to page debug
                if (Request.QueryString["debug"] == "true")
                    if (UserInfo.IsSuperUser
                        || DnnLogging.EnableLogging(GlobalConfiguration.Configuration.Properties))
                        return HtmlLog();
            }
            catch { /* ignore */ }

            return "";
        }

        private void EnsureCmsBlockAndPortalIsReady()
        {
            var timerWrap = Log.Call(message: $"module {ModuleId} on page {TabId}", useTimer: true);
            // throw better error if SxcInstance isn't available
            // not sure if this doesn't have side-effects...
            if (BlockBuilder == null)
                throw new Exception("Error - can't find 2sxc instance configuration. " +
                                    "Probably trying to show an app or content that has been deleted.");

            // check things if it's a module of this portal (ensure everything is ok, etc.)
            var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;
            if (!isSharedModule && !BlockBuilder.Block.ContentGroupExists && BlockBuilder.App != null)
                new DnnTenantSettings().EnsureTenantIsConfigured(BlockBuilder, Server, ControlPath);

            timerWrap(null);
        }


        private void SendStandalone(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private string HtmlLog() 
            => Log.Dump(" - ", "<!-- 2sxc insights for " + ModuleId + "\n", "-->");


        private void TryCatchAndLogToDnn(Action action, Action<string> timerWrap = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                IsError = true;
                try
                {
                    var msg = BlockBuilder.RenderingHelper.DesignErrorMessage(ex, true, null, false, true);
                    var wrappedMsg = BlockBuilder.UserMayEdit ? BlockBuilder.WrapInDivWithContext(msg) : msg;
                    phOutput.Controls.Add(new LiteralControl(wrappedMsg));
                } 
                catch { /* ignore */  }
            }
            finally
            {
                timerWrap?.Invoke(null);
            }
        }



    }
}