using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.Eav;
using ToSic.SexyContent;
using ToSic.SexyContent.DataImportExport;

namespace ToSic.SexyContent.Administration
{
    public partial class DataExport : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        // NOTE2tk: Find the dialog on http://2sexycontent.2tk.2sic/Home/tabid/55/ctl/dataexport/mid/388/Default.aspx?popUp=true
        
        // TODO2tk: Export blank data templates
        // TODO2tk: Get dimensions from DNN
        
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

        public Dimension DefaultDimension
        {
            get
            {
                if (ViewState["DefaultDimensionId"] == null)
                {
                    return null;
                }
                return (Dimension)ViewState["DefaultDimensionId"];
            }
            set
            {
                ViewState["DefaultDimensionId"] = value;
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

            DefaultDimension = sexyContent.ContentContext.Dimensions.FirstOrDefault(dim => dim.ExternalKey == PortalSettings.DefaultLanguage);

            ddlContentType.DataSource = sexyContent.GetAvailableAttributeSets(); ;
            ddlContentType.DataBind();

            ddlLanguage.DataSource = sexyContent.ContentContext.Dimensions.Where(dim => dim.Active);
            ddlLanguage.DataBind();

            rblRecordExport.DataSource = EnumToDataSource<RecordExportOption>();
            rblRecordExport.DataBind();
            rblRecordExport.SelectedValue = RecordExportOption.All.ToString();

            rblLanguageMissing.DataSource = EnumToDataSource<LanguageMissingOption>();
            rblLanguageMissing.DataBind();
            rblLanguageMissing.SelectedValue = LanguageMissingOption.Create.ToString();

            rblLanguageReference.DataSource = EnumToDataSource<LanguageReferenceOption>();
            rblLanguageReference.DataBind();
            rblLanguageReference.SelectedValue = LanguageReferenceOption.Resolve.ToString();

            rblResourceReference.DataSource = EnumToDataSource<ResourceReferenceOption>();
            rblResourceReference.DataBind();
            rblResourceReference.SelectedValue = ResourceReferenceOption.Resolve.ToString();
        }


        protected void OnRecordExportSelectedIndexChanged(object sender, EventArgs e)
        {
            var recordExportOption = ParseEnum<RecordExportOption>(rblRecordExport.SelectedValue);
            pnlExportReferenceOptions.Enabled = !recordExportOption.IsBlank();
        }

        protected void OnExportDataClick(object sender, EventArgs e)
        {
            var dataSerializer = new DataXmlSerializer();
            var dataXml = dataSerializer.Serialize
                (
                    ApplicationId,
                    int.Parse(ddlContentType.SelectedValue),
                    int.Parse(ddlLanguage.SelectedValue),
                    DefaultDimension.DimensionID,
                    ParseEnum<LanguageMissingOption>(rblLanguageMissing.SelectedValue),
                    ParseEnum<LanguageReferenceOption>(rblLanguageReference.SelectedValue),
                    ParseEnum<ResourceReferenceOption>(rblResourceReference.SelectedValue)
                );

            Response.Clear();
            Response.Write(dataXml);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=SexyContentData {0}.xml", ddlContentType.SelectedItem.Text));
            Response.AddHeader("Content-Length", dataXml.Length.ToString());
            Response.ContentType = "text/xml";
            Response.End();
        }

        protected void OnExportEmptyClick(object sender, EventArgs e)
        {
            // TODO2tk: ...
        }

        private IEnumerable<dynamic> EnumToDataSource<T>() where T : struct
        {
            return from Enum enumValue in Enum.GetValues(typeof(T))
                   select new
                   {
                       Value = enumValue,
                       Name = LocalizeString(typeof(T).Name + enumValue) // For example: LanguageReferenceOptionLink
                   };
        }

        private T ParseEnum<T>(string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}