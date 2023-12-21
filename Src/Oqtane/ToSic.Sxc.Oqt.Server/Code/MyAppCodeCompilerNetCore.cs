using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Oqt.Server.Code;

[PrivateApi]
internal class MyAppCodeCompilerNetCore: MyAppCodeCompiler
{
    public MyAppCodeCompilerNetCore(/*IServiceProvider serviceProvider,*/ LazySvc<IServerPaths> serverPaths) //: base(serviceProvider)
    {
        ConnectServices(
            _serverPaths = serverPaths
        );
    }

    private readonly LazySvc<IServerPaths> _serverPaths;

    protected internal override AssemblyResult GetAppCode(string virtualPath, int appId = 0)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {nameof(appId)}: {appId}", timer: true);

        try
        {
            var (sourceFiles, errResult) = GetSourceFilesOrError(NormalizeFullPath(_serverPaths.Value.FullContentPath(virtualPath.Backslash())));

            if (errResult != null)
                return l.ReturnAsError(errResult, errResult.ErrorMessages);


            //var initialPath = _serverPaths.Value.FullContentPath(virtualPath.Backslash());
            //// Get all C# files in the folder
            //var fullPath = NormalizeFullPath(initialPath);
            //var sourceFiles = Directory.GetFiles(fullPath, $"*{CsFiles}", UseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            //// Log all files
            //var wrapFiles = Log.Fn($"Source Files in {fullPath}:");
            //foreach (var sourceFile in sourceFiles) l.A(sourceFile);
            //wrapFiles.Done($"{sourceFiles.Length}");

            //// Validate are there any C# files
            //// TODO: if no files exist, it shouldn't be an error, because it could be that it's just not here yet
            //if (sourceFiles.Length == 0)
            //    return l.ReturnAsError(new AssemblyResult(
            //            errorMessages: $"Error: given path '{virtualPath}' doesn't contain any {CsFiles} files"),
            //        $"given path '{virtualPath}' doesn't contain any {CsFiles} files");

            var assemblyLocations = GetAssemblyLocations(appId);
            var dllName = Path.GetFileName(assemblyLocations[1]);
            var assemblyResult = new Compiler().GetCompiledAssemblyFromFolder(sourceFiles, assemblyLocations[1], assemblyLocations[0], dllName);

            // Compile ok
            if (assemblyResult.ErrorMessages.IsEmpty())
            {
                LogAllTypes(assemblyResult.Assembly);
                return l.ReturnAsOk(new AssemblyResult(assembly: assemblyResult.Assembly, assemblyLocations: assemblyLocations));
            }

            return l.ReturnAsError(new AssemblyResult(errorMessages: assemblyResult.ErrorMessages), assemblyResult.ErrorMessages);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{MyAppCodeDll}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage), "error");
        }
    }

    private string[] GetAssemblyLocations(int appId)
    {
        var l = Log.Fn<string[]>($"{nameof(appId)}: {appId}");
        var tempAssemblyFolderPath = _serverPaths.Value.FullContentPath(@"App_Data\2sxc.bin");
        l.A($"TempAssemblyFolderPath: '{tempAssemblyFolderPath}'");
        // Ensure "2sxc.bin" folder exists to preserve dlls
        Directory.CreateDirectory(tempAssemblyFolderPath);

        // need name 
        var assemblyName = GetAppCodeDllName(tempAssemblyFolderPath, appId);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = new[] { symbolsFilePath, assemblyFilePath };
        return l.ReturnAsOk(assemblyLocations);
    }

    //protected override (Type Type, string ErrorMessage) GetCsHtmlType(string virtualPath)
    //    => throw new("Runtime Compile of .cshtml is Not Implemented in .net standard / core");

    /// <summary>
    /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
    /// </summary>
    /// <returns>The generated random name.</returns>
    protected override string GetAppCodeDllName(string folderPath, int appId)
    {
        return Log.Fn<string>().Return($"App-{appId:0000}");
        //var l = Log.Fn<string>($"{nameof(folderPath)}: '{folderPath}'; {nameof(appId)}: {appId}", timer: true);
        //string randomNameWithoutExtension;
        //do
        //{
        //    randomNameWithoutExtension = $"App-{appId:0000}-{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
        //}
        //while (File.Exists(Path.Combine(folderPath, $"{randomNameWithoutExtension}.dll")));
        //return l.ReturnAsOk(randomNameWithoutExtension);
    }

    ///// <summary>
    ///// Normalize full file path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
    ///// </summary>
    ///// <param name="fullPath"></param>
    ///// <returns></returns>
    //private static string NormalizeFullPath(string fullPath) => new FileInfo(fullPath).FullName;
}