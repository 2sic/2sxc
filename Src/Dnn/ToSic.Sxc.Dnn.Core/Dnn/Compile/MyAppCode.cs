using System.IO;
using System.Web;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.Compile
{
    /// <summary>
    /// DNN on start need to clean the "2sxc.bin", because it is used to temporary save shared temp MyAppCode assemblies.
    /// </summary>
    [PrivateApi]
    public static class MyAppCode
    {
        private static bool Cleaned { get; set; }
        private static readonly object CleaningLock = new();
        private static string TempAssemblyFolderPath { get; } = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "2sxc.bin");

        public static void CleanTempAssemblyFolder()
        {
            // Ensure that cleaning is executed only once
            if (Cleaned) return;

            lock (CleaningLock)
            {
                if (Cleaned) return;

                if (Directory.Exists(TempAssemblyFolderPath))
                {
                    var filesToClean = Directory.GetFiles(TempAssemblyFolderPath, "*.*", SearchOption.TopDirectoryOnly);
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
