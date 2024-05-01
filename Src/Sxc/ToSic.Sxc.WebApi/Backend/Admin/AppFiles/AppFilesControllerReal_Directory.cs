using System.IO;
using ToSic.Eav;
using ToSic.Eav.ImportExport.Internal;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    private readonly string _appCodeFolder = $"{Path.DirectorySeparatorChar}{Constants.AppCode}{Path.DirectorySeparatorChar}".ToLower();

    private void FullDirList(DirectoryInfo dir, string searchPattern, List<DirectoryInfo> folders, List<FileInfo> files, SearchOption opt, int level = 0)
    {
        // list the files
        try
        {
            if (!IsApiControllerFilesSearch(searchPattern)
                || (Constants.Api.Equals(dir.Name, StringComparison.OrdinalIgnoreCase) // controller files in "api" folder
                    || Constants.AppCode.Equals(dir.Name, StringComparison.OrdinalIgnoreCase) // controller files directly in "AppCode" folder
                    || dir.FullName.ToLower().Contains(_appCodeFolder) // controller files in any "AppCode" subfolder
                    )
                )
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
                
                if (IsApiControllerFilesSearch(searchPattern))
                {
                    //  we need to skip "AppCode" because it is handled differently in AllApiControllerFilesInAppCodeForAllEditions
                    if (Constants.AppCode.Equals(d.Name, StringComparison.OrdinalIgnoreCase)) continue;

                    // we skip any folder that is deeper more than 2 levels (except "AppCode" and its subfolders),
                    // because "api" folder can't be deeper than 2 levels
                    if (level > 1) continue; // level is zero based
                }

                folders.Add(d);
                FullDirList(d, searchPattern, folders, files, opt, level + 1);
            }
            catch
            {
                // ignored
            }
        }
    }

    // detect special case when searching for api controller files
    private static bool IsApiControllerFilesSearch(string searchPattern) 
        => searchPattern.Equals($"*{Constants.ApiControllerSuffix}.cs", StringComparison.OrdinalIgnoreCase);
}