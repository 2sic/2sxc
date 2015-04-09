using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.SexyContent.DataImportExport;
using ToSic.SexyContent.DataImportExport.Options;

namespace ToSic.SexyContent.Administration
{
    public partial class DataExport : SexyControlAdminBase
    {
        public int ApplicationId
        {
            get
            {
                if (ViewState["ApplicationId"] == null)
                {
                    return 0;
                }
                return (int)ViewState["ApplicationId"];
            }
            set
            {
                ViewState["ApplicationId"] = value;
            }
        }

        public List<string> Languages
        {
            get
            {
                if (ViewState["Languages"] == null)
                {
                    return new List<string>();
                }
                return ViewState["Languages"] as List<string>;
            }
            set
            {
                ViewState["Languages"] = value;
            }
        }

        public string LanguageFallback
        {
            get
            {
                if (ViewState["LanguageFallback"] == null)
                {
                    return null;
                }
                return (string)ViewState["LanguageFallback"];
            }
            set
            {
                ViewState["LanguageFallback"] = value;
            }
        }

        public string LanguageSelected
        {
            get { return ddlLanguage.SelectedValue; }
        }

        public LanguageMissingExport LanguageMissingOptionSelected
        {
            get { return ParseEnum<LanguageMissingExport>(rblLanguageMissing.SelectedValue); }
        }

        public LanguageReferenceExport LanguageReferenceOptionSelected
        {
            get { return ParseEnum<LanguageReferenceExport>(rblLanguageReference.SelectedValue); }
        }

        public ResourceReferenceExport ResourceReferenceOptionSelected
        {
            get { return ParseEnum<ResourceReferenceExport>(rblResourceReference.SelectedValue); }
        }

        public RecordExport RecordExportOptionSelected
        {
            get { return ParseEnum<RecordExport>(rblRecordExport.SelectedValue); }
        }

        public int ContentTypeIdSelected
        {
            get { return int.Parse(ddlContentType.SelectedValue); }
        }

        public string ContentTypeNameSelected
        {
            get { return ddlContentType.SelectedItem.Text; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            ApplicationId = Request["AppID"] != null ? int.Parse(Request["AppID"]) : 0;

            var sexyContent = new SexyContent(ZoneId.Value, ApplicationId);

            LanguageFallback = PortalSettings.DefaultLanguage;
            Languages = sexyContent.ContentContext.GetLanguages()
                                                  .Where(language => language.Active)
                                                  .Select(language => language.ExternalKey)
                                                  .OrderBy(language => language != LanguageFallback)
                                                  .ThenBy(language => language)
                                                  .ToList();

            ddlLanguage.DataSource = Languages;
            ddlLanguage.DataBind();
            
            ddlContentType.DataSource = sexyContent.GetAvailableContentTypes(SexyContent.AttributeSetScope);
            ddlContentType.DataBind();

            rblRecordExport.DataSource = EnumToDataSource<RecordExport>();
            rblRecordExport.DataBind();
            rblRecordExport.SelectedValue = RecordExport.All.ToString();

            rblLanguageMissing.DataSource = EnumToDataSource<LanguageMissingExport>();
            rblLanguageMissing.DataBind();
            rblLanguageMissing.SelectedValue = LanguageMissingExport.Create.ToString();

            rblLanguageReference.DataSource = EnumToDataSource<LanguageReferenceExport>();
            rblLanguageReference.DataBind();
            rblLanguageReference.SelectedValue = LanguageReferenceExport.Link.ToString();

            rblResourceReference.DataSource = EnumToDataSource<ResourceReferenceExport>();
            rblResourceReference.DataBind();
            rblResourceReference.SelectedValue = ResourceReferenceExport.Link.ToString();
        }


        protected void OnRecordExportSelectedIndexChanged(object sender, EventArgs e)
        {
            var recordExportOption = ParseEnum<RecordExport>(rblRecordExport.SelectedValue);
            pnlExportReferenceOptions.Enabled = !recordExportOption.IsBlank();
        }

        protected void OnExportDataClick(object sender, EventArgs e)
        {
            var dataXml = default(string);
            var dataSerializer = new XmlExport();
            
            if (RecordExportOptionSelected.IsBlank())
            {
                dataXml = dataSerializer.CreateBlankXml(ZoneId.Value, ApplicationId, ContentTypeIdSelected);
            }
            else
            {
                dataXml = dataSerializer.CreateXml(ZoneId.Value, ApplicationId, ContentTypeIdSelected, LanguageSelected, LanguageFallback, Languages, LanguageReferenceOptionSelected, ResourceReferenceOptionSelected);
            }


            var fileName = string.Format
                (
                    "2sxc {0} {1} {2} {3}.xml",
                    ContentTypeNameSelected.Replace(" ", "-"),
                    LanguageSelected.Replace(" ", "-"),
                    RecordExportOptionSelected.IsBlank() ? "Template" : "Data",
                    DateTime.Now.ToString("yyyyMMddHHmmss")
                );
            Response.Clear();
            Response.Write(dataXml);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", fileName));
            Response.AddHeader("Content-Length", dataXml.Length.ToString());
            Response.ContentType = "text/xml";
            Response.End();
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