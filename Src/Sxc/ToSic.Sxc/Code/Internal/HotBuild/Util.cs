using System.IO;
using ToSic.Eav.Internal.Configuration;

namespace ToSic.Sxc.Code.Internal.HotBuild;

/// <summary>
/// On start need to clean the "2sxc.bin", because it is used to temporary save shared temp AppCode assemblies.
/// </summary>
public class Util(IGlobalConfiguration globalConfiguration)
{
    private static bool Cleaned { get; set; }
    private static readonly object CleaningLock = new();

    public void CleanTempAssemblyFolder()
    {
        // Ensure that cleaning is executed only once
        if (Cleaned) return;

        lock (CleaningLock)
        {
            if (Cleaned) return;

            if (Directory.Exists(globalConfiguration.TempAssemblyFolder))
            {
                var filesToClean = Directory.GetFiles(globalConfiguration.TempAssemblyFolder, "*.*", SearchOption.TopDirectoryOnly);
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
            else
            {
                // Ensure "2sxc.bin" folder exists to preserve dlls
                Directory.CreateDirectory(globalConfiguration.TempAssemblyFolder);
            }

            Cleaned = true;
        }
    }
}