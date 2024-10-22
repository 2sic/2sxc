using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Razor;
using System.Web.Razor.Generator;
using ToSic.Eav.Caching;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using CodeCompiler = ToSic.Sxc.Code.Internal.HotBuild.CodeCompiler;

namespace ToSic.Sxc.Dnn.Razor.Internal
{
    /// <summary>
    /// This class is responsible for managing the compilation of Razor templates using Roslyn.
    /// </summary>
    public class RoslynBuildManager(
        AssemblyCacheManager assemblyCacheManager,
        LazySvc<AppCodeLoader> appCodeLoader,
        AssemblyResolver assemblyResolver,
        IReferencedAssembliesProvider referencedAssembliesProvider,
        MemoryCacheService memoryCacheService)
        : ServiceBase("Dnn.RoslynBuildManager", connect: [assemblyCacheManager, appCodeLoader, assemblyResolver, referencedAssembliesProvider, memoryCacheService]),
            IRoslynBuildManager
    {
        private static readonly NamedLocks CompileAssemblyLocks = new();

        private const string DefaultNamespace = "RazorHost";

        // TODO: THIS IS PROBABLY Wrong, but not important for now
        // It's wrong, because the web.config gives the default to be a very old 2sxc base class
        private const string FallbackBaseClass = "System.Web.WebPages.WebPageBase";

        /// <summary>
        /// Manage template compilations, cache the assembly and returns the generated type.
        /// </summary>
        /// <param name="codeFileInfo"></param>
        /// <param name="spec"></param>
        /// <returns>The generated type for razor cshtml.</returns>
        public Type GetCompiledType(CodeFileInfo codeFileInfo, HotBuildSpec spec)
            => GetCompiledAssembly(codeFileInfo, null, spec)?.MainType;

        public AssemblyResult GetCompiledAssembly(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};");

            var lockObject = CompileAssemblyLocks.Get(codeFileInfo.FullPath);

            // 2024-02-19 Something is buggy here, so I must add logging to find out what's going on
            // ATM it appears that sometimes it returns null, but I don't know why
            // I believe it's mostly on first startup or something
            var (result, generated, message) = new TryLockTryDo(lockObject).Call(
                conditionToGenerate: () => assemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath)?.MainType == null,
                generator: () => CompileCodeFile(codeFileInfo, className, spec),
                cacheOrFallback: () => assemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath)
            );

            // ReSharper disable once InvertIf
            if (!generated)
            {
                l.E("Object was not generated - additional logs to better find root cause next time this happens");
                l.A($"result: {result}");
                l.A($"message: {message}");
                var cache = assemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath);
                l.A($"{nameof(cache)}: {cache}");
                l.A($"{nameof(cache.MainType)}: {cache?.MainType}");
                l.A($"{nameof(cache.HasAssembly)}: {cache?.HasAssembly}");
            }

            return l.Return(result, message);

        }

        private AssemblyResult CompileCodeFile(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};", timer: true);

            // Initialize the list of referenced assemblies with the default ones
            var referencedAssemblies = referencedAssembliesProvider.Locations(codeFileInfo.RelativePath, spec);

            // Roslyn compiler need reference to location of dll, when dll is not in bin folder
            // get assembly - try to get from cache, otherwise compile
            var lTimer = Log.Fn("Timer AppCodeLoader", timer: true);
            var (appCodeAssemblyResult, _) = appCodeLoader.Value.GetAppCode(spec);

            // Add the latest assembly to the .net assembly resolver (singleton)
            assemblyResolver.AddAssembly(appCodeAssemblyResult?.Assembly);

            var appCodeAssembly = appCodeAssemblyResult?.Assembly;
            if (appCodeAssembly != null)
            {
                var assemblyLocation = appCodeAssembly.Location;
                referencedAssemblies.Add(assemblyLocation);
                l.A($"Added reference to AppCode assembly: {assemblyLocation}");
            }
            lTimer.Done();

            // Compile the template
            lTimer = Log.Fn("timer for Compile", timer: true);
            var pathLowerCase = codeFileInfo.RelativePath.ToLowerInvariant();
            var isCshtml = pathLowerCase.EndsWith(CodeCompiler.CsHtmlFileExtension);
            if (isCshtml) className = GetSafeClassName(codeFileInfo.FullPath);
            l.A($"Compiling template. Class: {className}");

            var (generatedAssembly, errors) = isCshtml
                ? CompileRazor(codeFileInfo.SourceCode, referencedAssemblies, className, DefaultNamespace, codeFileInfo.FullPath)
                : CompileCSharpCode(codeFileInfo.SourceCode, referencedAssemblies);

            if (generatedAssembly == null)
                throw l.Ex(new Exception(
                    $"Found {errors.Count} errors compiling Razor '{codeFileInfo.FullPath}'" +
                    $" (length: {codeFileInfo.SourceCode.Length}," +
                    $" lines: {codeFileInfo.SourceCode.Split('\n').Length}):" +
                    $" {ErrorMessagesFromCompileErrors(errors)}")
                );
            lTimer.Done();

            // Add the compiled assembly to the cache

            lTimer = Log.Fn("timer for ChangeMonitors", timer: true);

            // directly attach a type to the cache
            var mainType = FindMainType(generatedAssembly, className, isCshtml);
            l.A($"Main type: {mainType}");

            var assemblyResult = new AssemblyResult(generatedAssembly, safeClassName: className, mainType: mainType);
            assemblyResult.CacheDependencyId = AssemblyCacheManager.KeyTemplate(codeFileInfo.FullPath);

            

            assemblyCacheManager.Add(
                cacheKey: assemblyResult.CacheDependencyId,
                data: assemblyResult,
                slidingDuration: CacheConstants.DurationRazorAndCode,
                filePaths: [codeFileInfo.FullPath], // better to only monitor the current file
                                                    // otherwise all caches keep getting flushed when any file changes
                                                    // TODO: must also watch for global shared code changes
                dependencies: appCodeAssembly == null 
                    ? null 
                    : [appCodeAssemblyResult]
                );
            lTimer.Done();

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
                throw new(message);

            var startPos = relativePath.IndexOf("/2sxc/", 10, StringComparison.Ordinal); // start from 10 to skip '/' before 'site-id-or-name'
            if (startPos < 0) throw new(message);

            // find position of 5th slash in relativePath 
            var pos = startPos + 6; // skipping first 4 slashes
            pos = relativePath.IndexOf('/', pos + 1);
            if (pos < 0) throw new(message);

            return relativePath.Substring(0, pos).Backslash();
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
        /// Compiles the Razor into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileRazor(string sourceCode, List<string> referencedAssemblies, string className, string defaultNamespace, string sourceFileName)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"{nameof(sourceCode)}: {sourceCode.Length} chars");

            // Find the base class for the template
            var baseClass = FindBaseClass(sourceCode);
            l.A($"Base class: {baseClass}");

            // Create the Razor template engine host
            var engine = CreateRazorTemplateEngine(className, baseClass, defaultNamespace);

            // Generate C# code from Razor template
            var lTimer = Log.Fn("Generate Code", timer: true);
            using var reader = new StringReader(sourceCode);
            var razorResults = engine.GenerateCode(reader, className, defaultNamespace, sourceFileName);
            lTimer.Done();

            // Compile the template into an assembly
            var compiler = GetCSharpCodeProvider();
            lTimer = Log.Fn("Compile", timer: true);
            var compilerParameters = RazorCompilerParameters(referencedAssemblies);
            var compilerResults = compiler.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            lTimer.Done();

            if (compilerResults.Errors.Count <= 0)
                return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

            // compilation errors (but not warnings)
            var errorList = compilerResults.Errors.Cast<CompilerError>().Where(e => !e.IsWarning).ToList();

            return (!errorList.Any())
                ? l.ReturnAsOk((compilerResults.CompiledAssembly, null)) 
                : l.ReturnAsError((null, errorList), "error");
        }

        /// <summary>
        /// WIP Experimental 2dm - multiple compiles seem to take much less time - 0.4 instead of 2-2.5 seconds.
        /// So I'm experimenting with temporary caching of the compiler provider.
        ///
        /// Current results: saves ca. 2 seconds when compiling Razor, not sure about the memory impact and other side effects though.
        ///
        /// But the performance benefit is not consistent...
        /// </summary>
        /// <returns></returns>
        private CSharpCodeProvider GetCSharpCodeProvider()
        {
            var l = Log.Fn<CSharpCodeProvider>(timer: true);
            // See if in memory cache
            if (memoryCacheService.TryGet<CSharpCodeProvider>(CSharpCodeProviderCacheKey, out var fromCache))
                return l.Return(fromCache, "from cached");
            
            var codeProvider = new CSharpCodeProvider(); // TODO: @stv test with latest nuget package for @inherits ; issue
            // add to memory cache for 5 minute floating expiry
            memoryCacheService.SetNew(CSharpCodeProviderCacheKey, codeProvider, p=> p.SetSlidingExpiration(CSharpCodeProviderCacheMinutes * 60));
            // memoryCacheService.Add(CSharpCodeProviderCacheKey, codeProvider, new CacheItemPolicy { SlidingExpiration = new(0, CSharpCodeProviderCacheMinutes, 0) });

            return l.Return(codeProvider, "created new and added to cache for 1 min");
        }
        private const string CSharpCodeProviderCacheKey = "Sxc-Dnn-CSharpCodeProvider";
        private const int CSharpCodeProviderCacheMinutes = 5;   // basic idea is that at startup, there is usually more to compile, after a while it's not so important any more.


        private static CompilerParameters RazorCompilerParameters(List<string> referencedAssemblies)
        {
            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,

                CompilerOptions = $"{DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };
            return compilerParameters;
        }


        /// <summary>
        /// Compiles the C# code into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileCSharpCode(string csharpCode, List<string> referencedAssemblies)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"C# code content length: {csharpCode.Length}");

            var lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,
                CompilerOptions = $"{DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };
            lTimer.Done();

            // Compile the C# code into an assembly
            var compiler = GetCSharpCodeProvider();
            lTimer = Log.Fn("Compile", timer: true);
            var compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, csharpCode);
            lTimer.Done();

            if (compilerResults.Errors.Count <= 0)
                return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

            // compilation errors (but not warnings)
            var errorList = compilerResults.Errors.Cast<CompilerError>().Where(e => !e.IsWarning).ToList();

            return (!errorList.Any())
                ? l.ReturnAsOk((compilerResults.CompiledAssembly, null))
                : l.ReturnAsError((null, errorList), "error");
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
        private string FindBaseClass(string template)
        {
            var l = Log.Fn<string>($"{nameof(template)}: {template.Length} chars");
            var baseClass = FallbackBaseClass;
            try
            {
                var inheritsMatch = Regex.Match(template, @"@inherits\s+(?<BaseName>[\w\.]+)", RegexOptions.Multiline);

                if (!inheritsMatch.Success)
                    return l.Return(FallbackBaseClass, $"no @inherits found, fallback to '{FallbackBaseClass}'");

                baseClass = inheritsMatch.Groups["BaseName"].Value;
                if (baseClass.IsEmptyOrWs())
                    return l.Return(FallbackBaseClass, $"@inherits empty string, fallback to '{FallbackBaseClass}'");

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                //Type.GetType(baseClass, true); // test for known type

                return l.ReturnAsOk(baseClass);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return l.ReturnAsError(baseClass, "error");
            }
        }

        /// <summary>
        /// Creates a new instance of the RazorTemplateEngine class with the specified configuration.
        ///
        /// Basically imitating https://github.com/Antaris/RazorEngine/blob/master/src/source/RazorEngine.Core/Compilation/CompilerServiceBase.cs#L203-L229
        /// </summary>
        /// <returns>The initialized RazorTemplateEngine instance.</returns>
        private RazorTemplateEngine CreateRazorTemplateEngine(string className, string baseClass, string defaultNamespace)
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