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

        public override (Assembly Assembly, string ErrorMessages) GetAssembly(string relativePath, string className)
        {
            // TODO:
            // - cache assembly in similar way like we do with custom DataSources
            // - take a care of multi-staging of 2sxc apps

            var fullPath = Path.GetDirectoryName(HostingEnvironment.MapPath(relativePath));

            // 1. Handle Compile standalone file
            if (File.Exists(fullPath))
            { 
                var assembly = BuildManager.GetCompiledAssembly(relativePath);
                return (assembly, null);
            }

            // 2. Handle Compile all in folder
            if (Directory.Exists(fullPath))
            {
                // Get all C# files in the folder
                var sourceFiles = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);

                // Validate are there any C# files
                if (sourceFiles.Length == 0)
                    return (null, $"Error: given path '{relativePath}' doesn't contain any .cs files");

                var results = GetCompiledAssemblyFromFolder(sourceFiles);

                // Compile ok
                if (!results.Errors.HasErrors)
                    return (results.CompiledAssembly, null);

                // Compile error case
                var errors = "";
                foreach (CompilerError error in results.Errors)
                    errors += $"Error ({error.ErrorNumber}): {error.ErrorText}\n";

                return (null, errors);
            }

            // 3. Path do not exists
            return (null, $"Error: given path '{relativePath}' doesn't exist");
        }

        private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = true,
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
    }
}