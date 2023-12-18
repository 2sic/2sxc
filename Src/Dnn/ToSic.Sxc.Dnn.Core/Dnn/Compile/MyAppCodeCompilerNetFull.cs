using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Compilation;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Dnn.Compile;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code;

[PrivateApi]
internal class MyAppCodeCompilerNetFull : MyAppCodeCompiler
{
    public const string CsFiles = ".cs";
    public const bool UseSubfolders = false;

    private readonly IHostingEnvironmentWrapper _hostingEnvironment;
    private readonly IReferencedAssembliesProvider _referencedAssembliesProvider;

    public MyAppCodeCompilerNetFull(IServiceProvider serviceProvider, IHostingEnvironmentWrapper hostingEnvironment, IReferencedAssembliesProvider referencedAssembliesProvider) : base(serviceProvider)
    {
        ConnectServices(
            _hostingEnvironment = hostingEnvironment,
            _referencedAssembliesProvider = referencedAssembliesProvider
        );
    }

    protected internal override AssemblyResult GetAssembly(string relativePath, string className, int appId = 0)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}: '{relativePath}'; {nameof(className)}: '{className}'; {nameof(appId)}: {appId}");

        try
        {
            // Get all C# files in the folder
            var fullPath = NormalizeFullPath(_hostingEnvironment.MapPath(relativePath));
            var sourceFiles = Directory.GetFiles(fullPath, $"*{CsFiles}", UseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // Log all files
            var wrapFiles = l.Fn($"Source Files in {fullPath}:");
            foreach (var sourceFile in sourceFiles) l.A(sourceFile);
            wrapFiles.Done($"{sourceFiles.Length}");

            // Validate are there any C# files
            // TODO: if no files exist, it shouldn't be an error, because it could be that it's just not here yet
            if (sourceFiles.Length == 0)
                return l.ReturnAsError(new AssemblyResult(
                        errorMessages: $"Error: given path '{relativePath}' doesn't contain any {CsFiles} files"),
                    $"given path '{relativePath}' doesn't contain any {CsFiles} files");

            var assemblyLocations = GetAssemblyLocations(appId);

            var results = GetCompiledAssemblyFromFolder(sourceFiles, assemblyLocations[1]);

            // Compile ok
            if (!results.Errors.HasErrors)
                return l.ReturnAsOk(new AssemblyResult(assembly: results.CompiledAssembly, assemblyLocations: assemblyLocations));

            // Compile error case
            var errors = "";
            foreach (CompilerError error in results.Errors)
            {
                var msg = $"Error ({error.ErrorNumber}): {error.ErrorText}";
                l.E(msg);
                errors += $"{msg}\n";
            }

            return l.ReturnAsError(new AssemblyResult(errorMessages: errors), errors);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{className}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage));
        }
    }

    private string[] GetAssemblyLocations(int appId)
    {
        var l = Log.Fn<string[]>($"{nameof(appId)}: {appId}");
        var tempAssemblyFolderPath = Path.Combine(_hostingEnvironment.MapPath("~/App_Data"), "2sxc.bin");
        l.A($"TempAssemblyFolderPath: '{tempAssemblyFolderPath}'");
        // Ensure "2sxc.bin" folder exists to preserve dlls
        Directory.CreateDirectory(tempAssemblyFolderPath);

        // need random name, because assemblies has to be preserved on disk, and we can not replace them until AppDomain is unloaded 
        var assemblyName = GetRandomUniqueNameInFolder(tempAssemblyFolderPath, appId);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = new[] { symbolsFilePath, assemblyFilePath };
        return l.ReturnAsOk(assemblyLocations);
    }

    private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles, string assemblyFilePath)
    {
        var l = Log.Fn<CompilerResults>($"{nameof(sourceFiles)}: {sourceFiles.Length}; {nameof(assemblyFilePath)}: '{assemblyFilePath}'", timer: true);

        var provider = new CSharpCodeProvider();
        // need to save dll so latter can be loaded by roslyn compiler
        var parameters = new CompilerParameters(null, assemblyFilePath)
            {
                GenerateInMemory = false,
                GenerateExecutable = false,
                IncludeDebugInformation = true,
                CompilerOptions = "/optimize- /warnaserror-",
            };

        // Add all referenced assemblies
        parameters.ReferencedAssemblies.AddRange(_referencedAssembliesProvider.Locations());

        var compilerResults = provider.CompileAssemblyFromFile(parameters, sourceFiles);

        return compilerResults.Errors.HasErrors
            ? l.ReturnAsError(compilerResults)
            : l.ReturnAsOk(compilerResults);
    }

    protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
    {
        var l = Log.Fn<(Type Type, string ErrorMessage)>($"{nameof(relativePath)}: '{relativePath}'", timer: true);
        var compiledType = BuildManager.GetCompiledType(relativePath);
        var errMsg = compiledType == null ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
        return l.Return((compiledType, errMsg), errMsg ?? "Ok");
    }

    /// <summary>
    /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
    /// </summary>
    /// <returns>The generated random name.</returns>
    private string GetRandomUniqueNameInFolder(string folderPath, int appId)
    {
        var l = Log.Fn<string>($"{nameof(folderPath)}: '{folderPath}'; {nameof(appId)}: {appId}", timer: true);
        string randomNameWithoutExtension;
        do
        {
            randomNameWithoutExtension = $"App-{appId:0000}-{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
        }
        while (File.Exists(Path.Combine(folderPath, $"{randomNameWithoutExtension}.dll")));
        return l.ReturnAsOk(randomNameWithoutExtension);
    }

    /// <summary>
    /// Normalize full file or folder path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    private static string NormalizeFullPath(string fullPath) => new FileInfo(fullPath).FullName;
}