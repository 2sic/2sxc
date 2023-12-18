using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.WebPages;
using ToSic.Eav.Caching.CachingMonitors;
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
        private const string DefaultNamespace = "RazorHost";

        // TODO: @STV - probably not needed, see below
        //private readonly ISite _site;
        //private readonly IAppStates _appStates;
        //private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
        private readonly AssemblyCacheManager _assemblyCacheManager;

        public RoslynBuildManager(
            // TODO: @STV - probably not needed, see below
            //ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, 
            AssemblyCacheManager assemblyCacheManager) : base("Dnn.RoslynBuildManager")
        {
            ConnectServices(
                // TODO: @STV - probably not needed, see below
                //_site = site,
                //_appStates = appStates,
                //_appPathsLazy = appPathsLazy,
                _assemblyCacheManager = assemblyCacheManager
            );
        }

        private static readonly Lazy<List<string>> DefaultReferencedAssemblies = new(GetDefaultReferencedAssemblies);

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
            var (result, cacheKey) = AssemblyCacheManager.TryGetTemplate(templateFullPath);
            if (result != null) // AssemblyCacheManager.HasTemplate(templateFullPath))
            {
                //var assemblyResult = AssemblyCacheManager.GetTemplate(templateFullPath);
                var cacheAssembly = /*assemblyResult?*/result.Assembly;
                var cacheClassName = /*assemblyResult?*/result.SafeClassName;
                l.A($"Template found in cache. Assembly: {cacheAssembly?.FullName}, Class: {cacheClassName}");
                if (/*cacheAssembly*/ result.MainType != null)
                    return l.ReturnAsOk(result.MainType);// cacheAssembly.GetType(cacheClassName));
                    // before: return l.ReturnAsOk(cacheAssembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(cacheClassName)));
            }

            // If the assembly was not in the cache, we need to compile it
            Assembly generatedAssembly = null;
            string className = null;
            if (generatedAssembly == null)
            {
                l.A($"Template not found in cache. Path: {templateFullPath}");

                // Initialize the list of referenced assemblies with the default ones
                List<string> referencedAssemblies = [.. DefaultReferencedAssemblies.Value];

                // Roslyn compiler need reference to location of dll, when dll is not in bin folder
                var appCode = AssemblyCacheManager.TryGetAppCode(appId);
                var myAppCodeAssembly = appCode.Result?.Assembly;
                if (myAppCodeAssembly != null)
                {
                    referencedAssemblies.Add(myAppCodeAssembly.Location);
                    l.A($"Added reference to MyApp.Code assembly: {myAppCodeAssembly.Location}");
                }

                // Compile the template
                className = GetSafeClassName(templateFullPath);
                l.A($"Compiling template. Class: {className}");

                var template = File.ReadAllText(templateFullPath);
                (generatedAssembly, var errors) = CompileTemplate(template, referencedAssemblies, className);
                if (generatedAssembly == null)
                    throw l.Ex(new Exception(
                        $"Found {errors.Count} errors compiling Razor '{templateFullPath}' (length: {template.Length}, lines: {template.Split('\n').Length}): {ErrorMessagesFromCompileErrors(errors)}"));

                // Add the compiled assembly to the cache

                // TODO: @STV - original tried to get both global and shared path
                // ...but there is no reason for this, the file can only be in one path and never in two
                // ...also throws errors when adding to cache, because the global path often doesn't exist
                //var appPaths = GetAppFolderPaths(appId);
                //var appPaths = new [] { Path.GetDirectoryName(templateFullPath) };

                // Changed again: better to only monitor the current file
                // otherwise all caches keep getting flushed when any file changes
                // TODO: must also watch for global shared code changes

                var fileChangeMon = new HostFileChangeMonitor(new[] { templateFullPath });
                var sharedFolderChangeMon = appCode.Result == null ? null : new FolderChangeMonitor(appCode.Result.WatcherFolders);
                var changeMonitors = appCode.Result == null 
                    ? new ChangeMonitor[] { fileChangeMon } 
                    : [fileChangeMon, sharedFolderChangeMon];

                // FYI: @STV - added ability to directly attach a type to the cache
                // TODO: @STV - you used StartsWith, I changed it to use the full namespace. is that ok?
                var mainType = generatedAssembly.GetType($"{DefaultNamespace}.{className}");
                l.A($"Main type: {mainType}");
                //var typeWithSearch = generatedAssembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(className));
                //l.A($"Type with search: {typeWithSearch}");
                _assemblyCacheManager.Add(
                    cacheKey: cacheKey,
                    data: new AssemblyResult(generatedAssembly, safeClassName: className, mainType: mainType),
                    duration: 3600,
                    changeMonitor: changeMonitors,
                    //appPaths: appPaths,
                    updateCallback: null);
            }

            // Find the generated type in the assembly and return it
            return l.ReturnAsOk(generatedAssembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(className)));
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
            if (!string.IsNullOrWhiteSpace(templateFullPath))
                return "RazorView" + GetSafeString(Path.GetFileNameWithoutExtension(templateFullPath));

            // Fallback class name with a unique identifier
            return "RazorView" + Guid.NewGuid().ToString("N");
        }

        private static string GetSafeString(string input)
        {
            var safeChars = input.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
            var safeString = new string(safeChars);

            // Ensure the class name starts with a letter or underscore
            if (!char.IsLetter(safeString.FirstOrDefault()) && safeString.FirstOrDefault() != '_')
                safeString = "_" + safeString;

            return safeString;
        }


        /// <summary>
        /// Compiles the template into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileTemplate(string template, List<string> referencedAssemblies, string className)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"Template content length: {template.Length}");

            //// fix template
            //template = FixTemplate(template);

            // Find the base class for the template
            var baseClass = FindTemplateType(template);
            l.A($"Base class: {baseClass}");

            // Create the Razor template engine host
            var engine = CreateHost(className, baseClass);

            var lTimer = Log.Fn("Generate Code", timer: true);
            using var reader = new StringReader(template);
            var razorResults = engine.GenerateCode(reader);
            lTimer.Done();

            lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false
            };
            lTimer.Done();

            // Compile the template into an assembly
            lTimer = Log.Fn("Compile", timer: true);
            var codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            lTimer.Done();
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

        // TODO: @STV - probably not needed, see above
        //private string[] GetAppFolderPaths(int appId)
        //{
        //    var l = Log.Fn<string[]>($"{nameof(appId)}: {appId}");
        //    var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(appId));
        //    l.A($"AppPaths: {appPaths.PhysicalPath}, {appPaths.PhysicalPathShared}");
        //    return l.ReturnAsOk([appPaths.PhysicalPath, appPaths.PhysicalPathShared]);
        //}

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
        ///
        /// Basically imitating https://github.com/Antaris/RazorEngine/blob/master/src/source/RazorEngine.Core/Compilation/CompilerServiceBase.cs#L203-L229
        /// </summary>
        /// <returns>The initialized RazorTemplateEngine instance.</returns>
        private RazorTemplateEngine CreateHost(string className, string baseClass)
        {
            var l = Log.Fn<RazorTemplateEngine>($"{nameof(className)}: '{className}'; {nameof(baseClass)}: '{baseClass}'", timer: true);

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = baseClass,
                DefaultClassName = className,
                DefaultNamespace = DefaultNamespace
            };

            var context = new GeneratedClassContext(
                "Execute",
                "Write",
                "WriteLiteral", 
                "WriteTo",
                "WriteLiteralTo",
                typeof(HelperResult).FullName,
                "DefineSection")
            {
                ResolveUrlMethodName = "ResolveUrl"
            };

            host.GeneratedClassContext = context;

            //foreach (var ns in ReferencedNamespaces) host.NamespaceImports.Add(ns);

            var engine = new RazorTemplateEngine(host);
            return l.ReturnAsOk(engine);
        }
    }
}