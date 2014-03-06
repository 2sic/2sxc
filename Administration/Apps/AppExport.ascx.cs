using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent.Administration.Apps
{
    public partial class AppExport : SexyControlAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportApp_OnClick(object sender, EventArgs e)
        {
            var stream = new ToSic.SexyContent.ImportExport.ZipExport(ZoneId.Value, AppId.Value).ExportApp();
            Response.AddHeader("content-disposition", "attachment;filename=App.zip");
            Response.ContentType = "application/zip";

            stream.WriteTo(Response.OutputStream);
            stream.Close();
            Response.Flush();
            Response.Close();
        }
    }
}