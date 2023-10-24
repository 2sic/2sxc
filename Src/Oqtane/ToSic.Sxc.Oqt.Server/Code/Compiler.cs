using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Oqt.Server.Code
{

    // Code is based on DynamicRun by Laurent Kempé
    // https://github.com/laurentkempe/DynamicRun
    // https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
    public class Compiler : ServiceBase
    {
        private static List<MetadataReference> References => _references ??= GetMetadataReferences();
        private static List<MetadataReference> _references;

        public Compiler() : base("Sys.CodCpl")
        { }

        public AssemblyResult Compile(string fullPath, string dllName)
        {
            var cache = MemoryCache.Default;
            if (cache[fullPath.ToLowerInvariant()] is AssemblyResult assemblyResult) return assemblyResult;
            assemblyResult = CompileSourceCode(fullPath, dllName);
            if (assemblyResult?.Assembly != null)
                cache.Set(fullPath.ToLowerInvariant(), assemblyResult, GetCacheItemPolicy(fullPath));
            return assemblyResult;
        }

        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]

        private AssemblyResult CompileSourceCode(string path, string dllName)
        {
            var l = Log.Fn<AssemblyResult>($"Starting compilation of: '{path}', {nameof(dllName)}: {dllName}.");

            var encoding = Encoding.UTF8;

            var options = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.CSharp11)
                .WithPreprocessorSymbols("OQTANE", "NETCOREAPP", "NET5_0");

            var syntaxTrees = new List<SyntaxTree>();
            var embeddedTexts = new List<EmbeddedText>();

            foreach (var sourceFile in GetSourceFiles(path))
            {
                var sourceCode = File.ReadAllText(sourceFile);
                syntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(sourceCode, options));
                embeddedTexts.Add(EmbeddedText.FromSource(sourceFile, SourceText.From(sourceCode, encoding)));
            }

            var save = Directory.Exists(path);
            var assemblyName = dllName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? dllName.Substring(0, dllName.Length - 4) : dllName;
            var assemblyFilePath = Path.Combine(path, $"{assemblyName}.dll");
            var pdbFilePath = Path.Combine(path, $"{assemblyName}.pdb");

            var compilation = CSharpCompilation.Create(
                $"{assemblyName}.dll",
                syntaxTrees,
                references: References,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
            // var assemblyLoadContext = AssemblyLoadContext.Default;

            if (save)
            {
                // Create file streams to save the compiled assembly and PDB to disk
                using (var peFileStream = new FileStream(assemblyFilePath, FileMode.Create, FileAccess.Write))
                using (var pdbFileStream = new FileStream(pdbFilePath, FileMode.Create, FileAccess.Write))
                {
                    var result = compilation.Emit(peFileStream, pdbFileStream, embeddedTexts: embeddedTexts,
                        options: new EmitOptions(
                            debugInformationFormat: DebugInformationFormat.PortablePdb,
                            pdbFilePath: pdbFilePath));

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

                    peFileStream.Flush();
                    pdbFileStream.Flush();
                }

                var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblyFilePath);
                var assemblyBytes = File.ReadAllBytes(assemblyFilePath);

                return l.Return(new AssemblyResult(assembly, assemblyBytes, null), "Compilation done without any error.");
            }
            else
            {
                // Compile to in-memory streams as before
                using (var peStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var result = compilation.Emit(peStream, pdbStream, embeddedTexts: embeddedTexts,
                        options: new EmitOptions(
                            debugInformationFormat: DebugInformationFormat.PortablePdb,
                            pdbFilePath: $"{assemblyName}.pdb"));

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

                        throw l.Done(new InvalidOperationException(string.Join("\n", errors)));
                    }

                    peStream.Seek(0, SeekOrigin.Begin);
                    pdbStream?.Seek(0, SeekOrigin.Begin);

                    var assembly = assemblyLoadContext.LoadFromStream(peStream, pdbStream);

                    return l.Return(new AssemblyResult(assembly, peStream.ToArray(), null), "Compilation done without any error.");
                }
            }
        }

        private static IEnumerable<string> GetSourceFiles(string path)
        {
            if (File.Exists(path))
                return new[] { path };
            if (Directory.Exists(path))
                return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            return Array.Empty<string>();
        }

        //private static CSharpCompilation GenerateCode(string path, SourceText sourceCode, string dllName)
        //{
        //    var options = CSharpParseOptions.Default
        //        .WithLanguageVersion(LanguageVersion.CSharp11)
        //        .WithPreprocessorSymbols("OQTANE", "NETCOREAPP", "NET5_0");

        //    var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options, path);
        //    var peName = $"{dllName}.dll";

        //    return CSharpCompilation.Create(peName,
        //        new[] { parsedSyntaxTree },
        //        references: References,
        //        options: new(OutputKind.DynamicallyLinkedLibrary,
        //            optimizationLevel: OptimizationLevel.Debug,
        //            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        //}

        //private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles)
        //{
        //    var provider = new CSharpCodeProvider();
        //    var parameters = new CompilerParameters
        //    {
        //        GenerateInMemory = true,
        //        GenerateExecutable = false, // Dynamically linked library in Roslyn
        //        IncludeDebugInformation = true, // Equivalent to OptimizationLevel.Debug in Roslyn
        //        CompilerOptions = "/langversion:11 /define:OQTANE;NETCOREAPP;NET5_0 /optimize-"
        //    };

        //    // Add all referenced assemblies
        //    parameters.ReferencedAssemblies.AddRange(ReferencedAssembliesProvider.Locations());

        //    return provider.CompileAssemblyFromFile(parameters, sourceFiles);
        //}

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
            // expire cache item if not used in 30 min
            var cacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };

            // expire cache item on file or folder change
            if (File.Exists(filePath))
                cacheItemPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filePath }));
            else if (Directory.Exists(filePath))
                cacheItemPolicy.ChangeMonitors.Add(new FolderChangeMonitor(new List<string> { filePath }));
            return cacheItemPolicy;
        }
    }
}
