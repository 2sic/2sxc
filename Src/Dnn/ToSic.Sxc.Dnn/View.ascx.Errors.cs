using System;
using System.Web.UI;
using DotNetNuke.Services.Exceptions;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Dnn
{
    public partial class View
    {
        internal bool IsError;

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
                    // 1. Log to DNN
                    Exceptions.ProcessModuleLoadException(this, ex, false);

                    // 2. Try to show nice message on screen

                    // first get a rendering helper - but since BlockBuilder may be null, create a new one
                    var renderingHelper = GetService<IRenderingHelper>().Init(Block, Log);
                    var msg = renderingHelper.DesignErrorMessage(ex, true, null, false, true);
                    try
                    {
                        if (Block.Context.UserMayEdit)
                            msg = renderingHelper.WrapInContext(msg,
                                instanceId: Block.ParentId,
                                contentBlockId: Block.ContentBlockId,
                                editContext: true);
                    }
                    catch { /* ignore */ }

                    phOutput.Controls.Add(new LiteralControl(msg));
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