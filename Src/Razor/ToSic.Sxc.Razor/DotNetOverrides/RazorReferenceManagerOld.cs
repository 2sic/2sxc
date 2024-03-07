//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection.PortableExecutable;
//using System.Threading;
//using Microsoft.AspNetCore.Mvc.ApplicationParts;
//using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
//using Microsoft.CodeAnalysis;
//using Microsoft.Extensions.Options;

//namespace ToSic.Sxc.Razor.DotNetOverrides
//{
//    /// <summary>
//    /// This is a copy of the .net core 3.1 internal RazorReferenceManager
//    /// It's not used in runtime, but we need during development to to verify what's happening
//    /// when dependencies are missing
//    /// </summary>
//    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
//    internal class RazorReferenceManager
//    {
//        private readonly ApplicationPartManager _partManager;
//        private readonly MvcRazorRuntimeCompilationOptions _options;
//        private object _compilationReferencesLock = new();
//        private bool _compilationReferencesInitialized;
//        private IReadOnlyList<MetadataReference> _compilationReferences;

//        public RazorReferenceManager(
//            ApplicationPartManager partManager,
//            IOptions<MvcRazorRuntimeCompilationOptions> options)
//        {
//            _partManager = partManager;
//            _options = options.Value;
//        }

//        public virtual IReadOnlyList<MetadataReference> CompilationReferences =>
//            LazyInitializer.EnsureInitialized(
//                ref _compilationReferences,
//                ref _compilationReferencesInitialized,
//                ref _compilationReferencesLock,
//                GetCompilationReferences);

//        private IReadOnlyList<MetadataReference> GetCompilationReferences()
//        {
//            var referencePaths = GetReferencePaths();

//            return referencePaths
//                .Select(CreateMetadataReference)
//                .ToList();
//        }

//        // For unit testing
//        internal IEnumerable<string> GetReferencePaths()
//        {
//            var referencePaths = new List<string>(_options.AdditionalReferencePaths.Count);

//            foreach (var part in _partManager.ApplicationParts)
//                if (part is ICompilationReferencesProvider compilationReferenceProvider)
//                {
//                    referencePaths.AddRange(compilationReferenceProvider.GetReferencePaths());
//                }
//                else if (part is AssemblyPart assemblyPart)
//                {
//                    referencePaths.AddRange(assemblyPart.GetReferencePaths());
//                }

//            referencePaths.AddRange(_options.AdditionalReferencePaths);

//            return referencePaths;
//        }

//        private static MetadataReference CreateMetadataReference(string path)
//        {
//            using var stream = File.OpenRead(path);
//            var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
//            var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);

//            return assemblyMetadata.GetReference(filePath: path);
//        }
//    }
//}
