using System;
using System.Text.RegularExpressions;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.Administration.Apps
{
    public partial class AppExport : SexyControlAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportApp_OnClick(object sender, EventArgs e)
        {
            using (var stream = new ZipExport(ZoneId.Value, AppId.Value).ExportApp())
            {
                Response.Clear();
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment;filename=2sxcApp_" + Regex.Replace(Sexy.App.Name, "[^a-zA-Z0-9-_]", "") + "_" + Sexy.App.Configuration.Version + ".zip");
                Response.Flush();

                stream.WriteTo(Response.OutputStream);

                Response.Flush();
                Response.End();
            }
        }
    }
}