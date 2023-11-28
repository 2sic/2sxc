using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Oqt.Server.Code;

// Code is based on DynamicRun by Laurent Kempé
// https://github.com/laurentkempe/DynamicRun
// https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
public class Compiler : ServiceBase
{
    private static List<MetadataReference> References => _references ??= GetMetadataReferences();
    private static List<MetadataReference> _references;

    public Compiler() : base("Sys.CodCpl")
    {

    }

    public Assembly Compile(string filePath, string dllName)
    {
        Log.A($"Starting compilation of: '{filePath}'");

        var sourceCode = File.ReadAllText(filePath);

        var cache = MemoryCache.Default;
        var assembly = cache[filePath.ToLowerInvariant()] as Assembly;
        if (assembly != null) return assembly;
        assembly = CompileSourceCode(filePath, sourceCode, dllName);
        if (assembly != null)
            cache.Set(filePath.ToLowerInvariant(), assembly, GetCacheItemPolicy(filePath));
        return assembly;
    }

    // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
    // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
    [MethodImpl(MethodImplOptions.NoInlining)]

    private Assembly CompileSourceCode(string path, string sourceCode, string dllName)
    {
        var l = Log.Fn<Assembly>($"{nameof(dllName)}: {dllName}.");
        var encoding = Encoding.UTF8;
        var pdbName = $"{dllName}.pdb";
        using var peStream = new MemoryStream();
        using var pdbStream = new MemoryStream();
        var options = new EmitOptions(
            debugInformationFormat: DebugInformationFormat.PortablePdb,
            pdbFilePath: pdbName);

        var buffer = encoding.GetBytes(sourceCode);
        var sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);

        var embeddedTexts = new List<EmbeddedText>
        {
            EmbeddedText.FromSource(path, sourceText),
        };

        var result = GenerateCode(path, sourceText, dllName).Emit(peStream,
            pdbStream,
            embeddedTexts: embeddedTexts,
            options: options);

        if (!result.Success)
        {
            l.E("Compilation done with error.");

            var errors = new List<string>();

            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                l.A("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
            }

            throw l.Done(new IOException(string.Join("\n", errors)));
        }

        peStream.Seek(0, SeekOrigin.Begin);
        pdbStream?.Seek(0, SeekOrigin.Begin);

        var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
        var assembly = assemblyLoadContext.LoadFromStream(peStream, pdbStream);

        return l.Return(assembly, "Compilation done without any error.");
    }

    private static CSharpCompilation GenerateCode(string path, SourceText sourceCode, string dllName)
    {
        var options = CSharpParseOptions.Default
            .WithLanguageVersion(LanguageVersion.CSharp9)
            .WithPreprocessorSymbols("OQTANE", "NETCOREAPP", "NET5_0");

        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options, path);
        var peName = $"{dllName}.dll";

        return CSharpCompilation.Create(peName,
            new[] { parsedSyntaxTree },
            references: References,
            options: new(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Debug,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }

    private static List<MetadataReference> GetMetadataReferences()
    {
        var references = new List<MetadataReference>
        {
            //MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // Commented because it solves error when "refs" are referenced.
            MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)
        };

        Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
            .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

        // Add references to all dll's in bin folder.
        var dllLocation = AppContext.BaseDirectory;
        var dllPath = Path.GetDirectoryName(dllLocation);
        foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll"))
            references.Add(MetadataReference.CreateFromFile(dllFile));
        foreach (string dllFile in Directory.GetFiles(Path.Combine(dllPath, "refs"), "*.dll"))
            references.Add(MetadataReference.CreateFromFile(dllFile));
        return references;
    }

    private CacheItemPolicy GetCacheItemPolicy(string filePath)
    {
        var filePaths = new List<string> { filePath };

        // expire cache item if not used in 30 min
        var cacheItemPolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };
        // expire cache item on CS file change
        cacheItemPolicy.ChangeMonitors.Add(new
            HostFileChangeMonitor(filePaths));
        return cacheItemPolicy;
    }
}