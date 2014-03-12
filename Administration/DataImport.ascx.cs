using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.SexyContent.DataImportExport;

namespace ToSic.SexyContent.Administration
{
    public partial class DataImport : DotNetNuke.Entities.Modules.PortalModuleBase
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

        public int? ZoneId
        {
            get
            {
                if (ViewState["ZoneId"] == null)
                {
                    return new int?();
                }
                return (int?)ViewState["ZoneId"];
            }
            set
            {
                ViewState["ZoneId"] = value;
            }
        }

        public string UserName
        {
            get { return PortalSettings.UserInfo.DisplayName; }
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
            get { return PortalSettings.DefaultLanguage; }
        }

        public EntityCreateImportOption EntityCreateOptionSelected
        {
            get { return ParseEnum<EntityCreateImportOption>(rblEntityCreate.SelectedValue); }
        }

        public EntityClearImportOption EntityClearOptionSelected
        {
            get { return ParseEnum<EntityClearImportOption>(rblEntityClear.SelectedValue); }
        }

        public ResourceReferenceImportOption ResourceReferenceOptionSelected
        {
            get { return ParseEnum<ResourceReferenceImportOption>(rblResourceReference.SelectedValue); }
        }

        public LanguageReferenceImportOption LanguageReferenceOptionSelected
        {
            get { return ParseEnum<LanguageReferenceImportOption>(rblLanguageReference.SelectedValue); }
        }

        public int ContentTypeIdSelected
        {
            get { return int.Parse(ddlContentType.SelectedValue); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            ApplicationId = Request["AppID"] != null ? int.Parse(Request["AppID"]) : new int?();

            var sexyContent = new SexyContent(true, new int?(), ApplicationId);

            ZoneId = sexyContent.GetZoneID(PortalId);
            Languages = sexyContent.ContentContext.GetLanguages()
                                                  .Where(language => language.Active)
                                                  .Select(language => language.ExternalKey)
                                                  .OrderBy(language => language != LanguageFallback)
                                                  .ThenBy(language => language)
                                                  .ToList();

            ddlContentType.DataSource = sexyContent.GetAvailableAttributeSets(); ;
            ddlContentType.DataBind();

            rblEntityCreate.DataSource = EnumToDataSource<EntityCreateImportOption>();
            rblEntityCreate.DataBind();
            rblEntityCreate.SelectedValue = EntityCreateImportOption.Create.ToString();

            rblEntityClear.DataSource = EnumToDataSource<EntityClearImportOption>();
            rblEntityClear.DataBind();
            rblEntityClear.SelectedValue = EntityClearImportOption.None.ToString();

            rblLanguageReference.DataSource = EnumToDataSource<LanguageReferenceImportOption>();
            rblLanguageReference.DataBind();
            rblLanguageReference.SelectedValue = LanguageReferenceImportOption.Keep.ToString();

            rblResourceReference.DataSource = EnumToDataSource<ResourceReferenceImportOption>();
            rblResourceReference.DataBind();
            rblResourceReference.SelectedValue = ResourceReferenceImportOption.Keep.ToString();
        }


        protected void OnContinueClick(object sender, EventArgs e)
        {
            if (!FileUpload.HasFile)
            {
                // TODO2tk: Sow an error message
                return;
            }
           
            var dataStream = FileUpload.FileContent;
            var dataImport = DataXmlImport.Deserialize(dataStream, ApplicationId, ContentTypeIdSelected, Languages, LanguageFallback, EntityCreateOptionSelected, EntityClearOptionSelected, ResourceReferenceOptionSelected);
            if (dataImport.HasErrors)
            {
                // TODO2tk: Show error messages
                lblOutput.Text = "Errors: " + dataImport.ErrorProtocol.ErrorCount;
            }
            else
            {
                // TODO2tk: Show summary
                lblOutput.Text = string.Format
                        (
                            "Elements:{0}<br>Languages:{1}<br>Columns:{2}<br>Create {3} entities<br>Update {4} entities<br>Delete {5} entities<br><br>Summary<br>{6}",
                            dataImport.GetDocumentElementCount(),
                            dataImport.GetLanguagesCount(),
                            dataImport.GetAttributeNames(","),
                            dataImport.GetEntitiesCreateCount(),
                            dataImport.GetEntitiesUpdateCount(),
                            dataImport.GetEntitiesDeleteCount(),
                            dataImport.GetEntitiesDebugString()
                        );
                //dataImport.Pesrist(ZoneId, UserName);
            }
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