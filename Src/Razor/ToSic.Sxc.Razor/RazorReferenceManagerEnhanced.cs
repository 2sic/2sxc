using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Razor.DotNetOverrides;

namespace ToSic.Sxc.Razor
{
    internal class RazorReferenceManagerEnhanced(ApplicationPartManager partManager, IOptions<MvcRazorRuntimeCompilationOptions> options) : RazorReferenceManager(partManager, options)
    {
        // cache references for reuse;
        public override IReadOnlyList<MetadataReference> CompilationReferences => _compilationReferences ??= base.CompilationReferences;
        private IReadOnlyList<MetadataReference> _compilationReferences;

        public IReadOnlyList<MetadataReference> GetAdditionalCompilationReferences(IEnumerable<string> additionalReferencePaths)
        {
            if (additionalReferencePaths == null) return CompilationReferences;

            var additionalMetadataReferences = additionalReferencePaths
                .Select(CreateMetadataReference)
                .ToList();
            return CompilationReferences.Concat(additionalMetadataReferences).ToList().AsReadOnly(); ;
        }
    }
}
