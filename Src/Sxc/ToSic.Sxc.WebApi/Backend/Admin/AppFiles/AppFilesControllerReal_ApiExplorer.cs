using System.IO;
using System.Reflection;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal : Eav.WebApi.Admin.IAppExplorerControllerDependency
{

    /// <summary>
    /// Get all api controller files from AppCode for all editions
    /// </summary>
    /// <param name="appId"></param>
    /// <returns>used by ApiExplorerControllerReal.AppApiFiles</returns>
    [PrivateApi]
    public List<AllApiFileDto> AllApiFilesInAppCodeForAllEditions(int appId)
    {
        var l = Log.Fn<List<AllApiFileDto>>($"list all in AppCode a#{appId}");

        const string mask = $"*{Constants.ApiControllerSuffix}.cs";

        var appPath = ResolveAppPath(appId, global: false);
        var app = _appReaders.Get(appId).Specs;
        var editions = _codeController.Value.GetEditions(appId);
        l.A($"{nameof(app.Folder)}:'{app.Folder}', appPath:'{appPath}', editions:{editions.Editions.Count}");

        List<AllApiFileDto> appCodeApiControllerFiles = [];
        foreach (var editionDto in editions.Editions)
        {
            var edition = editionDto.Name;
            l.A($"collect ApiController files in AppCode for edition:'{edition}'");

            if (!Directory.Exists(Path.Combine(appPath, edition, Constants.AppCode)))
            {
                l.A($"edition:'{edition}' folder or '{Constants.AppCode}' subfolder do not exist in app");
                continue;
            }

            Assembly appCodeAssembly = null;
            try
            {
                // get AppCode assembly
                var spec = new HotBuildSpec(appId, edition: edition, app.Folder);
                l.A($"{spec}");
                var (result, _) = _appCodeLoader.Value.GetAppCode(spec);
                appCodeAssembly = result?.Assembly;
            }
            catch (Exception e)
            {
                l.Ex(e);
            }
            l.A($"has appCode assembly:{appCodeAssembly != null}");

            var codeApiControllerFiles = ApiControllerFilesInAppCode(mask, appPath, edition, appCodeAssembly);
            l.A($"ApiController files in AppCode for edition:'{edition}': {codeApiControllerFiles.Count}");

            appCodeApiControllerFiles.AddRange(
                codeApiControllerFiles
                    .Select(f => new AllApiFileDto
                    {
                        Path = f,
                        EndpointPath = ApiExplorerControllerReal.AppCodeEndpointPath(edition, Path.GetFileNameWithoutExtension(f)),
                        IsCompiled = true,
                        Edition = edition
                    }));
        }

        return l.Return(appCodeApiControllerFiles, $"ok, count:{appCodeApiControllerFiles.Count}");
    }

    private List<string> ApiControllerFilesInAppCode(string mask, string appPath, string edition,
        Assembly appCodeAssembly)
    {
        var l = Log.Fn<List<string>>(
            $"list ApiController files, {nameof(mask)}:'{mask}', {nameof(appPath)}:'{appPath}', {nameof(edition)}:'{edition}', has appCode assembly:{appCodeAssembly != null}");

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
        var files = new List<FileInfo>(); // List that will hold the files and sub-files in path
        var folders = new List<DirectoryInfo>(); // List that hold directories that cannot be accessed
        var di = new DirectoryInfo(fullPath);
        FullDirList(di, mask, folders, files, SearchOption.AllDirectories);
        l.A($"{nameof(FullDirList)}:'{fullPath}', files:{files.Count}, folders:{folders.Count}");

        // ApiController files with subfolders, when has its type in AppCode assembly
        var apiControllerFilesInAppCode = files.Where(f =>
                OptionalCheckForControllerTypeInAppCodeAssembly(Path.GetFileNameWithoutExtension(f.Name),
                    appCodeAssembly))
            .Select(f => f.FullName)
            .Select(p => EnsurePathMayBeAccessed(p, appPath, _user.IsSystemAdmin)) // do another security check
            .Select(x => x.Replace(appPath + "\\", "")) // truncate / remove internal server root path
            .Select(x => x.ForwardSlash()) // tip the slashes to web-convention (old template entries used "\")
            .ToList();

        return l.Return(apiControllerFilesInAppCode, $"ok, count:{apiControllerFilesInAppCode.Count}");
    }

    private bool OptionalCheckForControllerTypeInAppCodeAssembly(string controllerTypeName, Assembly appCodeAssembly)
    {
        var l = Log.Fn<bool>(
            $"{nameof(OptionalCheckForControllerTypeInAppCodeAssembly)}({nameof(controllerTypeName)}:'{controllerTypeName}', has appCode assembly:{appCodeAssembly != null})");

        // check if it is controller type
        if (controllerTypeName.EndsWith(Constants.ApiControllerSuffix, StringComparison.OrdinalIgnoreCase))
            return l.ReturnTrue($"'{controllerTypeName}' is not controller type");

        return l.ReturnAndLog(appCodeAssembly.FindControllerTypeByName(controllerTypeName) != null);
    }
}