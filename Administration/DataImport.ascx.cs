using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.SexyContent.DataImportExport;
using ToSic.SexyContent.DataImportExport.Extensions;

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

            var sexyContent = new SexyContent(1, ApplicationId.Value); // TODO2tk: Get the zone ID

            ZoneId = SexyContent.GetZoneID(PortalId);
            Languages = sexyContent.ContentContext.GetLanguages()
                                                  .Where(language => language.Active)
                                                  .Select(language => language.ExternalKey)
                                                  .OrderBy(language => language != LanguageFallback)
                                                  .ThenBy(language => language)
                                                  .ToList();
            FileTemporaryDirectory = CreatePhysicalDirectory(SexyContent.TemporaryDirectory);

            ddlContentType.DataSource = sexyContent.GetAvailableAttributeSets(); ;
            ddlContentType.DataBind();

            rblEntityClear.DataSource = EnumToDataSource<EntityClearImportOption>();
            rblEntityClear.DataBind();
            rblEntityClear.SelectedValue = EntityClearImportOption.None.ToString();

            rblResourceReference.DataSource = EnumToDataSource<ResourceReferenceImportOption>();
            rblResourceReference.DataBind();
            rblResourceReference.SelectedValue = ResourceReferenceImportOption.Resolve.ToString();
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
            var fileImport = new DataXmlImport(ApplicationId, ContentTypeIdSelected, Languages, LanguageFallback, EntityClearOptionSelected, ResourceReferenceOptionSelected);
            fileImport.Deserialize(fileContent);
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
          
            var fileImport = new DataXmlImport(ApplicationId, ContentTypeIdSelected, Languages, LanguageFallback, EntityClearOptionSelected, ResourceReferenceOptionSelected);
            fileImport.Deserialize(fileContent);
            var fileImported = fileImport.Pesrist(ZoneId, UserName);
           
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

        private void ShowDetailPanel(DataXmlImport dataImport, bool showDebugOutput)
        {
            lblDetailInfo.Text = LocalizeFormatString("lblDetailInfo", FileName);
            lblDetailElementCount.Text = LocalizeFormatString
            (
                "lblDetailElementCount", dataImport.GetDocumentElementCount()
            );
            lblDetailLanguageCount.Text = LocalizeFormatString
            (
                "lblDetailLanguageCount", dataImport.GetDocumentElementLanguageCount()
            );
            lblDetailAttributes.Text = LocalizeFormatString
            (
                "lblDetailAttributes", dataImport.GetDocumentElementAttributeCount(), dataImport.GetDocumentElementAttributeNames(", ")
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

            if (showDebugOutput)
            {
                lblDetailDebugOutput.Text = dataImport.GetEntitiesDebugString();
            }

            pnlSetup.Visible = false;
            pnlDetail.Visible = true;
            pnlError.Visible = false;
            pnlDone.Visible = false;
        }

        private void ShowErrorPanel(DataXmlImport dataImport)
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