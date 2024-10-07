// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// based on: https://github.dev/dotnet/aspnetcore/tree/v8.0.5
// src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Razor.DotNetOverrides;

#pragma warning disable CA1852 // Seal internal types
internal partial class RuntimeViewCompiler : ServiceBase, IViewCompiler, ILogShouldNeverConnect
#pragma warning restore CA1852 // Seal internal types
{
    private readonly object _cacheLock = new object();
    private readonly Dictionary<string, CompiledViewDescriptor> _precompiledViews;
    private readonly ConcurrentDictionary<string, string> _normalizedPathCache;
    private readonly IFileProvider _fileProvider;
    private readonly RazorProjectEngine _projectEngine;
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private readonly AssemblyResolver _assemblyResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SourceAnalyzer _sourceAnalyzer;
    private readonly IWebHostEnvironment _env;
    private readonly CSharpCompiler _csharpCompiler;

#if DEBUG
    private const bool Dbg = true;
#else
    private const bool Dbg = false;
#endif

    public RuntimeViewCompiler(
        IFileProvider fileProvider,
        RazorProjectEngine projectEngine,
        CSharpCompiler csharpCompiler,
        IList<CompiledViewDescriptor> precompiledViews,
        ILogger logger,
        AssemblyResolver assemblyResolver,
        IHttpContextAccessor httpContextAccessor,
        SourceAnalyzer sourceAnalyzer,
        IWebHostEnvironment env,
        ILogStore logStore) 
        : base($"{SxcLogging.SxcLogName}.RzrViewCmp", connect: [assemblyResolver, sourceAnalyzer])
    {
        var l = Dbg ? base.Log.Fn($"{nameof(precompiledViews)}:{precompiledViews?.Count}") : null;

        if (Dbg)
            logStore.Add(SxcLogging.SxcLogAppCodeLoader, base.Log);

        ArgumentNullException.ThrowIfNull(fileProvider);
        ArgumentNullException.ThrowIfNull(projectEngine);
        ArgumentNullException.ThrowIfNull(csharpCompiler);
        ArgumentNullException.ThrowIfNull(precompiledViews);
        ArgumentNullException.ThrowIfNull(logger);

        _fileProvider = fileProvider;
        _projectEngine = projectEngine;
        _csharpCompiler = csharpCompiler;
        _logger = logger;
        _assemblyResolver = assemblyResolver;
        _httpContextAccessor = httpContextAccessor;
        _sourceAnalyzer = sourceAnalyzer;
        _env = env;

        _normalizedPathCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        // This is our L0 cache, and is a durable store. Views migrate into the cache as they are requested
        // from either the set of known precompiled views, or by being compiled.
        _cache = new MemoryCache(new MemoryCacheOptions());

        // We need to validate that the all of the precompiled views are unique by path (case-insensitive).
        // We do this because there's no good way to canonicalize paths on windows, and it will create
        // problems when deploying to linux. Rather than deal with these issues, we just don't support
        // views that differ only by case.
        _precompiledViews = new Dictionary<string, CompiledViewDescriptor>(
            precompiledViews.Count,
            StringComparer.OrdinalIgnoreCase);

        foreach (var precompiledView in precompiledViews)
        {
            Log.ViewCompilerLocatedCompiledView(_logger, precompiledView.RelativePath);
            l.A($"{precompiledView.RelativePath}:'{precompiledView.RelativePath}'");

            if (!_precompiledViews.ContainsKey(precompiledView.RelativePath))
            {
                // View ordering has precedence semantics, a view with a higher precedence was
                // already added to the list.
                _precompiledViews.Add(precompiledView.RelativePath, precompiledView);
                l.A($"add precompiledView:'{precompiledView.RelativePath}'");
            }
        }

        if (_precompiledViews.Count == 0)
        {
            Log.ViewCompilerNoCompiledViewsFound(_logger);
            l.A($"no compiled views found");
        }

        l.Done();
    }

    public Task<CompiledViewDescriptor> CompileAsync(string relativePath)
    {
        var l = Dbg ? base.Log.Fn<Task<CompiledViewDescriptor>>($"{nameof(relativePath)}:'{relativePath}'") : null;
        
        ArgumentNullException.ThrowIfNull(relativePath);

        // Attempt to lookup the cache entry using the passed in path. This will succeed if the path is already
        // normalized and a cache entry exists.
        if (_cache.TryGetValue<Task<CompiledViewDescriptor>>(relativePath, out var cachedResult) && cachedResult is not null)
        {
            return l.Return(cachedResult, $"OK, got cache result for {nameof(relativePath)}:'{relativePath}'");
        }
        l.A($"NO cache result for {nameof(relativePath)}:'{relativePath}'");

        var normalizedPath = GetNormalizedPath(relativePath);
        l.A($"{nameof(normalizedPath)}:'{normalizedPath}'");
        if (_cache.TryGetValue(normalizedPath, out cachedResult) && cachedResult is not null)
        {
            return l.Return(cachedResult, $"OK, got cache result for {nameof(normalizedPath)}:'{normalizedPath}'");
        }
        l.A($"NO cache result for {nameof(normalizedPath)}:'{normalizedPath}'");

        // Entry does not exist. Attempt to create one.
        l.A($"Entry does not exist. Attempt to create one for {nameof(normalizedPath)}:'{normalizedPath}'");
        cachedResult = OnCacheMiss(normalizedPath);
        return l.ReturnAsOk(cachedResult);
    }

    private Task<CompiledViewDescriptor> OnCacheMiss(string normalizedPath)
    {
        ViewCompilerWorkItem item;
        TaskCompletionSource<CompiledViewDescriptor> taskSource;
        MemoryCacheEntryOptions cacheEntryOptions;

        // Safe races cannot be allowed when compiling Razor pages. To ensure only one compilation request succeeds
        // per file, we'll lock the creation of a cache entry. Creating the cache entry should be very quick. The
        // actual work for compiling files happens outside the critical section.
        lock (_cacheLock)
        {
            // Double-checked locking to handle a possible race.
            if (_cache.TryGetValue<Task<CompiledViewDescriptor>>(normalizedPath, out var result) && result is not null)
            {
                return result;
            }

            if (_precompiledViews.TryGetValue(normalizedPath, out var precompiledView))
            {
                Log.ViewCompilerLocatedCompiledViewForPath(_logger, normalizedPath);
                item = CreatePrecompiledWorkItem(normalizedPath, precompiledView);
            }
            else
            {
                item = CreateRuntimeCompilationWorkItem(normalizedPath);
            }

            // At this point, we've decided what to do - but we should create the cache entry and
            // release the lock first.
            cacheEntryOptions = new MemoryCacheEntryOptions();

            Debug.Assert(item.ExpirationTokens != null);
            for (var i = 0; i < item.ExpirationTokens.Count; i++)
            {
                cacheEntryOptions.ExpirationTokens.Add(item.ExpirationTokens[i]);
            }

            taskSource = new TaskCompletionSource<CompiledViewDescriptor>(creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);
            if (item.SupportsCompilation)
            {
                // We'll compile in just a sec, be patient.
            }
            else
            {
                // If we can't compile, we should have already created the descriptor
                Debug.Assert(item.Descriptor != null);
                taskSource.SetResult(item.Descriptor);
            }

            _cache.Set(normalizedPath, taskSource.Task, cacheEntryOptions);
        }

        // Now the lock has been released so we can do more expensive processing.
        if (item.SupportsCompilation)
        {
            Debug.Assert(taskSource != null);

            if (item.Descriptor?.Item != null &&
                ChecksumValidator.IsItemValid(_projectEngine.FileSystem, item.Descriptor.Item))
            {
                // If the item has checksums to validate, we should also have a precompiled view.
                Debug.Assert(item.Descriptor != null);

                taskSource.SetResult(item.Descriptor);
                return taskSource.Task;
            }

            Log.ViewCompilerInvalidatingCompiledFile(_logger, item.NormalizedPath);
            try
            {
                var descriptor = CompileAndEmit(normalizedPath);
                descriptor.ExpirationTokens = cacheEntryOptions.ExpirationTokens;
                taskSource.SetResult(descriptor);
            }
            catch (Exception ex)
            {
                taskSource.SetException(ex);
            }
        }

        return taskSource.Task;
    }

    private ViewCompilerWorkItem CreatePrecompiledWorkItem(string normalizedPath, CompiledViewDescriptor precompiledView)
    {
        // We have a precompiled view - but we're not sure that we can use it yet.
        //
        // We need to determine first if we have enough information to 'recompile' this view. If that's the case
        // we'll create change tokens for all of the files.
        //
        // Then we'll attempt to validate if any of those files have different content than the original sources
        // based on checksums.
        if (precompiledView.Item == null || !ChecksumValidator.IsRecompilationSupported(precompiledView.Item))
        {
            return new ViewCompilerWorkItem()
            {
                // If we don't have a checksum for the primary source file we can't recompile.
                SupportsCompilation = false,

                ExpirationTokens = Array.Empty<IChangeToken>(), // Never expire because we can't recompile.
                Descriptor = precompiledView, // This will be used as-is.
            };
        }

        var item = new ViewCompilerWorkItem()
        {
            SupportsCompilation = true,

            Descriptor = precompiledView, // This might be used, if the checksums match.

            // Used to validate and recompile
            NormalizedPath = normalizedPath,

            ExpirationTokens = GetExpirationTokens(precompiledView),
        };

        // We also need to create a new descriptor, because the original one doesn't have expiration tokens on
        // it. These will be used by the view location cache, which is like an L1 cache for views (this class is
        // the L2 cache).
        item.Descriptor = new CompiledViewDescriptor()
        {
            ExpirationTokens = item.ExpirationTokens,
            Item = precompiledView.Item,
            RelativePath = precompiledView.RelativePath,
        };

        return item;
    }

    private ViewCompilerWorkItem CreateRuntimeCompilationWorkItem(string normalizedPath)
    {
        IList<IChangeToken> expirationTokens = new List<IChangeToken>
            {
                _fileProvider.Watch(normalizedPath),
            };

        var projectItem = _projectEngine.FileSystem.GetItem(normalizedPath, fileKind: null);
        if (!projectItem.Exists)
        {
            Log.ViewCompilerCouldNotFindFileAtPath(_logger, normalizedPath);

            // If the file doesn't exist, we can't do compilation right now - we still want to cache
            // the fact that we tried. This will allow us to re-trigger compilation if the view file
            // is added.
            return new ViewCompilerWorkItem()
            {
                // We don't have enough information to compile
                SupportsCompilation = false,

                Descriptor = new CompiledViewDescriptor()
                {
                    RelativePath = normalizedPath,
                    ExpirationTokens = expirationTokens,
                },

                // We can try again if the file gets created.
                ExpirationTokens = expirationTokens,
            };
        }

        Log.ViewCompilerFoundFileToCompile(_logger, normalizedPath);

        GetChangeTokensFromImports(expirationTokens, projectItem);

        GetChangeTokenFromAppCode(expirationTokens, normalizedPath);

        return new ViewCompilerWorkItem()
        {
            SupportsCompilation = true,

            NormalizedPath = normalizedPath,
            ExpirationTokens = expirationTokens,
        };
    }

    private IList<IChangeToken> GetExpirationTokens(CompiledViewDescriptor precompiledView)
    {
        var checksums = precompiledView.Item.GetChecksumMetadata();
        var expirationTokens = new List<IChangeToken>(checksums.Count);

        for (var i = 0; i < checksums.Count; i++)
        {
            // We rely on Razor to provide the right set of checksums. Trust the compiler, it has to do a good job,
            // so it probably will.
            expirationTokens.Add(_fileProvider.Watch(checksums[i].Identifier));
        }

        return expirationTokens;
    }

    private void GetChangeTokensFromImports(IList<IChangeToken> expirationTokens, RazorProjectItem projectItem)
    {
        // OK this means we can do compilation. For now let's just identify the other files we need to watch
        // so we can create the cache entry. Compilation will happen after we release the lock.
        var importFeature = _projectEngine.ProjectFeatures.OfType<IImportProjectFeature>().ToArray();
        foreach (var feature in importFeature)
        {
            foreach (var file in feature.GetImports(projectItem))
            {
                if (file.FilePath != null)
                {
                    expirationTokens.Add(_fileProvider.Watch(file.FilePath));
                }
            }
        }
    }

    private void GetChangeTokenFromAppCode(IList<IChangeToken> expirationTokens, string relativePath)
    {
        // find do we require dependency on AppCode in razor view?
        if (!_sourceAnalyzer.TypeOfVirtualPath(relativePath).IsHotBuildSupported()) return;

        // if AppCode folder exists, related to relativePath, watch it
        var appCodeRelativePath = AppCodeRelativePathIfExists(relativePath.Backslash());
        if (appCodeRelativePath != null)
            expirationTokens.Add(_fileProvider.Watch($"{appCodeRelativePath.ForwardSlash().PrefixSlash().SuffixSlash()}**/*.cs"));
    }

    private string AppCodeRelativePathIfExists(string normalizedPath)
    {
        var l = Dbg ? base.Log.Fn<string>($"{nameof(normalizedPath)}:'{normalizedPath}'") : null;

        var (appRelativePath, edition) = GetSxcAppRelativePathWithEdition(normalizedPath);
        l.A($"{nameof(appRelativePath)}:'{appRelativePath}'; {nameof(edition)}:'{edition}'");

        if (!appRelativePath.HasValue())
            return l.ReturnEmpty($"{nameof(appRelativePath)} is empty");

        if (edition.HasValue() && Directory.Exists(Path.Combine(_env.ContentRootPath, appRelativePath.Backslash(), edition, Constants.AppCode)))
            return l.ReturnAndLog(Path.Combine(appRelativePath.Backslash(), edition, Constants.AppCode));

        return l.ReturnAndLog((Directory.Exists(Path.Combine(_env.ContentRootPath, appRelativePath.Backslash(), Constants.AppCode))
            ? Path.Combine(appRelativePath.Backslash(), Constants.AppCode)
            : ""));
    }

    protected virtual CompiledViewDescriptor CompileAndEmit(string relativePath)
    {
        var projectItem = _projectEngine.FileSystem.GetItem(relativePath, fileKind: null);
        var codeDocument = _projectEngine.Process(projectItem);
        var cSharpDocument = codeDocument.GetCSharpDocument();

        if (cSharpDocument.Diagnostics.Count > 0)
        {
            throw CompilationFailedExceptionFactory.Create(
                codeDocument,
                cSharpDocument.Diagnostics);
        }

        var assembly = CompileAndEmit(codeDocument, cSharpDocument.GeneratedCode, GetMetadataReferences(relativePath));

        // Anything we compile from source will use Razor 2.1 and so should have the new metadata.
        var loader = new RazorCompiledItemLoader();
        var item = loader.LoadItems(assembly).Single();
        return new CompiledViewDescriptor(item);
    }

    /// <summary>
    /// get MetadataReferences for relativePath
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    private IEnumerable<MetadataReference> GetMetadataReferences(string relativePath)
    {
        IEnumerable<MetadataReference> references = new List<MetadataReference>();
        var appCodePath = GetAppCodeDllPath(relativePath);
        if (appCodePath != null)
            references = references.Append(MetadataReference.CreateFromFile(appCodePath));
        return references;
    }

    private string GetAppCodeDllPath(string relativePath)
    {
        var (appRelativePath, edition) = GetSxcAppRelativePathWithEdition(relativePath);
        if (appRelativePath == null) return null;
        if (edition.HasValue()) appRelativePath = Path.Combine(appRelativePath, edition);
        appRelativePath = appRelativePath.Backslash();
        var appCodeDllPath = _assemblyResolver.GetAssemblyLocation(appRelativePath);
        return appCodeDllPath;
    }

    private (string appRelativePath, string edition) GetSxcAppRelativePathWithEdition(string relativePath)
    {
        if (_httpContextAccessor?.HttpContext == null)
            return GetSxcAppRelativePathWithEditionFallback(relativePath);
        
        var sp = _httpContextAccessor.HttpContext.RequestServices;
        var ctxResolver = sp.GetService<ISxcContextResolver>();

        var block = ctxResolver?.BlockOrNull();
        if (block == null)
            return GetSxcAppRelativePathWithEditionFallback(relativePath);

        // appRelativePath from block (this is not working for inner-content)
        var appRelativePath = block.App.RelativePath;

        // Inner-content app case
        if (!IsTemplateLocatedInAppFolder(appRelativePath, relativePath))
            return GetSxcAppRelativePathWithEditionFallback(relativePath);

        // Standard case (appRelativePath and edition from block)
        var edition = sp.GetService<PolymorphConfigReader>().UseViewEditionOrGet(block);
        return (appRelativePath, edition);
    }

    /// <summary>
    /// Validate that relative path of template is not located in relative app path,
    /// indicates that razor template is not in app, so it likely in inner app.
    /// </summary>
    /// <param name="appRelativePath">app relative path</param>
    /// <param name="relativePath">template relative path</param>
    /// <returns></returns>
    private static bool IsTemplateLocatedInAppFolder(string appRelativePath, string relativePath) 
        => relativePath.ForwardSlash().TrimPrefixSlash().StartsWith(appRelativePath.ForwardSlash());

    /// <summary>
    /// extract appRelativePathWithEdition from relativePath
    /// fallback, when block is null
    /// </summary>
    /// <param name="relativePath">string "/2sxc/n/aaa-folder-name/edition/etc..."</param>
    /// <returns>string "2sxc\\n\\aaa-folder-name\\edition" or null</returns>
    private (string appRelativePath, string edition) GetSxcAppRelativePathWithEditionFallback(string relativePath)
    {
        var l = Dbg ? base.Log.Fn<(string appRelativePath, string edition)>($"{nameof(relativePath)}:'{relativePath}'") : null;

        relativePath = relativePath?.ForwardSlash();

        if ((string.IsNullOrEmpty(relativePath))
            || (relativePath.Length < 8)
            || (relativePath[0] != '/')
            || (relativePath[5] != '/'))
            //throw new($"relativePath:'{relativePath}' is not in format '/2sxc/n/app-folder-name/etc...'");
            return l.ReturnAsError((appRelativePath: null, edition: null));

        // find position of 4th slash in relativePath 
        var posApp = 6; // skipping first 2 slashes
        for (var i = 0; i < 2; i++)
        {
            posApp = relativePath.IndexOf('/', posApp + 1);
            if (posApp < 0)
                //throw new($"relativePath:'{relativePath}' is not in format '/2sxc/n/app-folder-name/etc...'");
                return l.ReturnAsError((appRelativePath: null, edition: null));
        }
        var appRelativePath = relativePath.Substring(1, posApp - 1);

        if (relativePath.Length <= posApp)
            return l.ReturnAsOk((appRelativePath, edition: ""));

        // find optional "edition" subfolder
        var edition = "";

        // next subfolder is probably optional "edition" subfolder
        var posEdition = relativePath.IndexOf('/', posApp + 1);

        // can't find "edition" subfolder, so return app folder without edition
        if (posEdition >= 0)
            edition = relativePath.Substring(posApp + 1, posEdition - posApp -1);

        return l.ReturnAsOk((appRelativePath, edition));
    }

    internal Assembly CompileAndEmit(RazorCodeDocument codeDocument, string generatedCode, IEnumerable<MetadataReference> references)
    {
        Log.GeneratedCodeToAssemblyCompilationStart(_logger, codeDocument.Source.FilePath);

        var startTimestamp = _logger.IsEnabled(LogLevel.Debug) ? Stopwatch.GetTimestamp() : 0;

        var assemblyName = Path.GetRandomFileName();
        var compilation = CreateCompilation(generatedCode, assemblyName, references);

        var emitOptions = _csharpCompiler.EmitOptions;
        var emitPdbFile = _csharpCompiler.EmitPdb && emitOptions.DebugInformationFormat != DebugInformationFormat.Embedded;

        using (var assemblyStream = new MemoryStream())
        using (var pdbStream = emitPdbFile ? new MemoryStream() : null)
        {
            var result = compilation.Emit(
                assemblyStream,
                pdbStream,
                options: emitOptions);

            if (!result.Success)
            {
                throw CompilationFailedExceptionFactory.Create(
                    codeDocument,
                    generatedCode,
                    assemblyName,
                    result.Diagnostics);
            }

            assemblyStream.Seek(0, SeekOrigin.Begin);
            pdbStream?.Seek(0, SeekOrigin.Begin);

            var assembly = Assembly.Load(assemblyStream.ToArray(), pdbStream?.ToArray());
            Log.GeneratedCodeToAssemblyCompilationEnd(_logger, codeDocument.Source.FilePath, startTimestamp);

            return assembly;
        }
    }

    private CSharpCompilation CreateCompilation(string compilationContent, string assemblyName, IEnumerable<MetadataReference> references)
    {
        var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
        var syntaxTree = _csharpCompiler.CreateSyntaxTree(sourceText).WithFilePath(assemblyName);
        return _csharpCompiler
            .CreateCompilation(assemblyName)
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(references);
    }

    private string GetNormalizedPath(string relativePath)
    {
        Debug.Assert(relativePath != null);
        if (relativePath.Length == 0)
        {
            return relativePath;
        }

        if (!_normalizedPathCache.TryGetValue(relativePath, out var normalizedPath))
        {
            normalizedPath = ViewPath.NormalizePath(relativePath);
            _normalizedPathCache[relativePath] = normalizedPath;
        }

        return normalizedPath;
    }

    private class ViewCompilerWorkItem
    {
        public bool SupportsCompilation { get; set; } = default!;

        public string NormalizedPath { get; set; } = default!;

        public IList<IChangeToken> ExpirationTokens { get; set; } = default!;

        public CompiledViewDescriptor Descriptor { get; set; } = default!;
    }

    private static partial class Log
    {
        [LoggerMessage(1, LogLevel.Debug, "Compilation of the generated code for the Razor file at '{FilePath}' started.")]
        public static partial void GeneratedCodeToAssemblyCompilationStart(ILogger logger, string filePath);

        [LoggerMessage(2, LogLevel.Debug, "Compilation of the generated code for the Razor file at '{FilePath}' completed in {ElapsedMilliseconds}ms.")]
        private static partial void GeneratedCodeToAssemblyCompilationEnd(ILogger logger, string filePath, double elapsedMilliseconds);

        public static void GeneratedCodeToAssemblyCompilationEnd(ILogger logger, string filePath, long startTimestamp)
        {
            // Don't log if logging wasn't enabled at start of request as time will be wildly wrong.
            if (startTimestamp != 0)
            {
                var currentTimestamp = Stopwatch.GetTimestamp();
                var elapsed = Stopwatch.GetElapsedTime(startTimestamp, currentTimestamp);
                GeneratedCodeToAssemblyCompilationEnd(logger, filePath, elapsed.TotalMilliseconds);
            }
        }

        [LoggerMessage(3, LogLevel.Debug, "Initializing Razor view compiler with compiled view: '{ViewName}'.")]
        public static partial void ViewCompilerLocatedCompiledView(ILogger logger, string viewName);

        [LoggerMessage(4, LogLevel.Debug, "Initializing Razor view compiler with no compiled views.")]
        public static partial void ViewCompilerNoCompiledViewsFound(ILogger logger);

        [LoggerMessage(5, LogLevel.Trace, "Located compiled view for view at path '{Path}'.")]
        public static partial void ViewCompilerLocatedCompiledViewForPath(ILogger logger, string path);

        [LoggerMessage(6, LogLevel.Trace, "Invalidating compiled view for view at path '{Path}'.")]
        public static partial void ViewCompilerRecompilingCompiledView(ILogger logger, string path);

        [LoggerMessage(7, LogLevel.Trace, "Could not find a file for view at path '{Path}'.")]
        public static partial void ViewCompilerCouldNotFindFileAtPath(ILogger logger, string path);

        [LoggerMessage(8, LogLevel.Trace, "Found file at path '{Path}'.")]
        public static partial void ViewCompilerFoundFileToCompile(ILogger logger, string path);

        [LoggerMessage(9, LogLevel.Trace, "Invalidating compiled view at path '{Path}' with a file since the checksum did not match.")]
        public static partial void ViewCompilerInvalidatingCompiledFile(ILogger logger, string path);
    }
}
