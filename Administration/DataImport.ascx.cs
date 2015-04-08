using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.SexyContent.DataImportExport;
using ToSic.SexyContent.DataImportExport.Extensions;
using ToSic.SexyContent.DataImportExport.Options;

namespace ToSic.SexyContent.Administration
{
    public partial class DataImport : SexyControlAdminBase
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

        public string UserName
        {
            get { return PortalSettings.UserInfo.DisplayName; }
        }

        public string FileTemporaryDirectory
        {
            get
            {
                if (ViewState["FileTemporaryDirectory"] == null)
                {
                    return null;
                }
                return ViewState["FileTemporaryDirectory"] as string;
            }
            set
            {
                ViewState["FileTemporaryDirectory"] = value;
            }
        }

        public string FileName
        {
            get
            {
                if (ViewState["FileName"] == null)
                {
                    return null;
                }
                return ViewState["FileName"] as string;
            }
            set
            {
                ViewState["FileName"] = value;
            }
        }

        public string FilePath
        {
            get { return Path.Combine(FileTemporaryDirectory, FileName); }
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

        public EntityClearImport EntityClearOptionSelected
        {
            get { return ParseEnum<EntityClearImport>(rblEntityClear.SelectedValue); }
        }

        public ResourceReferenceImport ResourceReferenceOptionSelected
        {
            get { return ParseEnum<ResourceReferenceImport>(rblResourceReference.SelectedValue); }
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

            ApplicationId = Request["AppID"] != null ? int.Parse(Request["AppID"]) : 0;
            var sexyContent = new SexyContent(ZoneId.Value, ApplicationId);

            Languages = sexyContent.ContentContext.GetLanguages()
                                                  .Where(language => language.Active)
                                                  .Select(language => language.ExternalKey)
                                                  .OrderBy(language => language != LanguageFallback)
                                                  .ThenBy(language => language)
                                                  .ToList();
            FileTemporaryDirectory = CreatePhysicalDirectory(SexyContent.TemporaryDirectory);

            ddlContentType.DataSource = sexyContent.GetAvailableContentTypes(SexyContent.AttributeSetScope);
            ddlContentType.DataBind();

            rblEntityClear.DataSource = EnumToDataSource<EntityClearImport>();
            rblEntityClear.DataBind();
            rblEntityClear.SelectedValue = EntityClearImport.None.ToString();

            rblResourceReference.DataSource = EnumToDataSource<ResourceReferenceImport>();
            rblResourceReference.DataBind();
            rblResourceReference.SelectedValue = ResourceReferenceImport.Resolve.ToString();
        }


        protected void OnTestDataDetailedClick(object sender, EventArgs e)
        {
            OnTestDataClick(true);
        }

        protected void OnTestDataClick(object sender, EventArgs e)
        {
            OnTestDataClick(false);
        }

        private void OnTestDataClick(bool detailed)
        {
            if (!fuFileUpload.HasFile)
            {
                lblFileUploadError.Visible = true;
                return;
            }

            FileName = fuFileUpload.FileName;

            var fileContent = fuFileUpload.FileContent;
            var fileImport = new XmlImport(ZoneId.Value, ApplicationId, ContentTypeIdSelected, fileContent, Languages, LanguageFallback, EntityClearOptionSelected, ResourceReferenceOptionSelected);
            if (fileImport.HasErrors)
            {
                ShowErrorPanel(fileImport);
            }
            else
            {
                var fileInfo = new FileInfo(FilePath);
                fileInfo.WriteStream(fileContent);

                ShowDetailPanel(fileImport, detailed);
            }
        }

        protected void OnImportDataClick(object sender, EventArgs e)
        {
            var fileInfo = new FileInfo(FilePath);
            var fileContent = fileInfo.ReadStream();
            fileInfo.Delete();  
          
            var fileImport = new XmlImport(ZoneId.Value, ApplicationId, ContentTypeIdSelected, fileContent, Languages, LanguageFallback, EntityClearOptionSelected, ResourceReferenceOptionSelected);
            var fileImported = fileImport.PersistImportToRepository(UserName);
           
            ShowDonePanel(!fileImported);
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

        private string CreatePhysicalDirectory(string serverDirectory)
        {
            var physicalDirectory = Server.MapPath(serverDirectory).TrimEnd('\\') + "\\";
            if (!Directory.Exists(physicalDirectory))
            {
                Directory.CreateDirectory(physicalDirectory);
            }
            return physicalDirectory;
        }


        private void ShowSetupPanel()
        {
            pnlSetup.Visible = true;
            pnlDetail.Visible = false;
            pnlError.Visible = false;
            pnlDone.Visible = false;
        }

        private void ShowDetailPanel(XmlImport dataImport, bool showDebugOutput)
        {
            lblDetailInfo.Text = LocalizeFormatString("lblDetailInfo", FileName);
            lblDetailElementCount.Text = LocalizeFormatString
            (
                "lblDetailElementCount", dataImport.DocumentElements.Count()
            );
            lblDetailLanguageCount.Text = LocalizeFormatString
            (
                "lblDetailLanguageCount", dataImport.LanguagesInDocument.Count()
            );
            lblDetailAttributes.Text = LocalizeFormatString
            (
                "lblDetailAttributes", dataImport.AttributeNamesInDocument.Count(), string.Join(", ", dataImport.AttributeNamesInDocument)
            );
            lblDetailEntitiesCreate.Text = LocalizeFormatString
            (
                "lblDetailEntitiesCreate", dataImport.AmountOfEntitiesCreated
            );
            lblDetailEntitiesUpdate.Text = LocalizeFormatString
            (
                "lblDetailEntitiesUpdate", dataImport.AmountOfEntitiesUpdated
            );
            lblDetailDetailsDelete.Text = LocalizeFormatString
            (
                "lblDetailDetailsDelete", dataImport.AmountOfEntitiesDeleted
            );
            lblDetailAttributeIgnore.Text = LocalizeFormatString
            (
                "lblDetailAttributeIgnore", dataImport.AttributeNamesNotImported.Count(), string.Join(", ", dataImport.AttributeNamesNotImported)
            );

            if (showDebugOutput)
            {
                lblDetailDebugOutput.Text = dataImport.GetDebugReport();
            }

            pnlSetup.Visible = false;
            pnlDetail.Visible = true;
            pnlError.Visible = false;
            pnlDone.Visible = false;
        }

        private void ShowErrorPanel(XmlImport dataImport)
        {
            lblErrorInfo.Text = LocalizeFormatString("lblErrorInfo", FileName);

            var errorProtocolHtml = string.Empty;
            foreach(var error in dataImport.ErrorProtocol.Errors)
            {
                errorProtocolHtml += string.Format
                    (
                        "<li>{0}{1}{2}{3}</li>", 
                        error.ErrorCode.GetDescription(), 
                        error.ErrorDetail != null ? LocalizeFormatString("lblErrorProtocolErrorDetail", error.ErrorDetail) : "",
                        error.LineNumber != null ? LocalizeFormatString("lblErrorProtocolLineNo", error.LineNumber) : "",
                        error.LineDetail != null ? LocalizeFormatString("lblErrorProtocolLineDetail", error.LineDetail) : ""
                    );
            }
            ulErrorProtocol.InnerHtml = errorProtocolHtml;

            pnlSetup.Visible = false;
            pnlDetail.Visible = false;
            pnlError.Visible = true;
            pnlDone.Visible = false;
        }

        private void ShowDonePanel(bool importFailed = false, string importFaileReason = null)
        {
            lblDoneInfo.Text = LocalizeFormatString("lblDoneInfo", FileName);
            if (importFailed)
            {
                lblDoneResult.Text = LocalizeFormatString("lblDoneResultFailed", importFaileReason);
            }
            else
            {
                lblDoneResult.Text = LocalizeString("lblDoneResultSucceeded");
            }

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