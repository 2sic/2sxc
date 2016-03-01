using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using ToSic.SexyContent.Administration;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    public partial class AppImport : SexyControlAdminBaseWillSoonBeRemoved
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                ImportFromStream(fileUpload.FileContent);
            }
        }

        protected void ImportFromStream(Stream importStream)
        {
            var messages = new List<ExportImportMessage>();
            var success = false;

            success = new ZipImport(ZoneId.Value, null, UserInfo.IsSuperUser).ImportApp(importStream, Server, PortalSettings, messages);

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

    }
}