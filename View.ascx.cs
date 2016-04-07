using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public partial class View : SexyViewContentOrApp, IActionable
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ContentBlock = new ModuleContentBlock(ModuleConfiguration);
        }


        /// <summary>
        /// Page Load event - preload template chooser if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // always do this, part of the guarantee that everything will work
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            if (!UserMayEditThisModule) return;

            #region If logged in, inject Edit JavaScript, and delete / add items
            // register scripts and css
            try
            {
                var renderHelp = new RenderingHelpers(_sxcInstance);
                renderHelp.RegisterClientDependencies(Page, string.IsNullOrEmpty(Request.QueryString["debug"]));
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
            #endregion
        }

        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                // check things if it's a module of this portal (ensure everything is ok, etc.)
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;
                if (!isSharedModule && !ContentBlock.ContentGroupExists)
                    new DnnStuffToRefactor().EnsurePortalIsConfigured(_sxcInstance, Server, ControlPath);
                ProcessView();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void ProcessView()
        {
            // If standalone is specified, output just the template without anything else
            var renderedTemplate = GetRenderedTemplateOrJson();

            if (Request.QueryString["standalone"] == "true")
                SendJsonFeedAndCloseRequest(renderedTemplate);
            else
                phOutput.Controls.Add(new LiteralControl(renderedTemplate));
        }

        private void SendJsonFeedAndCloseRequest(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}