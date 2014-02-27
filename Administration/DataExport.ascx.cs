using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.Eav;
using ToSic.SexyContent;

namespace ToSic.SexyContent.Administration
{
    public partial class DataExport : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        public int? ApplicationId
        {
            get
            {
                if (ViewState["ApplicationId"] == null)
                {
                    return new int?();
                }
                return (int?)ViewState["ApplicationId"];
            }
            set
            {
                ViewState["ApplicationId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            ApplicationId = Request["AppID"] != null ? int.Parse(Request["AppID"]) : new int?();

            var sexyContent = new SexyContent(true, new int?(), ApplicationId);
            
            ddlContentType.DataSource = sexyContent.GetAvailableAttributeSets(); ;
            ddlContentType.DataBind();

            ddlLanguage.DataSource = sexyContent.ContentContext.GetLanguages();
            ddlLanguage.DataBind();
        }

        protected void OnExportDataClick(object sender, EventArgs e)
        {
            //var dataExporter = new DataExporter();
            //dataExporter.ExportData
            //    (
            //        ApplicationId, 
            //        ddlContentType.SelectedValue, 
            //        ddlLanguage.SelectedValue, ...
            //    );

            // TODO2tk: Show a popup export done
            // TODO2tk: Export data (create a separate class for export and import)
        }

        protected void OnExportEmptyClick(object sender, EventArgs e)
        {

        }
    }
}