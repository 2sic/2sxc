using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    public partial class Import : SexyControlAdminBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkExport.NavigateUrl =  EditUrl(TabId, SexyContent.ControlKeys.Export, true, "mid=" + ModuleId + "&" + SexyContent.AppIDString + "=" + AppId);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                ImportFromStream(fileUpload.FileContent, fileUpload.FileName.EndsWith(".zip"));
            }
        }

        protected void ImportFromStream(Stream importStream, bool isZip)
        {
            var messages = new List<ExportImportMessage>();
            var success = false;

            if (isZip)
            {
                success = new ZipImport(ZoneId.Value, AppId.Value, UserInfo.IsSuperUser).ImportZip(importStream, Server, PortalSettings, messages);
            }
            else
            {
                var xml = new StreamReader(importStream).ReadToEnd();
	            var doc = XDocument.Parse(xml);
                var import = new XmlImport(PortalSettings.Current.DefaultLanguage, PortalSettings.Current.UserInfo.Username);
				success = import.ImportXml(ZoneId.Value, AppId.Value, doc);
                messages = import.ImportLog;
            }

            lstvSummary.DataSource = messages;
            lstvSummary.DataBind();
            pnlSummary.Visible = true;
            pnlUpload.Visible = false;
        }

        protected void lstvSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var MessageType = ((ExportImportMessage)e.Item.DataItem).MessageType;
                var Pnl = (Panel)e.Item.FindControl("pnlMessage");

                if (MessageType == ExportImportMessage.MessageTypes.Error)
                    Pnl.CssClass += " dnnFormValidationSummary";
                if (MessageType == ExportImportMessage.MessageTypes.Information)
                    Pnl.CssClass += " dnnFormSuccess";
                if (MessageType == ExportImportMessage.MessageTypes.Warning)
                    Pnl.CssClass += " dnnFormWarning";
            }
        }

        // 2016-02-24 2dm - seems unused
        //protected void btnInstallGettingStarted_Click(object sender, EventArgs e)
        //{
        //    var messages = new List<ExportImportMessage>();
        //    new GettingStartedImport(ZoneId.Value, AppId.Value).ImportGettingStartedTemplates(UserInfo, messages);
        //}
    }
}