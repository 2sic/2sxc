using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;

namespace ToSic.SexyContent
{
    /// <summary>
    /// This class contains code that are both used in ViewApp.ascx.cs and View.ascx.cs
    /// </summary>
    public class SexyViewContentOrApp : SexyControlEditBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            try
            {

                // If logged in, inject Edit JavaScript, and delete / add items
                if (UserMayEditThisModule && Sexy != null)
                {
                    ClientScriptManager ClientScript = Page.ClientScript;
                    ClientScript.RegisterClientScriptInclude("ViewEdit", ResolveClientUrl("~/DesktopModules/ToSIC_SexyContent/Js/ViewEdit.js"));

                    ((DotNetNuke.UI.Modules.ModuleHost)this.Parent).Attributes.Add("data-2sxc", (new
                    {
                        moduleId = ModuleId,
                        manage = new
                        {
                            isEditMode = UserMayEditThisModule,
                            config = new
                            {
                                portalId = PortalId,
                                tabId = TabId,
                                moduleId = ModuleId,
                                contentGroupId = Elements.Any() ? Elements.First().GroupId : -1,
                                dialogUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId),
                                returnUrl = Request.RawUrl,
                                appPath = AppId.HasValue ? Sexy.App.Path : null,
                                cultureDimension = Sexy.GetCurrentLanguageID(),
                                isList = Template != null && Template.UseForList
                            }
                        }
                    }).ToJson());

                    ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.api.js", 90);
                    ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.api.manage.js", 91);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

        }

        /// <summary>
        /// Get the content data and render it with the given template to the page.
        /// </summary>
        protected void ProcessView(PlaceHolder phOutput, Panel pnlError, Panel pnlMessage)
        {
            #region Check if everything has values and return if not

            if (Template == null)
            {
                ShowError(LocalizeString("TemplateConfigurationMissing.Text"), pnlError);
                return;
            }

            if (DataSource.GetCache(ZoneId.Value, AppId.Value).GetContentType(Template.AttributeSetID) == null)
            {
                ShowError("The contents of this module cannot be displayed because it's located in another VDB.", pnlError);
                return;
            }

            if (Elements.All(e => e.Content == null))
            {
                var toolbar = IsEditable ? "<ul class='sc-menu' data-toolbar='" + new { sortOrder = Elements.First().SortOrder, useModuleList = true, action = "edit" }.ToJson() + "'></ul>" : "";
                ShowMessage(LocalizeString("NoDemoItem.Text") + " " + toolbar, pnlMessage);
                return;
            }

            #endregion

            try
            {

                string renderedTemplate = "";
                var engine = EngineFactory.CreateEngine(Template);
                var dataSource = (ViewDataSource)Sexy.GetViewDataSource(this.ModuleId, SexyContent.HasEditPermission(this.ModuleConfiguration), DotNetNuke.Common.Globals.IsEditMode());
                engine.Init(Template, Sexy.App, this.ModuleConfiguration, dataSource, Request.QueryString["type"] == "data" ? InstancePurposes.PublishData : InstancePurposes.WebView, Sexy);
                engine.CustomizeData();

                // Output JSON data if type=data in URL
                if (Request.QueryString["type"] == "data")
                {
                    if (dataSource.Publish.Enabled)
                    {
                        var publishedStreams = dataSource.Publish.Streams;
                        renderedTemplate = Sexy.GetJsonFromStreams(dataSource, publishedStreams.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        Response.StatusCode = 403;
                        var moduleTitle = new ModuleController().GetModule(ModuleId).ModuleTitle;
                        renderedTemplate = (new { error = "2sxc Content (" + ModuleId + "): " + String.Format(LocalizeString("EnableDataPublishing.Text"), ModuleId, moduleTitle) }).ToJson();
                        Response.TrySkipIisCustomErrors = true;
                    }
                    Response.ContentType = "application/json";
                }
                else
                {
                    renderedTemplate = engine.Render();
                }

                // If standalone is specified, output just the template without anything else
                if (StandAlone)
                {
                    Response.Clear();
                    Response.Write(renderedTemplate);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                    phOutput.Controls.Add(new LiteralControl(renderedTemplate));
            }
            // Catch errors; log them
            catch (Exception Ex)
            {
                ShowError(LocalizeString("TemplateError.Text") + ": " + HttpUtility.HtmlEncode(Ex.ToString()), pnlError, LocalizeString("TemplateError.Text"), false);
                Exceptions.LogException(Ex);
            }
        }

        #region Show Message or Errors
        protected void ShowMessage(string Message, Panel pnlMessage, bool ShowOnlyEdit = true)
        {
            if (!ShowOnlyEdit || UserMayEditThisModule)
            {
                pnlMessage.Controls.Add(new LiteralControl(Message));
                pnlMessage.Visible = true;
            }
        }

        protected void ShowError(string Error, Panel pnlError, string NonHostError = "", bool ShowOnlyEdit = true)
        {
            if (String.IsNullOrEmpty(NonHostError))
                NonHostError = Error;

            if (!ShowOnlyEdit || UserMayEditThisModule)
            {
                pnlError.Controls.Add(new LiteralControl(UserInfo.IsSuperUser ? Error : NonHostError));
                pnlError.Visible = true;
            }
        }
        #endregion

    }
}