using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Razor;
using System.Web.Razor.Generator;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sys.Caching;
using ToSic.Sys.Locking;

namespace ToSic.Sxc.Dnn.Razor.Sys
{
    /// <summary>
    /// This class is responsible for managing the compilation of Razor templates using Roslyn.
    /// </summary>
    public class RoslynBuildManager(
        AssemblyCacheManager assemblyCacheManager,
        LazySvc<AppCodeLoader> appCodeLoader,
        AssemblyResolver assemblyResolver,
        IReferencedAssembliesProvider referencedAssembliesProvider,
        MemoryCacheService memoryCacheService,
        IAssemblyDiskCacheService diskCacheService)
        : ServiceBase("Dnn.RoslynBuildManager", connect: [assemblyCacheManager, appCodeLoader, assemblyResolver, referencedAssembliesProvider, memoryCacheService, diskCacheService]),
            IRoslynBuildManager
    {
        private static readonly NamedLocks CompileAssemblyLocks = new();

        internal const string DefaultNamespace = "RazorHost";

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
                conditionToGenerate: () =>
                {
                    // Check memory cache first
                    var memoryResult = assemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath);
                    if (memoryResult?.MainType != null)
                        return false; // Found in memory cache - no need to generate

                    // Memory cache miss - try disk cache
                    l.A("Memory cache miss - checking disk cache");

                    if (!diskCacheService.IsEnabled())
                    {
                        l.A("Disk cache disabled via feature flag - will compile template");
                        return true; // Disk cache disabled via feature flag - need to generate
                    }

                    var contentHash = diskCacheService.ComputeContentHash(codeFileInfo.SourceCode);
                    var appCodeHash = GetAppCodeHash(spec);
                    var diskResult = diskCacheService.TryLoadFromCache(spec, codeFileInfo.RelativePath, contentHash, appCodeHash, this, codeFileInfo);
                    if (diskResult != null)
                    {
                        l.A($"Disk cache hit for template: {codeFileInfo.RelativePath}");
                        // Add to memory cache for faster subsequent access
                        diskResult.CacheDependencyId = AssemblyCacheManager.KeyTemplate(codeFileInfo.FullPath);
                        assemblyCacheManager.Add(
                            cacheKey: diskResult.CacheDependencyId,
                            data: diskResult,
                            slidingDuration: CacheConstants.DurationRazorAndCode,
                            filePaths: [codeFileInfo.FullPath]
                        );
                        return false; // Found in disk cache - no need to generate
                    }

                    l.A("Disk cache miss - will compile template");
                    return true; // Not found anywhere - need to generate
                },
                generator: () => CompileCodeFile(codeFileInfo, className, spec),
                cacheOrFallback: () =>
                {
                    // Try memory cache first
                    var memoryResult = assemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath);
                    if (memoryResult != null)
                        return memoryResult;

                    // Fallback to disk cache (if available during race condition)
                    var contentHash = diskCacheService.ComputeContentHash(codeFileInfo.SourceCode);
                    var appCodeHash = GetAppCodeHash(spec);
                    return diskCacheService.TryLoadFromCache(spec, codeFileInfo.RelativePath, contentHash, appCodeHash, this, codeFileInfo);
                }
            );

            // ReSharper disable once InvertIf
            if (!generated)
            {
                // 2025-09-04 2dm: I can see that !generated seems to happen on AJAX calls, but it doesn't seem to be a problem
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

            var (referencedAssemblies, appCodeAssemblyResult, appCodeAssembly) = ReferencedAssemblies(codeFileInfo, spec);

            // Pre-compute disk cache output path for this compile
            var normalizedPathForCache = CacheKey.NormalizePath(codeFileInfo.RelativePath);
            var preContentHash = diskCacheService.ComputeContentHash(codeFileInfo.SourceCode);
            var preAppCodeHash = GetAppCodeHash(spec);
            var cacheDir = diskCacheService.GetCacheDirectoryPath();
            var outputAssemblyPath = new CacheKey(spec.AppId, spec.Edition, normalizedPathForCache, preContentHash, preAppCodeHash).GetFilePath(cacheDir);
            Directory.CreateDirectory(Path.GetDirectoryName(outputAssemblyPath));

            // Compile the template
            var lTimer = Log.Fn("timer for Compile", timer: true);
            var pathLowerCase = codeFileInfo.RelativePath.ToLowerInvariant();
            var isCshtml = pathLowerCase.EndsWith(SourceCodeConstants.CsHtmlFileExtension);
            if (isCshtml) className = GetSafeClassName(codeFileInfo.FullPath);
            l.A($"Compiling template. Class: {className}; Output: {outputAssemblyPath}");

            var (generatedAssembly, errors) = isCshtml
                ? CompileRazor(codeFileInfo.SourceCode, referencedAssemblies, className, DefaultNamespace, codeFileInfo.FullPath, outputAssemblyPath)
                : CompileCSharpCode(codeFileInfo.SourceCode, referencedAssemblies, outputAssemblyPath);

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
            var mainType = FindMainType(generatedAssembly, className, isCshtml, Log);
            l.A($"Main type: {mainType}");

            var assemblyResult = new AssemblyResult(generatedAssembly)
            {
                SafeClassName = className,
                MainType = mainType,
                CacheDependencyId = AssemblyCacheManager.KeyTemplate(codeFileInfo.FullPath)
            };


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

            // Save to disk cache after memory cache
            lTimer = Log.Fn("timer for DiskCache", timer: true);
            diskCacheService.TrySaveToCache(spec, codeFileInfo.RelativePath, preContentHash, preAppCodeHash, assemblyResult);
            lTimer.Done();

            return l.ReturnAsOk(assemblyResult);
        }

        internal (List<string>, AssemblyResult, Assembly) ReferencedAssemblies(CodeFileInfo codeFileInfo, HotBuildSpec spec)
        {
            var lTimer = Log.Fn("Timer AppCodeLoader", timer: true);

            // Initialize the list of referenced assemblies with the default ones
            var referencedAssemblies = referencedAssembliesProvider.Locations(codeFileInfo.RelativePath, spec);

            // Roslyn compiler need reference to location of dll, when dll is not in bin folder
            // get assembly - try to get from cache, otherwise compile
            var (appCodeAssemblyResult, _) = appCodeLoader.Value.GetAppCode(spec);

            // Add the latest assembly to the .net assembly resolver (singleton)
            assemblyResolver.AddAssembly(appCodeAssemblyResult?.Assembly);

            var appCodeAssembly = appCodeAssemblyResult?.Assembly;
            if (appCodeAssembly != null)
            {
                var assemblyLocation = appCodeAssembly.Location;
                referencedAssemblies.Add(assemblyLocation);
                lTimer.A($"Added reference to AppCode assembly: {assemblyLocation}");
            }
            lTimer.Done();
            return (referencedAssemblies, appCodeAssemblyResult, appCodeAssembly);
        }

        internal static Type FindMainType(Assembly generatedAssembly, string className, bool isCshtml, ILog log)
        {
            var l = log.Fn<Type>($"{nameof(className)}: '{className}'; {nameof(isCshtml)}: {isCshtml}", timer: true);
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

        internal static string GetSafeClassName(string templateFullPath)
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
        private (Assembly Assembly, List<CompilerError> Errors) CompileRazor(string sourceCode, List<string> referencedAssemblies, string className, string defaultNamespace, string sourceFileName, string? outputAssemblyPath = null)
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
            var compilerParameters = RoslynCompilerParameters(referencedAssemblies, outputAssemblyPath);
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
            memoryCacheService.Set(CSharpCodeProviderCacheKey, codeProvider, p=> p.SetSlidingExpiration(CSharpCodeProviderCacheMinutes * 60));
            // memoryCacheService.Add(CSharpCodeProviderCacheKey, codeProvider, new CacheItemPolicy { SlidingExpiration = new(0, CSharpCodeProviderCacheMinutes, 0) });

            return l.Return(codeProvider, "created new and added to cache for 1 min");
        }
        private const string CSharpCodeProviderCacheKey = "Sxc-Dnn-CSharpCodeProvider";
        private const int CSharpCodeProviderCacheMinutes = 5;   // basic idea is that at startup, there is usually more to compile, after a while it's not so important any more.


        private CompilerParameters RoslynCompilerParameters(List<string> referencedAssemblies, string? outputAssemblyPath = null)
        {
            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = !diskCacheService.IsEnabled() && string.IsNullOrEmpty(outputAssemblyPath),
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,
                CompilerOptions = DnnRoslynConstants.CompilerOptions,
            };

            if (diskCacheService.IsEnabled() && !string.IsNullOrEmpty(outputAssemblyPath))
            {
                var outDir = Path.GetDirectoryName(outputAssemblyPath);
                if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
                compilerParameters.OutputAssembly = outputAssemblyPath;
                // Prevents pollution of the main cache directory with .cs, .cmdline, .err, .out, .tmp files
                compilerParameters.TempFiles = new TempFileCollection(EnsureTempDir(outDir));
            }

            return compilerParameters;
        }

        /// <summary>
        /// Compiles the C# code into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileCSharpCode(string csharpCode, List<string> referencedAssemblies, string? outputAssemblyPath = null)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"C# code content length: {csharpCode.Length}");

            var lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = RoslynCompilerParameters(referencedAssemblies, outputAssemblyPath);
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
        /// Gets the hash of the AppCode assembly for cache invalidation.
        /// Used by disk cache to detect AppCode changes and invalidate affected template caches.
        /// </summary>
        /// <param name="spec">Hot build specification containing app and edition info</param>
        /// <returns>Hash string representing the AppCode assembly; empty string if no AppCode</returns>
        /// <remarks>
        /// Uses assembly full name (includes version) as hash source.
        /// </remarks>
        public string GetAppCodeHash(HotBuildSpec spec)
        {
            var l = Log.Fn<string>($"{spec}");

            // Get AppCode assembly from cache/compiler
            var (appCodeAssemblyResult, _) = appCodeLoader.Value.GetAppCode(spec);
            var appCodeAssembly = appCodeAssemblyResult?.Assembly;

            if (appCodeAssembly == null)
            {
                l.A("No AppCode assembly - returning empty hash");
                return l.Return(string.Empty, "no-appcode");
            }

            // Use assembly full name as hash source (includes version info)
            var assemblyFullName = appCodeAssembly.FullName ?? string.Empty;
            l.A($"AppCode assembly: {assemblyFullName}");

            // Compute SHA256 hash of assembly full name
            using var sha256 = SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(assemblyFullName);
            var hashBytes = sha256.ComputeHash(bytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            return l.Return(hash, $"hash computed from assembly name");
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

        /// <summary>
        /// Use temp directory for compilation artifacts.
        /// </summary>
        /// <param name="outDir"></param>
        /// <returns></returns>
        private static string EnsureTempDir(string outDir)
        {
            var tempDir = Path.Combine(outDir, "temp");
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
            return tempDir;
        }
    }
}