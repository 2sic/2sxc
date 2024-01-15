using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Code.Internal
{

    // Code is based on DynamicRun by Laurent Kempé
    // https://github.com/laurentkempe/DynamicRun
    // https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
    public class Compiler : ServiceBase
    {
        private readonly LazySvc<ThisAppCodeLoader> _thisAppCodeLoader;

        public Compiler(LazySvc<ThisAppCodeLoader> thisAppCodeLoader) : base("Sys.CodCpl")
        {
            _thisAppCodeLoader = thisAppCodeLoader;
        }

        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal AssemblyResult Compile(string sourceFile, string dllName, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"Starting compilation of: '{sourceFile}'; {nameof(dllName)}: '{dllName}'; {spec}'.");

            //var codeAssembly = ThisAppCodeLoader.TryGetAssemblyOfCodeFromCache(spec, Log)?.Assembly
            //                   ?? _thisAppCodeLoader.Value.GetAppCodeAssemblyOrThrow(spec);
            var (codeAssembly, _) = _thisAppCodeLoader.Value.TryGetOrFallback(spec);

            var encoding = Encoding.UTF8;

            var options = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.CSharp11)
                .WithPreprocessorSymbols("OQTANE", "NETCOREAPP", "NET5_0");

            var syntaxTrees = new List<SyntaxTree>();
            var embeddedTexts = new List<EmbeddedText>();

            var sourceCode = File.ReadAllText(sourceFile);
            syntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(sourceCode, options));
            embeddedTexts.Add(EmbeddedText.FromSource(sourceFile, SourceText.From(sourceCode, encoding)));

            var assemblyName = dllName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? dllName.Substring(0, dllName.Length - 4) : dllName;

            var compilation = CSharpCompilation.Create(
                $"{assemblyName}.dll",
                syntaxTrees,
                references: GetMetadataReferences(codeAssembly?.Location),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

            // Compile to in-memory streams
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

                    //throw l.Done(new InvalidOperationException(string.Join("\n", errors)));
                    return l.ReturnAsError(new AssemblyResult(errorMessages: string.Join("\n", errors)));
                }

                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream?.Seek(0, SeekOrigin.Begin);

                var assembly = assemblyLoadContext.LoadFromStream(peStream, pdbStream);

                return l.ReturnAsOk(new AssemblyResult(assembly));
            }
        }

        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal AssemblyResult GetCompiledAssemblyFromFolder(string[] sourceFiles, string assemblyFilePath, string pdbFilePath, string dllName)
        {
            var l = Log.Fn<AssemblyResult>($"{nameof(sourceFiles)}: {sourceFiles.Length}; {nameof(assemblyFilePath)}: '{assemblyFilePath}'", timer: true);

            var encoding = Encoding.UTF8;

            var options = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.Latest)
                .WithPreprocessorSymbols("OQTANE", "NETCOREAPP", "NET5_0");

            var syntaxTrees = new List<SyntaxTree>();
            var embeddedTexts = new List<EmbeddedText>();

            foreach (var sourceFile in sourceFiles)
            {
                var sourceCode = File.ReadAllText(sourceFile);
                syntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(sourceCode, options));
                embeddedTexts.Add(EmbeddedText.FromSource(sourceFile, SourceText.From(sourceCode, encoding)));
            }

            var compilation = CSharpCompilation.Create(
                dllName,
                syntaxTrees,
                references: References,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

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

                    //throw l.Done(new IOException(string.Join("\n", errors)));
                    return l.ReturnAsError(new AssemblyResult(errorMessages: string.Join("\n", errors)));
                }

                peFileStream.Flush();
                pdbFileStream.Flush();
            }

            var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblyFilePath);
            //var assemblyBytes = File.ReadAllBytes(assemblyFilePath);

            return l.ReturnAsOk(new AssemblyResult(assembly, /*assemblyBytes,*/ null));
        }

        #region References
        private static List<MetadataReference> GetMetadataReferences(string appCodeFullPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(appCodeFullPath) && File.Exists(appCodeFullPath))
                    return References.Union(new List<MetadataReference>()
                            { MetadataReference.CreateFromFile(appCodeFullPath) })
                        .ToList();
            }
            catch
            {
                // sink
            };

            return References;
        }

        private static List<MetadataReference> References => _references ??= GetMetadataReferences();
        private static List<MetadataReference> _references;

        private static List<MetadataReference> GetMetadataReferences()
        {
            var references = new List<MetadataReference>();
            //AddMetadataReferenceFromFile(references, typeof(object).Assembly.Location); // Commented because it solves error when "refs" are referenced.
            AddMetadataReferenceFromAssemblyName(references, "netstandard, Version=2.0.0.0");
            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(assemblyName => AddMetadataReferenceFromAssemblyName(references, assemblyName));

            // Add references to all dll's in bin folder.
            var dllLocation = AppContext.BaseDirectory;
            var dllPath = Path.GetDirectoryName(dllLocation);
            foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll")) AddMetadataReferenceFromFile(references, dllFile);
            foreach (string dllFile in Directory.GetFiles(Path.Combine(dllPath, "refs"), "*.dll")) AddMetadataReferenceFromFile(references, dllFile);

            return references;
        }

        private static void AddMetadataReferenceFromAssemblyName(List<MetadataReference> references, string assemblyName)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location));
            }
            catch
            {
                // sink
            }
        }

        private static void AddMetadataReferenceFromAssemblyName(List<MetadataReference> references, AssemblyName assemblyName)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location));
            }
            catch
            {
                // sink
            }
        }

        private static void AddMetadataReferenceFromFile(List<MetadataReference> references, string dllFile)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(dllFile));
            }
            catch
            {
                // sink
            }
        }
        #endregion
    }
}
