using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Razor;

namespace ToSic.Sxc.Oqt.Server.Code.Internal
{

    // Code is based on DynamicRun by Laurent Kempé
    // https://github.com/laurentkempe/DynamicRun
    // https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
    internal class Compiler(LazySvc<AppCodeLoader> appCodeLoader, HotBuildReferenceManager referenceManager)
        : ServiceBase("Sys.CodCpl", connect: [appCodeLoader, referenceManager])
    {
        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal AssemblyResult Compile(string sourceFile, string dllName, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"Starting compilation of: '{sourceFile}'; {nameof(dllName)}: '{dllName}'; {spec}'.");

            var (assemblyResult, _) = appCodeLoader.Value.GetAppCode(spec);

            var encoding = Encoding.UTF8;

            var options = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.Latest)
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
                references: referenceManager.GetMetadataReferences(assemblyResult?.Assembly?.Location, spec),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

            // Compile to in-memory streams
            using var peStream = new MemoryStream();
            using var pdbStream = new MemoryStream();
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

        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal AssemblyResult GetCompiledAssemblyFromFolder(string[] sourceFiles, string assemblyFilePath, string pdbFilePath, string dllName, HotBuildSpec spec)
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
                references: referenceManager.GetMetadataReferences(assemblyFilePath, spec),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

            try
            {
                using var peStream = new MemoryStream();
                using var pdbStream = new MemoryStream();
                var result = compilation.Emit(peStream, pdbStream, embeddedTexts: embeddedTexts,
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

                // Create file streams to save the compiled assembly and PDB to disk
                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream?.Seek(0, SeekOrigin.Begin);

                using (var peFileStream = new FileStream(assemblyFilePath, FileMode.Create, FileAccess.Write))
                    peStream.CopyTo(peFileStream);

                using (var pdbFileStream = new FileStream(pdbFilePath, FileMode.Create, FileAccess.Write)) 
                    pdbStream.CopyTo(pdbFileStream);

                var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblyFilePath);


                return l.ReturnAsOk(new AssemblyResult(assembly));
            }
            catch (Exception ex)
            {
                l.E($"Exception during compilation: {ex.Message}");
                return l.ReturnAsError(new AssemblyResult(errorMessages: ex.Message));
            }
            finally
            {
                // Ensure that can't be kept alive.
                if (assemblyLoadContext.IsCollectible)
                    assemblyLoadContext.Unload();
            }
        }
    }
}
