// #define NETSTANDARD
#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Code.Builder
{

    // Code is based on DynamicRun by Laurent Kempé
    // https://github.com/laurentkempe/DynamicRun
    // https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
    public class Compiler : HasLog
    {
        public Compiler() : base("Sys.DynamicRun.Cmpl")
        {

        }
        public byte[] Compile(string filepath, string className)
        {
            Log.Add($"Starting compilation of: '{filepath}'");

            var sourceCode = File.ReadAllText(filepath);

            return CompileSourceCode(sourceCode, className);
        }

        public byte[] CompileSourceCode(string sourceCode, string className)
        {
            var wrapLog = Log.Call($"Source code compilation: {className}.");
            using (var peStream = new MemoryStream())
            {
                var result = GenerateCode(sourceCode, className).Emit(peStream);

                if (!result.Success)
                {
                    wrapLog("Compilation done with error.");

                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Log.Add("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }

                wrapLog("Compilation done without any error.");

                peStream.Seek(0, SeekOrigin.Begin);

                return peStream.ToArray();
            }
        }

        public static CSharpCompilation GenerateCode(string sourceCode, string className)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)
            };

            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            // Add references to all dll's in bin folder.
            var dllLocation = AppContext.BaseDirectory;
            var dllPath = Path.GetDirectoryName(dllLocation);
            foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll"))
                references.Add(MetadataReference.CreateFromFile(dllFile));

            return CSharpCompilation.Create($"{className}.dll",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
#endif
