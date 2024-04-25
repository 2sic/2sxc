using System.IO;
using System.Reflection;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.Assets;
using ToSic.Sxc.Code.Internal.HotBuild;

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
                .Select(f => new AllFileDto { Path = f, Folder = f, EndpointPath = ApiFileEndpointPath(f) }).ToArray();
        l.A($"local files:{localFiles.Length}");

        var globalFiles = _user.IsSystemAdmin
            ? All(appId, global: true, path: path, mask: mask, withSubfolders: true, returnFolders: false)
                .Select(f => new AllFileDto { Path = f, Shared = true, Folder = f, EndpointPath = ApiFileEndpointPath(f) }).ToArray()
            : [];
        l.A($"global files:{globalFiles.Length}");

        // only for api controller files
        var allInAppCode = AllApiControllerFilesInAppCodeForAllEditions(appId, mask: mask).ToArray();
        l.A($"all in AppCode:{allInAppCode.Length}");

        return l.ReturnAsOk( new() { Files = localFiles.Union(globalFiles).Union(allInAppCode) });
    }

    private List<AllFileDto> AllApiControllerFilesInAppCodeForAllEditions(int appId, bool global = false, string mask = "*.*")
    {
        var l = Log.Fn<List<AllFileDto>>($"list all in AppCode a#{appId}, {nameof(global)}:{global}, {nameof(mask)}:{mask}");

        if (!mask.Equals($"*{Constants.ApiControllerSuffix}.cs", StringComparison.OrdinalIgnoreCase))
            return l.Return([], $"nothing to do, mask:'{mask}' is not '*{Constants.ApiControllerSuffix}.cs', so we are not looking for controller files");

        // set global access security if ok...
        if (global && !_user.IsSystemAdmin)
            throw l.Ex(new NotSupportedException("only host user may access global files"));

        var appPath = ResolveAppPath(appId, global);
        var app = _appStates.GetReader(appId);
        var editions = _codeController.Value.GetEditions(appId);
        l.A($"{nameof(app.Folder)}:'{app.Folder}', appPath:'{appPath}', editions:{editions.Editions.Count}");
        
        List<AllFileDto> appCodeApiControllerFiles = [];
        foreach (var editionDto in editions.Editions)
        {
            var edition = editionDto.Name;
            l.A($"collect ApiController files in AppCode for edition:'{edition}'");

            // get AppCode assembly
            var spec = new HotBuildSpec(appId, edition: edition, app.Folder);
            l.A($"{spec}");
            var (result, _) = _appCodeLoader.Value.GetAppCode(spec);
            var appCodeAssembly = result?.Assembly;
            l.A($"has appCode assembly:{appCodeAssembly != null}");

            var codeApiControllerFiles = ApiControllerFilesInAppCode(mask, appPath, edition, appCodeAssembly);
            l.A($"ApiController files in AppCode for edition:'{edition}': {codeApiControllerFiles.Count}");

            appCodeApiControllerFiles.AddRange(
                codeApiControllerFiles
                .Select(f => new AllFileDto { Path = f, Folder = f, EndpointPath = AppCodeEndpointPath(edition, Path.GetFileNameWithoutExtension(f)), IsCompiled = true }));
        }

        return l.Return(appCodeApiControllerFiles, $"ok, count:{appCodeApiControllerFiles.Count}");
    }

    private List<string> ApiControllerFilesInAppCode(string mask, string appPath, string edition, Assembly appCodeAssembly)
    {
        var l = Log.Fn<List<string>>($"list ApiController files, {nameof(mask)}:'{mask}', {nameof(appPath)}:'{appPath}', {nameof(edition)}:'{edition}', has appCode assembly:{appCodeAssembly != null}");

        // 1. Check for AppCode assembly
        if (appCodeAssembly == null)
            return l.Return([], "nothing to do, AppCode assembly is missing");

        // 2. Check for AppCode directory
        var fullPath = Path.Combine(appPath, edition, Constants.AppCode);
        // if the edition directory doesn't exist, optional fallback to root edition AppCode
        if (!Directory.Exists(fullPath))
        {
            l.A($"AppCode folder do not exist on fullPath:'{fullPath}'");

            // if is root edition, then nothing to do
            if (string.IsNullOrEmpty(edition))
                return l.Return([], "nothing to do, root edition AppCode folder do not exits");

            // fallback to root edition AppCode
            fullPath = Path.Combine(appPath, Constants.AppCode);
            l.A($"fallback to root edition AppCode:'{fullPath}'");

            // if the root edition directory doesn't exist, then nothing to do
            if (!Directory.Exists(fullPath))
                return l.Return([], "nothing to do, fallback root edition AppCode folder do not exits");
        }

        // try to collect all files, ignoring long paths errors and similar etc.
        var files = new List<FileInfo>();           // List that will hold the files and sub-files in path
        var folders = new List<DirectoryInfo>();    // List that hold directories that cannot be accessed
        var di = new DirectoryInfo(fullPath);
        FullDirList(di, mask, folders, files, SearchOption.AllDirectories);
        l.A($"{nameof(FullDirList)}:'{fullPath}', files:{files.Count}, folders:{folders.Count}");

        // ApiController files with subfolders, when has its type in AppCode assembly
        var apiControllerFilesInAppCode = files.Where(f => OptionalCheckForControllerTypeInAppCodeAssembly(Path.GetFileNameWithoutExtension(f.Name), appCodeAssembly))
            .Select(f => f.FullName) 
            .Select(p => EnsurePathMayBeAccessed(p, appPath, _user.IsSystemAdmin))  // do another security check
            .Select(x => x.Replace(appPath + "\\", ""))           // truncate / remove internal server root path
            .Select(x => x.ForwardSlash()) // tip the slashes to web-convention (old template entries used "\")
            .ToList();

        return l.Return(apiControllerFilesInAppCode, $"ok, count:{apiControllerFilesInAppCode.Count}");
    }

    private bool OptionalCheckForControllerTypeInAppCodeAssembly(string controllerTypeName, Assembly appCodeAssembly)
    {
        var l = Log.Fn<bool>($"{nameof(OptionalCheckForControllerTypeInAppCodeAssembly)}({nameof(controllerTypeName)}:'{controllerTypeName}', has appCode assembly:{appCodeAssembly != null})");
        
        // check if it is controller type
        if (controllerTypeName.EndsWith(Constants.ApiControllerSuffix, StringComparison.OrdinalIgnoreCase)) 
            return l.ReturnTrue($"'{controllerTypeName}' is not controller type");

        return l.ReturnAndLog(appCodeAssembly.FindControllerTypeByName(controllerTypeName) != null);
    }

    private static string ApiFileEndpointPath(string relativePath)
        => AdjustControllerName(relativePath).ForwardSlash().PrefixSlash();

    private static string AppCodeEndpointPath(string edition, string controller)
        => Path.Combine(edition, Constants.Api, AdjustControllerName(controller)).ForwardSlash().PrefixSlash();

    private static string AdjustControllerName(string controllerName)
        => controllerName.EndsWith(Constants.ApiControllerSuffix, StringComparison.OrdinalIgnoreCase)
            ? controllerName.Substring(0, controllerName.Length - Constants.ApiControllerSuffix.Length)
            : controllerName;
}