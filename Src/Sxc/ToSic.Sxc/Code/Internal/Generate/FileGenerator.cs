using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FileGenerator(DataClassesGenerator generator, ISite site, IAppStates appStates, IAppPathsMicroSvc appPaths)
    : ServiceBase(SxcLogName + ".DMoGen")
{
    public const string AppRootFolderPlaceholder = "[app:root]";
    public const string EditionPlaceholder = "[target:edition]";


    public IAppState AppState => _appState ??= new Func<IAppState>(() => appStates.ToReader(appStates.GetCacheState(_parameters.AppId)))();
    private IAppState _appState;

    public void Setup(GenerateParameters parameters)
    {
        _parameters = parameters;
    }

    private GenerateParameters _parameters;


    public void GenerateAndSaveFiles(GenerateParameters parameters)
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
        if (!mask.StartsWith(AppRootFolderPlaceholder)) throw new($"Mask must start with '{AppRootFolderPlaceholder}'");


        // Get the full path to the app
        var path = mask.Replace(AppRootFolderPlaceholder, GetAppFullPath().TrimLastSlash());

        // Optionally add / replace the edition
        if (path.IndexOf(EditionPlaceholder, StringComparison.OrdinalIgnoreCase) > -1)
        {
            // sanitize path because 'edition' is user provided
            if (_parameters.Edition.ContainsPathTraversal())
                throw new($"Invalid edition '{_parameters.Edition}' - {PathFixer.PathTraversalMayNotContainMessage}");

            path = path.Replace(EditionPlaceholder, _parameters.Edition).TrimLastSlash();
        }

        path = path.FlattenSlashes().Backslash();

        var appWithEditionNormalized = new DirectoryInfo(path).FullName;
       
        if (!Directory.Exists(appWithEditionNormalized))
            throw new DirectoryNotFoundException(appWithEditionNormalized);

        return appWithEditionNormalized;
    }

    internal string GetPathToDotAppJson(GenerateParameters parameters)
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