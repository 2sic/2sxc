using System.IO;
using ToSic.Eav.WebApi.Assets;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    public List<string> All(int appId, bool global = false, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
    {
        Log.A($"list a#{appId}, global:{global}, path:{path}, mask:{mask}, withSub:{withSubfolders}, withFld:{returnFolders}");
        // set global access security if ok...
        if (global && !_user.IsSystemAdmin)
            throw new NotSupportedException("only host user may access global files");

        // make sure the folder-param is not null if it's missing
        if (string.IsNullOrEmpty(path)) path = "";
        var appPath = ResolveAppPath(appId, global);
        var fullPath = Path.Combine(appPath, path);

        // make sure the resulting path is still inside 2sxc
        if (!_user.IsSystemAdmin  && !fullPath.Contains("2sxc"))
            throw new DirectoryNotFoundException("the folder is not inside 2sxc-scope any more and the current user doesn't have the permissions - must cancel");

        // if the directory doesn't exist, return empty list
        if (!Directory.Exists(fullPath))
            return new();

        var opt = withSubfolders
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        // try to collect all files, ignoring long paths errors and similar etc.
        var files = new List<FileInfo>();           // List that will hold the files and sub-files in path
        var folders = new List<DirectoryInfo>();    // List that hold directories that cannot be accessed
        var di = new DirectoryInfo(fullPath);
        FullDirList(di, mask, folders, files, opt);

        // return folders or files (depending on setting) with/without subfolders
        return (returnFolders
                ? folders.Select(f => f.FullName)
                : files.Select(f => f.FullName)
            )
            .Select(p => EnsurePathMayBeAccessed(p, appPath, _user.IsSystemAdmin))  // do another security check
            .Select(x => x.Replace(appPath + "\\", ""))           // truncate / remove internal server root path
            .Select(x =>
                x.Replace("\\", "/")) // tip the slashes to web-convention (old template entries used "\")
            .ToList();
    }

    public AllFilesDto AppFiles(int appId, string path, string mask)
    {
        mask = mask ?? "*.*";
        var localFiles =
            All(appId, global: false, path: path, mask: mask, withSubfolders: true, returnFolders: false)
                .Select(f => new AllFileDto { Path = f });
        var globalFiles = _user.IsSystemAdmin
            ? All(appId, global: true, path: path, mask: mask, withSubfolders: true, returnFolders: false)
                .Select(f => new AllFileDto { Path = f, Shared = true }).ToArray()
            : Array.Empty<AllFileDto>();
        return new() { Files = localFiles.Union(globalFiles),};
    }
}