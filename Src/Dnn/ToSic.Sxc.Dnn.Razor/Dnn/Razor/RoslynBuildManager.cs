﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.WebPages;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Dnn.Razor
{
    /// <summary>
    /// This class is responsible for managing the compilation of Razor templates using Roslyn.
    /// </summary>
    public class RoslynBuildManager : ServiceBase
    {
        private readonly ISite _site;
        private readonly IAppStates _appStates;
        private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
        private readonly AssemblyCacheManager _assemblyCacheManager;

        public RoslynBuildManager(ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, AssemblyCacheManager assemblyCacheManager) : base("Dnn.RoslynBuildManager")
        {
            ConnectServices(
                _site = site,
                _appStates = appStates,
                _appPathsLazy = appPathsLazy,
                _assemblyCacheManager = assemblyCacheManager
            );
        }

        private string _className;
        private string _baseClass;
        //private List<string> _referencedAssemblies;
        //private string _errorMessage;

        private static readonly Lazy<List<string>> DefaultReferencedAssemblies = new(GetDefaultReferencedAssemblies);

        // TODO: @STV THIS LOOKS problematic, make sure we don't add anything which isn't included by default #Compile
        //private static readonly List<string> ReferencedNamespaces =
        //[
        //    "System",
        //    "System.Text",
        //    "System.Collections.Generic",
        //    "System.Linq",
        //    "System.IO",
        //    "System.Web.WebPages"
        //];

        /// <summary>
        /// Manage template compilations, cache the assembly and returns the generated type.
        /// </summary>
        /// <param name="templatePath">Relative path to template file.</param>
        /// <param name="appId">The ID of the application.</param>
        /// <returns>The generated type for razor cshtml.</returns>
        public Type GetCompiledType(string templatePath, int appId)
        {
            var l = Log.Fn<Type>($"{nameof(templatePath)}: '{templatePath}'; {nameof(appId)}: {appId}", timer: true);

            var templateFullPath = HostingEnvironment.MapPath(templatePath);

            // Check if the template is already in the assembly cache
            Assembly generatedAssembly = null;
            if (AssemblyCacheManager.HasTemplate(templateFullPath))
            {
                var assemblyResult = AssemblyCacheManager.GetTemplate(templateFullPath);
                generatedAssembly = assemblyResult?.Assembly;
                _className = assemblyResult?.SafeClassName;
                l.A($"Template found in cache. Assembly: {generatedAssembly?.FullName}, Class: {_className}");
            }

            // If the assembly was not in the cache, we need to compile it
            if (generatedAssembly == null)
            {
                l.A($"Template not found in cache. Path: {templateFullPath}");

                // Initialize the list of referenced assemblies with the default ones
                List<string> referencedAssemblies = [..DefaultReferencedAssemblies.Value];

                // Roslyn compiler need reference to location of dll, when dll is not in bin folder
                var myAppCodeAssembly = AssemblyCacheManager.GetMyAppCode(appId)?.Assembly;
                if (myAppCodeAssembly != null)
                {
                    referencedAssemblies.Add(myAppCodeAssembly.Location);
                    l.A($"Added reference to MyApp.Code assembly: {myAppCodeAssembly.Location}");
                }

                // Compile the template
                _className = GetSafeClassName(templateFullPath);
                l.A($"Compiling template. Class: {_className}");

                var template = File.ReadAllText(templateFullPath);
                (generatedAssembly, var errors) = CompileTemplate(template, referencedAssemblies);
                if (generatedAssembly == null)
                    throw l.Ex(new Exception(
                        $"Found {errors.Count} errors compiling Razor '{templateFullPath}' (length: {template.Length}, lines: {template.Split('\n').Length}): {ErrorMessagesFromCompileErrors(errors)}"));

                // Add the compiled assembly to the cache
                _assemblyCacheManager.Add(
                    cacheKey: AssemblyCacheManager.KeyTemplate(templateFullPath),
                    data: new AssemblyResult(generatedAssembly, safeClassName: _className),
                    duration: 3600,
                    appPaths: GetAppFolderPaths(appId));
            }

            // Find the generated type in the assembly and return it
            return l.ReturnAsOk(generatedAssembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(_className)));
        }

        private static List<string> GetDefaultReferencedAssemblies()
        {
            var referencedAssemblies = new List<string>
            {
                "System.dll",
                "System.Core.dll",
                typeof(IHtmlString).Assembly.Location, // System.Web
                "Microsoft.CSharp.dll" // dynamic support!
            };

            try
            {
                foreach (var dll in Directory.GetFiles(HttpRuntime.BinDirectory, "*.dll"))
                    referencedAssemblies.Add(dll);
            }
            catch
            {
                // sink
            }

            // deduplicate referencedAssemblies by filename, keep last duplicate
            referencedAssemblies = referencedAssemblies
                .GroupBy(Path.GetFileName)
                .Select(g => g.Last())
                .ToList();

            return referencedAssemblies;
        }

        private string GetSafeClassName(string templateFullPath)
        {
            if (string.IsNullOrWhiteSpace(templateFullPath))
                return "RazorView" + Guid.NewGuid().ToString().Replace("-", "_");
            return "RazorView" + Path.GetFileNameWithoutExtension(templateFullPath).Replace("-", "_");
        }

        /// <summary>
        /// Compiles the template into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileTemplate(string template, List<string> referencedAssemblies)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"Template content length: {template.Length}");

            //// fix template
            //template = FixTemplate(template);

            // Find the base class for the template
            _baseClass = FindTemplateType(template);
            l.A($"Base class: {_baseClass}");

            // Create the Razor template engine host
            var engine = CreateHost();

            using var reader = new StringReader(template);
            var razorResults = engine.GenerateCode(reader);

            var codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false
            };

            // Compile the template into an assembly
            var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            //var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, template);

            //var mappings = razorResults.DesignTimeLineMappings;

            if (compilerResults.Errors.Count <= 0)
                return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

            // Handle compilation errors
            var errorList = compilerResults.Errors.Cast<CompilerError>().ToList();

            return l.ReturnAsError((null, errorList), "error");
        }

        private string ErrorMessagesFromCompileErrors(List<CompilerError> errors)
        {
            var l = Log.Fn<string>(timer: true);
            var compileErrors = new StringBuilder();
            foreach (var compileError in errors)
                compileErrors.AppendLine($"Line: {compileError.Line}, Column: {compileError.Column}, Error: {compileError.ErrorText}");
            return l.ReturnAsOk(compileErrors.ToString());
        }

        private string[] GetAppFolderPaths(int appId)
        {
            var l = Log.Fn<string[]>($"{nameof(appId)}: {appId}");
            var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(appId));
            l.A($"AppPaths: {appPaths.PhysicalPath}, {appPaths.PhysicalPathShared}");
            return l.ReturnAsOk([appPaths.PhysicalPath, appPaths.PhysicalPathShared]);
        }

        /// <summary>
        /// Finds the type of the template based on the template content.
        /// </summary>
        /// <param name="template">The template content.</param>
        /// <returns>The type of the template.</returns>
        private static string FindTemplateType(string template)
        {
            try
            {
                if (!template.Contains("@inherits ")) return "System.Web.WebPages.WebPageBase";

                // extract the type name from the template
                var at = template.IndexOf("@inherits ", StringComparison.Ordinal);
                var at2 = template.IndexOf("\n", at, StringComparison.Ordinal);
                if (at2 == -1) at2 = template.Length;
                var line = template.Substring(at, at2 - at);
                line = line.Trim();
                if (line.EndsWith(";")) line = line.Substring(0, line.Length - 1);
                var typeName = line.Replace("@inherits ", "").Trim();

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Type.GetType(typeName, true); // test for known type

                return typeName;
            }
            catch (Exception)
            {
                // fallback;
                return "System.Web.WebPages.WebPageBase";
            }
        }

        /// <summary>
        /// Creates a new instance of the RazorTemplateEngine class with the specified configuration.
        /// </summary>
        /// <returns>The initialized RazorTemplateEngine instance.</returns>
        private RazorTemplateEngine CreateHost()
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = _baseClass,
                DefaultClassName = _className,
                DefaultNamespace = "RazorHost"
            };

            var context = new GeneratedClassContext("Execute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", typeof(HelperResult).FullName, "DefineSection")
            {
                ResolveUrlMethodName = "ResolveUrl"
            };

            host.GeneratedClassContext = context;

            //foreach (var ns in ReferencedNamespaces) host.NamespaceImports.Add(ns);

            return new RazorTemplateEngine(host);
        }
    }
}