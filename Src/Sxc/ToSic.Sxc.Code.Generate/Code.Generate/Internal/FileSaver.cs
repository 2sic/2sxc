using System;
using System.IO;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static ToSic.Sxc.Internal.SxcLogging;

namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// Service to take generated files and save them to the file system
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FileSaver(CSharpDataModelsGenerator generator, ISite site, IAppStates appStates, IAppPathsMicroSvc appPaths)
    : ServiceBase(SxcLogName + ".GenFSv")
{
    public IAppState AppState => _appState ??= new Func<IAppState>(() => appStates.ToReader(appStates.GetCacheState(_specs.AppId)))();
    private IAppState _appState;

    public void Setup(IFileGeneratorSpecs parameters)
    {
        _specs = parameters;
    }

    private IFileGeneratorSpecs _specs;


    public void GenerateAndSaveFiles(IFileGeneratorSpecs parameters)
    {
        var l = Log.Fn();
        Setup(parameters);

        generator.Setup(parameters);
        var bundle = generator.Generate().First();
        var physicalPath = GetAppCodeDataPhysicalPath(bundle.Path);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'");

        var classFiles = bundle.Files;
        foreach (var classSb in classFiles)
        {
            l.A($"Writing {classSb.FileName}; Path: {classSb.Path}; Content: {classSb.Body.Length}");

            var addPath = classSb.Path ?? "";
            if (addPath.StartsWith("/") || addPath.StartsWith("\\") || addPath.EndsWith("/") || addPath.EndsWith("\\") || addPath.Contains(".."))
                throw new($"Invalid path '{addPath}' in class '{classSb.FileName}' - contains invalid path like '..' or starts/ends with a slash.");

            var basePath = Path.Combine(physicalPath, classSb.Path);

            // ensure the folder for the file exists - it could be different for each file
            Directory.CreateDirectory(basePath);

            var fullPath = Path.Combine(basePath, classSb.FileName);
            File.WriteAllText(fullPath, classSb.Body);
        }

        l.Done();
    }

    private string GetAppFullPath() => appPaths.Init(site, AppState).PhysicalPath;

    private string GetAppCodeDataPhysicalPath(string mask)
    {
        // Do basic mask tests
        if (mask.IsEmpty()) throw new("Mask must not be empty");
        if (mask.ContainsPathTraversal()) throw new($"Mask {PathFixer.PathTraversalMayNotContainMessage}");
        if (!mask.StartsWith(GenerateConstants.AppRootFolderPlaceholder)) throw new($"Mask must start with '{GenerateConstants.AppRootFolderPlaceholder}'");


        // Get the full path to the app
        var path = mask.Replace(GenerateConstants.AppRootFolderPlaceholder, GetAppFullPath().TrimLastSlash());

        // Optionally add / replace the edition
        if (path.IndexOf(GenerateConstants.EditionPlaceholder, StringComparison.OrdinalIgnoreCase) > -1)
        {
            // sanitize path because 'edition' is user provided
            if (_specs.Edition.ContainsPathTraversal())
                throw new($"Invalid edition '{_specs.Edition}' - {PathFixer.PathTraversalMayNotContainMessage}");

            path = path.Replace(GenerateConstants.EditionPlaceholder, _specs.Edition).TrimLastSlash();
        }

        path = path.FlattenSlashes().Backslash();

        var appWithEditionNormalized = new DirectoryInfo(path).FullName;

        if (!Directory.Exists(appWithEditionNormalized))
            throw new DirectoryNotFoundException(appWithEditionNormalized);

        return appWithEditionNormalized;
    }

    // TODO: @STV - this should be moved to an AppJsonService in Eav.Apps
    internal string GetPathToDotAppJson(FileGeneratorSpecs parameters)
    {
        Setup(parameters);
        return Path.Combine(GetAppFullPath(), Constants.AppDataProtectedFolder, Constants.AppJson);
    }


    //public string Dump()
    //{
    //    var sb = new StringBuilder();

    //    var classFiles = Generate().First().Files;
    //    foreach (var classSb in classFiles)
    //    {
    //        sb.AppendLine($"// ----------------------- file: {classSb.FileName} ----------------------- ");
    //        sb.AppendLine(classSb.Body);
    //        sb.AppendLine();
    //        sb.AppendLine();
    //    }

    //    return sb.ToString();
    //}

}