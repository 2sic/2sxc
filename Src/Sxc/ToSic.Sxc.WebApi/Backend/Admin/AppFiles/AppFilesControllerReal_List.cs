using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.Assets;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    public List<string> All(int appId, bool global = false, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
    {
        var l = Log.Fn<List<string>>($"list a#{appId}, {nameof(global)}:{global}, {nameof(path)}:'{path}', {nameof(mask)}:'{mask}', withSub:{withSubfolders}, {nameof(returnFolders)}:{returnFolders}");

        // set global access security if ok...
        if (global && !_user.IsSystemAdmin)
            throw l.Ex(new NotSupportedException("only host user may access global files"));

        // make sure the folder-param is not null if it's missing
        if (string.IsNullOrEmpty(path)) path = "";
        var appPath = ResolveAppPath(appId, global);
        var fullPath = Path.Combine(appPath, path);
        l.A($"fullPath:'{fullPath}'");

        // make sure the resulting path is still inside 2sxc
        if (!_user.IsSystemAdmin && !fullPath.Contains("2sxc"))
            throw l.Ex(new DirectoryNotFoundException("the folder is not inside 2sxc-scope any more and the current user doesn't have the permissions - must cancel"));

        // if the directory doesn't exist, return empty list
        if (!Directory.Exists(fullPath))
            return l.Return([], "directory doesn't exist");

        var opt = withSubfolders
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        // try to collect all files, ignoring long paths errors and similar etc.
        var files = new List<FileInfo>();           // List that will hold the files and sub-files in path
        var folders = new List<DirectoryInfo>();    // List that hold directories that cannot be accessed
        var di = new DirectoryInfo(fullPath);
        FullDirList(di, mask, folders, files, opt);
        l.A($"{nameof(FullDirList)} files:{files.Count}, folders:{folders.Count}");

        // return folders or files (depending on setting) with/without subfolders
        var all = (returnFolders
            ? folders.Select(f => f.FullName)
            : files.Select(f => f.FullName)
            )
            .Select(p => EnsurePathMayBeAccessed(p, appPath, _user.IsSystemAdmin))  // do another security check
            .Select(x => x.Replace(appPath + "\\", ""))           // truncate / remove internal server root path
            .Select(x => x.ForwardSlash()) // tip the slashes to web-convention (old template entries used "\")
            .ToList();

        return l.Return(all, $"ok, count:{all.Count}");
    }

    public AllFilesDto AppFiles(int appId, string path, string mask)
    {
        mask = mask ?? "*.*";
        var l = Log.Fn<AllFilesDto>($"list all files a#{appId}, path:'{path}', mask:'{mask}'");

        var localFiles =
            All(appId, global: false, path: path, mask: mask, withSubfolders: true, returnFolders: false)
                .Select(f => new AllFileDto { Path = f }).ToArray();
        l.A($"local files:{localFiles.Length}");

        var globalFiles = _user.IsSystemAdmin
            ? All(appId, global: true, path: path, mask: mask, withSubfolders: true, returnFolders: false)
                .Select(f => new AllFileDto { Path = f, Shared = true }).ToArray()
            : [];
        l.A($"global files:{globalFiles.Length}");

        return l.ReturnAsOk(new() { Files = localFiles.Union(globalFiles) });
    }
}