using System.IO;
using ToSic.Eav.ImportExport.Internal;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{

    private void FullDirList(DirectoryInfo dir, string searchPattern, List<DirectoryInfo> folders, List<FileInfo> files, SearchOption opt)
    {
        // list the files
        try
        {
            foreach (var f in dir.GetFiles(searchPattern))
                try
                {
                    files.Add(f);
                }
                catch
                {
                    // ignored
                }
        }
        catch
        {
            // We already got an error trying to access dir so dont try to access it again
            return;
        }

        // process each directory
        // If I have been able to see the files in the directory I should also be able 
        // to look at its directories so I dont think I should place this in a try catch block
        if (opt != SearchOption.AllDirectories) return;

        foreach (var d in dir.GetDirectories())
        {
            try
            {
                // todo: possibly re-include subfolders with ".data"
                if (Settings.ExcludeFolders.Contains(d.Name)) continue;
                folders.Add(d);
                FullDirList(d, searchPattern, folders, files, opt);
            }
            catch
            {
                // ignored
            }
        }
    }
}