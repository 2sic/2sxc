using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web;
using System.Web.Compilation;
using Microsoft.CSharp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Web;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class CodeCompilerRoslynNetFull : CodeCompiler
    {
        public IHostingEnvironmentWrapper HostingEnvironment { get; }
        public IReferencedAssembliesProvider ReferencedAssembliesProvider { get; }

        public CodeCompilerRoslynNetFull(IServiceProvider serviceProvider, IHostingEnvironmentWrapper hostingEnvironment, IReferencedAssembliesProvider referencedAssembliesProvider) : base(serviceProvider)
        {
            ConnectServices(
                HostingEnvironment = hostingEnvironment,
                ReferencedAssembliesProvider = referencedAssembliesProvider
            );
        }

        private bool _save;
        private string _assemblyName;
        private string _assemblyFilePath;
        private string _pdbFilePath;
        private static readonly string AssemblyFolderPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), AppCodeLoader.AppCodeFolder);

        protected internal override AssemblyResult GetAssembly(string relativePath, string className = null)
        {
            var fullPath = NormalizeFullPath(HostingEnvironment.MapPath(relativePath));

            // 1. Handle Compile standalone file
            if (File.Exists(fullPath))
            {
                var assembly = BuildManager.GetCompiledAssembly(relativePath);
                return new AssemblyResult(assembly: assembly);
            }

            // When compile all folder, need to save dll so latter can be loaded by roslyn compiler
            _save = Directory.Exists(fullPath);

            // 2. Handle Compile all in folder
            if (_save)
            {
                _assemblyName = GetRandomUniqueNameInFolder(); // need random name, because assemblies has to be preserved on disk, and we can not replace them until AppDomain is unloaded 
                
                // Ensure /App_Data/AppCode/ folder exists to preserve dlls
                Directory.CreateDirectory(AssemblyFolderPath);

                _assemblyFilePath = Path.Combine(AssemblyFolderPath, $"{_assemblyName}.dll");
                _pdbFilePath = Path.Combine(AssemblyFolderPath, $"{_assemblyName}.pdb");
                var assemblyLocations = new string[] { _pdbFilePath, _assemblyFilePath }; 

                // Get all C# files in the folder
                var sourceFiles = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);

                // Validate are there any C# files
                if (sourceFiles.Length == 0)
                    return new AssemblyResult(errorMessages: $"Error: given path '{relativePath}' doesn't contain any .cs files", assemblyLocations: assemblyLocations);

                var results = GetCompiledAssemblyFromFolder(sourceFiles);

                // Compile ok
                if (!results.Errors.HasErrors)
                    return new AssemblyResult(assembly: results.CompiledAssembly, assemblyLocations: assemblyLocations);

                // Compile error case
                var errors = "";
                foreach (CompilerError error in results.Errors)
                    errors += $"Error ({error.ErrorNumber}): {error.ErrorText}\n";

                return new AssemblyResult(errorMessages: errors, assemblyLocations: assemblyLocations);
            }

            // 3. Path do not exists
            return new AssemblyResult(errorMessages: $"Error: given path '{relativePath}' doesn't exist");
        }

        private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles)
        {
            var provider = new CSharpCodeProvider();
            var parameters = _save // need to save dll so latter can be loaded by roslyn compiler
                ? new CompilerParameters(null, _assemblyFilePath)
                {
                    GenerateInMemory = false,
                    GenerateExecutable = false,
                    IncludeDebugInformation = true,
                    CompilerOptions = "/define:OQTANE;NETCOREAPP;NET5_0 /optimize-",
                }
                : new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false,
                    IncludeDebugInformation = true,
                    CompilerOptions = "/define:OQTANE;NETCOREAPP;NET5_0 /optimize-",
                };

            // Add all referenced assemblies
            parameters.ReferencedAssemblies.AddRange(ReferencedAssembliesProvider.Locations());
            return provider.CompileAssemblyFromFile(parameters, sourceFiles);
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
        {
            var compiledType = BuildManager.GetCompiledType(relativePath);
            var errMsg = compiledType == null
                ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
            return (compiledType, errMsg);
        }

        /// <summary>
        /// Generates a random name for a dll file and ensures it does not already exist in the App_Data/AppCode folder.
        /// </summary>
        /// <returns>The generated random name.</returns>
        private static string GetRandomUniqueNameInFolder()
        {
            string randomName, filePath;
            do
            {
                randomName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                filePath = Path.Combine(AssemblyFolderPath, $"{randomName}.dll");
            }
            while (File.Exists(filePath));
            return randomName;
        }
    }
}