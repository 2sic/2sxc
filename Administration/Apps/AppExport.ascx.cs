using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.Administration.Apps
{
    public partial class AppExport : SexyControlAdminBase
    {

        protected ZipExport Exporter;

        protected void Page_Load(object sender, EventArgs e)
        {
            Exporter = new ZipExport(ZoneId.Value, AppId.Value);
            Exporter.FileManager.AllFiles.Count();
        }

        protected void btnExportApp_OnClick(object sender, EventArgs e)
        {
            using (var stream = Exporter.ExportApp(chkIncludeContentGroups.Checked, chkResetApGuid.Checked))
            {
                Response.Clear();
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment;filename=2sxcApp_" + Regex.Replace(SxcContext.App.Name, "[^a-zA-Z0-9-_]", "") + "_" + (SxcContext.App.Configuration == null ? "" : SxcContext.App.Configuration.Version) + ".zip");
                Response.Flush();

                stream.WriteTo(Response.OutputStream);

                if (Response.IsClientConnected)
                {
                    Response.Flush();
                }
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}