using System.IO;
using System.Web;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// DNN on start need to clean the App_Data/appCode folder, because it is used to temporary save shared AppCode assemblies.
    /// </summary>
    public class CleanAppCode
    {
        public static bool Cleaned { get; private set; } = false;
        private static readonly object CleaningLock = new object();
        private static string AppCode { get; } = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), AppCodeLoader.AppCodeFolder);

        public static void CleanAppCodeDirectory()
        {
            // Ensure that cleaning is executed only once
            if (Cleaned) return;

            lock (CleaningLock)
            {
                if (Cleaned) return;

                if (Directory.Exists(AppCode))
                {
                    var filesToClean = Directory.GetFiles(AppCode, "*.*", SearchOption.TopDirectoryOnly);
                    foreach (var file in filesToClean)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                }
                Cleaned = true;
            }
        }
    }
}
