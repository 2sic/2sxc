using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class CodeCompilerNetFull: CodeCompiler
    {
        public IHostingEnvironmentWrapper HostingEnvironment { get; }
        public IReferencedAssembliesProvider ReferencedAssembliesProvider { get; }

        public CodeCompilerNetFull(IServiceProvider serviceProvider, IHostingEnvironmentWrapper hostingEnvironment, IReferencedAssembliesProvider referencedAssembliesProvider) : base(serviceProvider)
        {
            HostingEnvironment = hostingEnvironment;
            ReferencedAssembliesProvider = referencedAssembliesProvider;
        }

        protected override (Assembly Assembly, string ErrorMessages) GetAssembly(string fileRelativePath, string className)
        {
            var assembly = BuildManager.GetCompiledAssembly(fileRelativePath);
            return (assembly, null);
        }

        public override (Assembly Assembly, string ErrorMessages) GetAssembly2(string folderRelativePath, string className)
        {
            // TODO:
            // - compile all *.cs files to assembly only content of the App_Code folder
            // - fallback to old GetAssembly that will compile single file
            // - cache assembly in similar way like we do with custom DataSources
            // - take a care of multi-staging of 2sxc apps

            var folderPath = Path.GetDirectoryName(HostingEnvironment.MapPath(folderRelativePath));

            // Check is exists folder path
            if (!Directory.Exists(folderPath))
                return (null, $"Error: given path '{folderRelativePath}' doesn't exist");

            // Get all C# files in the folder
            var sourceFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            // Check do we have any C# files
            if (sourceFiles.Length == 0)
                return (null, $"Error: given path '{folderRelativePath}' doesn't contain any .cs files");

            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = true,
            };

            // Add all referenced assemblies
            parameters.ReferencedAssemblies.AddRange(ReferencedAssembliesProvider.Locations());

            var results = provider.CompileAssemblyFromFile(parameters, sourceFiles);

            // Compile ok
            if (!results.Errors.HasErrors) 
                return (results.CompiledAssembly, null);

            // Compile error case
            var errors = "";
            foreach (CompilerError error in results.Errors)
                errors += $"Error ({error.ErrorNumber}): {error.ErrorText}\n";

            return (null, errors);
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
        {
            var compiledType = BuildManager.GetCompiledType(relativePath);
            var errMsg = compiledType == null
                ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
            return (compiledType, errMsg);
        }
    }
}