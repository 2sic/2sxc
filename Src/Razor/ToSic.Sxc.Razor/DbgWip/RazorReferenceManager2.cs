// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace ToSic.Sxc.Razor.DbgWip
{
    internal class RazorReferenceManager
    {
        private readonly ApplicationPartManager _partManager;
        private readonly MvcRazorRuntimeCompilationOptions _options;
        private object _compilationReferencesLock = new object();
        private bool _compilationReferencesInitialized;
        private IReadOnlyList<MetadataReference>? _compilationReferences;

        public RazorReferenceManager(
            ApplicationPartManager partManager,
            IOptions<MvcRazorRuntimeCompilationOptions> options)
        {
            _partManager = partManager;
            _options = options.Value;
            //_options.AdditionalReferencePaths.Add(@"A:\2sxc\oqtane\oqtane.framework\Oqtane.Server\App_Data\2sxc.bin\App-0389.dll");
        }

        public virtual IReadOnlyList<MetadataReference> CompilationReferences
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _compilationReferences,
                    ref _compilationReferencesInitialized,
                    ref _compilationReferencesLock,
                    GetCompilationReferences)!;
            }
        }

        private IReadOnlyList<MetadataReference> GetCompilationReferences()
        {
            var referencePaths = GetReferencePaths();
            var md = referencePaths
                .Select(CreateMetadataReference)
                .ToList();
            //md.Add(MetadataReference.CreateFromFile(@"A:\2sxc\oqtane\oqtane.framework\Oqtane.Server\App_Data\2sxc.bin\App-0389a.dll"));
            return md;
        }

        // For unit testing
        internal IEnumerable<string> GetReferencePaths()
        {
            var referencePaths = new List<string>(_options.AdditionalReferencePaths.Count);

            foreach (var part in _partManager.ApplicationParts)
            {
                if (part is ICompilationReferencesProvider compilationReferenceProvider)
                {
                    referencePaths.AddRange(compilationReferenceProvider.GetReferencePaths());
                }
                else if (part is AssemblyPart assemblyPart)
                {
                    referencePaths.AddRange(assemblyPart.GetReferencePaths());
                }
            }

            referencePaths.AddRange(_options.AdditionalReferencePaths);

            return referencePaths;
        }

        private static MetadataReference CreateMetadataReference(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);

                return assemblyMetadata.GetReference(filePath: path);
            }
        }
    }
}
