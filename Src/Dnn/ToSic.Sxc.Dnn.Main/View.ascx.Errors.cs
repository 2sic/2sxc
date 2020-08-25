using System;
using System.Web.UI;
using DotNetNuke.Services.Exceptions;
using ToSic.Sxc.Dnn.Install;

namespace ToSic.SexyContent
{
    public partial class View
    {
        internal bool IsError;

        /// <summary>
        /// Verify that the portal is ready, otherwise show a good error
        /// </summary>
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
            var block = Block;
            if (!isSharedModule && !block.ContentGroupExists && block.App != null)
                new DnnTenantSettings().EnsureTenantIsConfigured(block, Server);

            timerWrap(null);
        }

        /// <summary>
        /// Run some code in a try/catch, and output it nicely if an error is thrown
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timerWrap"></param>
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
                    // Log to DNN
                    Exceptions.ProcessModuleLoadException(this, ex, false);

                    // Try to show nice message on screen
                    var msg = BlockBuilder.RenderingHelper.DesignErrorMessage(ex, true, null, false, true);
                    var wrappedMsg = Block.EditAllowed ? BlockBuilder.WrapInDivWithContext(msg) : msg;
                    phOutput.Controls.Add(new LiteralControl(wrappedMsg));
                }
                catch
                {
                    phOutput.Controls.Add(new LiteralControl("Something went really wrong in view.ascx - check error logs"));
                }
            }
            finally
            {
                timerWrap?.Invoke(null);
            }
        }

    }
}