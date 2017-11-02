using System.Web;
using ToSic.Eav.Enums;
using ToSic.Eav.ImportExport.Persistence.File;

namespace ToSic.SexyContent.Environment
{
    public class DnnRuntime : Runtime
    {
        public DnnRuntime() : base("Dnn.Runtim")
        {
            Source = Repositories.SystemFiles;
        }

        // 1 - find the current path to the .data folder
        public override string Path =>
            HttpContext.Current.Server.MapPath(System.IO.Path.Combine(Eav.ImportExport.Settings.ModuleDirectory, ".data"));

    }
}