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

        public string FileLocation
        {
            get
            {
                if (ViewState["FileLocation"] == null)
                {
                    return null;
                }
                return ViewState["FileLocation"] as string;
            }
            set
            {
                ViewState["FileLocation"] = value;
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
            get { return PortalSettings.DefaultLanguage; }
        }

        public EntityClearImportOption EntityClearOptionSelected
        {
            get { return ParseEnum<EntityClearImportOption>(rblEntityClear.SelectedValue); }
        }

        public ResourceReferenceImportOption ResourceReferenceOptionSelected
        {
            get { return ParseEnum<ResourceReferenceImportOption>(rblResourceReference.SelectedValue); }
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

            rblEntityClear.DataSource = EnumToDataSource<EntityClearImportOption>();
            rblEntityClear.DataBind();
            rblEntityClear.SelectedValue = EntityClearImportOption.None.ToString();

            rblResourceReference.DataSource = EnumToDataSource<ResourceReferenceImportOption>();
            rblResourceReference.DataBind();
            rblResourceReference.SelectedValue = ResourceReferenceImportOption.Keep.ToString();
        }


        protected void OnTestDataClick(object sender, EventArgs e)
        {
            if (!fuFileUpload.HasFile)
            {
                // TODO2tk: Sow an error message
                return;
            }

            var dataFileName = fuFileUpload.FileName;
            var dataStream = fuFileUpload.FileContent;
            var dataImport = DataXmlImport.Deserialize(dataStream, ApplicationId, ContentTypeIdSelected, Languages, LanguageFallback, EntityClearOptionSelected, ResourceReferenceOptionSelected);
            if (dataImport.HasErrors)
            {
                ShowErrorPanel(dataFileName, dataImport);
            }
            else
            {
                ShowDetailPanel(dataFileName, dataImport);
            }
        }

        protected void OnImportDataClick(object sender, EventArgs e)
        {
            // TODO2tk: Get the file name from the view state
            // TODO2tk: Persist the import data
            ShowDonePanel();
        }

        protected void OnBackClick(object sender, EventArgs e)
        {
            ShowSetupPanel();
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


        private void ShowSetupPanel()
        {
            pnlSetup.Visible = true;
            pnlDetail.Visible = false;
            pnlError.Visible = false;
            pnlDone.Visible = false;
        }

        private void ShowDetailPanel(string dataFileName, DataXmlImport dataImport)
        {
            lblDetailInfo.Text = LocalizeFormatString("lblDetailInfo", dataFileName);
            lblDetailElementCount.Text = LocalizeFormatString
            (
                "lblDetailElementCount", dataImport.GetDocumentElementCount()
            );
            lblDetailLanguageCount.Text = LocalizeFormatString
            (
                "lblDetailLanguageCount", dataImport.GetLanguageCount()
            );
            lblDetailAttributes.Text = LocalizeFormatString
            (
                "lblDetailAttributes", dataImport.GetAttributeCount(), dataImport.GetAttributeNames(", ")
            );
            lblDetailEntitiesCreate.Text = LocalizeFormatString
            (
                "lblDetailEntitiesCreate", dataImport.GetEntitiesCreateCount()
            );
            lblDetailEntitiesUpdate.Text = LocalizeFormatString
            (
                "lblDetailEntitiesUpdate", dataImport.GetEntitiesUpdateCount()
            );
            lblDetailDetailsDelete.Text = LocalizeFormatString
            (
                "lblDetailDetailsDelete", dataImport.GetEntitiesDeleteCount()
            );
            lblDetailAttributeIgnore.Text = LocalizeFormatString
            (
                "lblDetailAttributeIgnore", dataImport.GetAttributeIgnoreCount(), dataImport.GetAttributeIgnoredNames(", ")
            );

            pnlSetup.Visible = false;
            pnlDetail.Visible = true;
            pnlError.Visible = false;
            pnlDone.Visible = false;
        }

        private void ShowErrorPanel(string dataFileName, DataXmlImport dataImport)
        {
            lblErrorInfo.Text = LocalizeFormatString("lblErrorInfo", dataFileName);

            var errorProtocolHtml = string.Empty;
            foreach(var error in dataImport.ErrorProtocol.Errors)
            {
                errorProtocolHtml += string.Format("<li>{0} ({1})</li>", error.ErrorCode, error.ErrorDetail);
            }
            ulErrorProtocol.InnerHtml = errorProtocolHtml;

            pnlSetup.Visible = false;
            pnlDetail.Visible = false;
            pnlError.Visible = true;
            pnlDone.Visible = false;
        }

        private void ShowDonePanel()
        {
            pnlSetup.Visible = false;
            pnlDetail.Visible = false;
            pnlError.Visible = false;
            pnlDone.Visible = true;
        }

        private string LocalizeFormatString(string formatStringKey, params object[] values)
        {
            return string.Format(LocalizeString(formatStringKey), values);
        }
    }
}