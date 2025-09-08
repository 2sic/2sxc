using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.Sys;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    private readonly string _appCodeFolder = $"{Path.DirectorySeparatorChar}{FolderConstants.AppCodeFolder}{Path.DirectorySeparatorChar}".ToLower();

    private (List<DirectoryInfo> Folders, List<FileInfo> Files) FullDirList(DirectoryInfo dir, string searchPattern, bool withSubfolders, int level = 0)
    {
        var l = Log.Fn<(List<DirectoryInfo> Folders, List<FileInfo> Files)>($"'{dir.FullName}', '{searchPattern}', {nameof(withSubfolders)}: {withSubfolders}, level:{level}");

        // detect special case when searching for api controller files
        var isApiControllerSearch = searchPattern.Equals($"*{EavConstants.ApiControllerSuffix}.cs", StringComparison.OrdinalIgnoreCase);

        List<DirectoryInfo> folders = [];
        List<FileInfo> files = [];

        // list the files
        try
        {
            if (!isApiControllerSearch
                || (EavConstants.Api.Equals(dir.Name, StringComparison.OrdinalIgnoreCase) // controller files in "api" folder
                    || FolderConstants.AppCodeFolder.Equals(dir.Name, StringComparison.OrdinalIgnoreCase) // controller files directly in "AppCode" folder
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
            // We already got an error trying to access dir so don't try to access it again
            return l.Return((folders, files), $"files:{files.Count}, folders:{folders.Count}");
        }

        // process each directory
        // If I have been able to see the files in the directory I should also be able 
        // to look at its directories, so I don't think I should place this in a try catch block
        if (!withSubfolders)
            return l.Return((folders, files), $"files:{files.Count}, folders:{folders.Count}");

        foreach (var d in dir.GetDirectories())
        {
            try
            {
                // todo: possibly re-include subfolders with ".data"
                if (Settings.ExcludeFolders.Contains(d.Name))
                    continue;
                
                if (isApiControllerSearch)
                {
                    //  we need to skip "AppCode" because it is handled differently in AllApiControllerFilesInAppCodeForAllEditions
                    if (FolderConstants.AppCodeFolder.Equals(d.Name, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // we skip any folder that is deeper more than 2 levels (except "AppCode" and its subfolders),
                    // because "api" folder can't be deeper than 2 levels
                    // 2025-09-02 2dm - changed this, because extensions in the 'AppCode/System' folder will have a deeper structure
                    if (level > 1 && !d.FullName.Contains($"{FolderConstants.AppCodeFolder}{Path.DirectorySeparatorChar}System", StringComparison.InvariantCultureIgnoreCase))
                        continue; // level is zero based
                }

                folders.Add(d);
                var (newFolders, newFiles) = FullDirList(d, searchPattern, withSubfolders, level + 1);
                folders.AddRange(newFolders);
                files.AddRange(newFiles);
            }
            catch
            {
                // ignored
            }
        }
        return l.Return((folders, files), $"files:{files.Count}, folders:{folders.Count}");
    }
}