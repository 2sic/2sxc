//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.Emit;
//using Microsoft.CodeAnalysis.Text;

using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Razor;
using System.Web.Razor.Generator;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Dnn.Compile;
using CodeCompiler = ToSic.Sxc.Code.Internal.HotBuild.CodeCompiler;

namespace ToSic.Sxc.Dnn.Razor.Internal
{
    /// <summary>
    /// This class is responsible for managing the compilation of Razor templates using Roslyn.
    /// </summary>
    public class RoslynBuildManager : ServiceBase, IRoslynBuildManager
    {
        private const string DefaultNamespace = "RazorHost";

        // TODO: THIS IS PROBABLY Wrong, but not important for now
        // It's wrong, because the web.config gives the default to be a very old 2sxc base class
        private const string FallbackBaseClass = "System.Web.WebPages.WebPageBase";

        private readonly AssemblyCacheManager _assemblyCacheManager;
        private readonly LazySvc<ThisAppCodeLoader> _thisAppCodeLoader;
        private readonly AssemblyResolver _assemblyResolver;

        public RoslynBuildManager(AssemblyCacheManager assemblyCacheManager, LazySvc<ThisAppCodeLoader> thisAppCodeLoader, AssemblyResolver assemblyResolver) : base("Dnn.RoslynBuildManager")
        {
            ;
            ConnectServices(
                _assemblyCacheManager = assemblyCacheManager,
                _thisAppCodeLoader = thisAppCodeLoader,
                _assemblyResolver = assemblyResolver
            );
        }

        private static readonly Lazy<List<string>> DefaultReferencedAssemblies = new(GetDefaultReferencedAssemblies);

        /// <summary>
        /// Manage template compilations, cache the assembly and returns the generated type.
        /// </summary>
        /// <param name="templatePath">Relative path to template file.</param>
        /// <param name="spec"></param>
        /// <returns>The generated type for razor cshtml.</returns>
        public Type GetCompiledType(string templatePath, HotBuildSpec spec)
            => GetCompiledAssembly(templatePath, null, spec).MainType;

        public AssemblyResult GetCompiledAssembly(string virtualPath, string className, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'", timer: true);

            var fileFullPath = HostingEnvironment.MapPath(virtualPath);

            // Check if the template is already in the assembly cache
            var (result, cacheKey) = AssemblyCacheManager.TryGetTemplate(fileFullPath);
            if (result != null)
            {
                var cacheAssembly = result.Assembly;
                var cacheClassName = result.SafeClassName;
                l.A($"Template found in cache. Assembly: {cacheAssembly?.FullName}, Class: {cacheClassName}");
                if (result.MainType != null)
                    return l.ReturnAsOk(result);
            }

            // If the assembly was not in the cache, we need to compile it
            l.A($"Template not found in cache. Path: {fileFullPath}");

            // Initialize the list of referenced assemblies with the default ones
            List<string> referencedAssemblies = [.. DefaultReferencedAssemblies.Value];

            // Roslyn compiler need reference to location of dll, when dll is not in bin folder
            // get assembly - try to get from cache, otherwise compile
            var codeAssembly = ThisAppCodeLoader.TryGetAssemblyOfCodeFromCache(spec, Log)?.Assembly
                               ?? _thisAppCodeLoader.Value.GetAppCodeAssemblyOrThrow(spec);

            _assemblyResolver.AddAssembly(codeAssembly);

            var thisAppCode = AssemblyCacheManager.TryGetThisAppCode(spec);
            var thisAppCodeAssembly = thisAppCode.Result?.Assembly;
            if (thisAppCodeAssembly != null)
            {
                var assemblyLocation = thisAppCodeAssembly.Location;
                referencedAssemblies.Add(assemblyLocation);
                l.A($"Added reference to ThisApp.Code assembly: {assemblyLocation}");
            }

            // Compile the template
            var pathLowerCase = virtualPath.ToLowerInvariant();
            var isCshtml = pathLowerCase.EndsWith(CodeCompiler.CsHtmlFileExtension);
            if (isCshtml) className = GetSafeClassName(fileFullPath);
            l.A($"Compiling template. Class: {className}");

            var template = File.ReadAllText(fileFullPath);
            var (generatedAssembly, errors) = isCshtml
                ? CompileTemplate(template, referencedAssemblies, className, DefaultNamespace, fileFullPath)
                : CompileCSharpCode(template, referencedAssemblies);
            if (generatedAssembly == null)
                throw l.Ex(new Exception(
                    $"Found {errors.Count} errors compiling Razor '{fileFullPath}' (length: {template.Length}, lines: {template.Split('\n').Length}): {ErrorMessagesFromCompileErrors(errors)}"));

            // Add the compiled assembly to the cache

            // Changed again: better to only monitor the current file
            // otherwise all caches keep getting flushed when any file changes
            // TODO: must also watch for global shared code changes

            var fileChangeMon = new HostFileChangeMonitor(new[] { fileFullPath });
            var sharedFolderChangeMon = thisAppCode.Result == null ? null : new FolderChangeMonitor(thisAppCode.Result.WatcherFolders);
            var changeMonitors = thisAppCode.Result == null
                ? new ChangeMonitor[] { fileChangeMon }
                : [fileChangeMon, sharedFolderChangeMon];

            // directly attach a type to the cache
            var mainType = FindMainType(generatedAssembly, className, isCshtml);
            l.A($"Main type: {mainType}");

            var assemblyResult = new AssemblyResult(generatedAssembly, safeClassName: className, mainType: mainType);

            _assemblyCacheManager.Add(
                cacheKey: cacheKey,
                data: assemblyResult,
                duration: 3600,
                changeMonitor: changeMonitors,
                //appPaths: appPaths,
                updateCallback: null);

            return l.ReturnAsOk(assemblyResult);
        }

        private Type FindMainType(Assembly generatedAssembly, string className, bool isCshtml)
        {
            var l = Log.Fn<Type>($"{nameof(className)}: '{className}'; {nameof(isCshtml)}: {isCshtml}", timer: true);
            if (generatedAssembly == null) return l.ReturnAsError(null, "generatedAssembly is null, so type is null");

            var mainType = generatedAssembly.GetType(isCshtml ? $"{DefaultNamespace}.{className}" : className, false, true);
            if (mainType != null) return l.ReturnAsOk(mainType);

            l.A("can't find MainType in standard way, fallback #1 - search by classname, ignoring namespace");
            foreach (var mainTypeFallback1 in generatedAssembly.GetTypes())
                if (mainTypeFallback1.Name.Equals(className, StringComparison.OrdinalIgnoreCase))
                    return l.ReturnAsOk(mainTypeFallback1);

            l.A("can't find mainTypeFallback1, fallback #2 - just return first type (in most cases we have one only)");
            var mainTypeFallback2 = generatedAssembly.GetTypes().FirstOrDefault();
            return l.ReturnAsOk(mainTypeFallback2);
        }


        /// <summary>
        /// extract appRelativePath from relativePath
        /// </summary>
        /// <param name="relativePath">string "/Portals/site-id-or-name/2sxc/app-folder-name/etc..."</param>
        /// <returns>string "\\Portals\\site-id-or-name\\2sxc\\app-folder-name" or null</returns>
        private static string GetAppRelativePath(string relativePath)
        {
            // TODO: stv, this has to more generic because it is very 2sxc on DNN specific, only for default case of templates under /Portals/xxxx/ folder

            // validations
            var message = $"relativePath:'{relativePath}' is not in format '/Portals/site-id-or-name/2sxc/app-folder-name/etc...'";
            if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith("/Portals/"))
                throw new Exception(message);

            var startPos = relativePath.IndexOf("/2sxc/", 10); // start from 10 to skip '/' before 'site-id-or-name'
            if (startPos < 0) throw new Exception(message);

            // find position of 5th slash in relativePath 
            var pos = startPos + 6; // skipping first 4 slashes
            pos = relativePath.IndexOf('/', pos + 1);
            if (pos < 0) throw new Exception(message);

            return relativePath.Substring(0, pos).Backslash();
        }

        private static List<string> GetDefaultReferencedAssemblies()
        {
            // TODO: @STV - this is different here and in the AppCode
            // it's unclear why, so either make it the same, or document why it's different
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
                    if (IsValidAssembly(dll)) referencedAssemblies.Add(dll);
            }
            catch
            {
                // sink
            }

            // deduplicate referencedAssemblies by filename, keep last duplicate
            referencedAssemblies = referencedAssemblies
                //.Where(IsValidAssembly)
                .GroupBy(Path.GetFileName)
                .Select(g => g.Last())
                .ToList();

            return referencedAssemblies;
        }

        /// <summary>
        /// We need to skip invalid assemblies to not break compile process
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsValidAssembly(string filePath)
        {
            try
            {
                Assembly.ReflectionOnlyLoadFrom(filePath);
                return true;
            }
            catch
            {
                return false;
            }
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
        private (Assembly Assembly, List<CompilerError> Errors) CompileTemplate(string template, List<string> referencedAssemblies, string className, string defaultNamespace, string sourceFileName)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"Template content length: {template.Length}");

            // Find the base class for the template
            var baseClass = FindBaseClass(template);
            l.A($"Base class: {baseClass}");

            var designTimeLineMappings = new Dictionary<int, GeneratedCodeMapping>();

            // Create the Razor template engine host
            var engine = CreateHost(className, baseClass, defaultNamespace);

            // Generate C# code from Razor template
            var lTimer = Log.Fn("Generate Code", timer: true);
            using var reader = new StringReader(template);
            var razorResults = engine.GenerateCode(reader, className, defaultNamespace, sourceFileName);
            lTimer.Done();

            lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,

                CompilerOptions = $"{DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };
            lTimer.Done();

            // Compile the template into an assembly
            lTimer = Log.Fn("Compile", timer: true);
            var codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

            var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            lTimer.Done();

            if (compilerResults.Errors.Count <= 0)
                return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

            // Handle compilation errors
            var errorList = compilerResults.Errors.Cast<CompilerError>().ToList();

            return l.ReturnAsError((null, errorList), "error");
        }

        // TODO: use roslyn compiler directly (need nuget packages)
        //private (Assembly Assembly, List<string> Errors) RoslynCompileTemplate(string template, List<string> referencedAssemblies, string className)
        //{
        //    var l = Log.Fn<(Assembly, List<string>)>(timer: true, parameters: $"Template content length: {template.Length}");

        //    // Find the base class for the template
        //    var baseClass = FindTemplateType(template);
        //    l.A($"Base class: {baseClass}");

        //    // Create the Razor template engine host
        //    var engine = CreateHost(className, baseClass);

        //    var lTimer = Log.Fn("Generate Code", timer: true);
        //    using var reader = new StringReader(template);
        //    var razorResults = engine.GenerateCode(reader);
        //    var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(template));
        //    lTimer.Done();

        //    lTimer = Log.Fn("Compiler Params", timer: true);
        //    var references = referencedAssemblies.Select(r => MetadataReference.CreateFromFile(r)).ToList();
        //    lTimer.Done();

        //    // Compile the template into an assembly
        //    lTimer = Log.Fn("Compile", timer: true);
        //    var compilation = CSharpCompilation.Create(
        //        "CompiledTemplateAssembly",
        //        new[] { syntaxTree },
        //        references,
        //        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        //            .WithOptimizationLevel(OptimizationLevel.Debug)
        //            .WithWarningLevel(4)
        //            .WithLanguageVersion(LanguageVersion.CSharp8)
        //    );
        //    using var ms = new MemoryStream();
        //    EmitResult result = compilation.Emit(ms);
        //    lTimer.Done();

        //    if (!result.Success)
        //    {
        //        // Handle compilation errors
        //        var errorList = result.Diagnostics
        //            .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error)
        //            .Select(diagnostic => diagnostic.ToString())
        //            .ToList();

        //        return l.ReturnAsError((null, errorList), "Compilation error");
        //    }

        //    ms.Seek(0, SeekOrigin.Begin);
        //    var compiledAssembly = Assembly.Load(ms.ToArray());

        //    return l.ReturnAsOk((compiledAssembly, null));
        //}

        /// <summary>
        /// Compiles the C# code into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileCSharpCode(string csharpCode, List<string> referencedAssemblies)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"C# code content length: {csharpCode.Length}");

            var lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = new CompilerParameters(referencedAssemblies.ToArray())
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,
                CompilerOptions = $"{DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };
            lTimer.Done();

            // Compile the C# code into an assembly
            lTimer = Log.Fn("Compile", timer: true);
            var codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider(); // TODO: @stvtest with latest nuget package for @inherits ; issue

            var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, csharpCode);
            lTimer.Done();

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


        /// <summary>
        /// Finds the type of the template based on the template content.
        /// </summary>
        /// <param name="template">The template content.</param>
        /// <returns>The type of the template.</returns>
        private static string FindBaseClass(string template)
        {
            try
            {
                // TODO: stv, use existing 2sxc regex for extraction (from CodeType)

                if (!template.Contains("@inherits ")) return FallbackBaseClass;

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
                return FallbackBaseClass;
            }
        }

        /// <summary>
        /// Creates a new instance of the RazorTemplateEngine class with the specified configuration.
        ///
        /// Basically imitating https://github.com/Antaris/RazorEngine/blob/master/src/source/RazorEngine.Core/Compilation/CompilerServiceBase.cs#L203-L229
        /// </summary>
        /// <returns>The initialized RazorTemplateEngine instance.</returns>
        private RazorTemplateEngine CreateHost(string className, string baseClass, string defaultNamespace)
        {
            var l = Log.Fn<RazorTemplateEngine>($"{nameof(className)}: '{className}'; {nameof(baseClass)}: '{baseClass}'", timer: true);

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = baseClass,
                DefaultClassName = className,
                DefaultNamespace = defaultNamespace
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

            // add implicit usings
            foreach (var ns in ImplicitUsings.ForRazor) host.NamespaceImports.Add(ns);

            var engine = new RazorTemplateEngine(host);
            return l.ReturnAsOk(engine);
        }
    }
}