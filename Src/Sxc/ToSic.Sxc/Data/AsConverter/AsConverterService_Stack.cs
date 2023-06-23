using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {
        [PrivateApi]
        public ITypedStack MergeTyped(params object[] parts)
        {
            return AsStack(null, DynamicEntityServices, parts);
        }

        public DynamicStack AsStack(string name, params object[] parts)
        {
            if (parts == null || !parts.SafeAny()) throw new ArgumentNullException(nameof(parts));
            
            // Must create a stack
            var sources = parts
                .Select(e => e as IPropertyLookup)
                .Where(e => e != null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e))
                .ToList();
            return new DynamicStack(name ?? Eav.Constants.NullNameId, DynamicEntityServices, sources);

        }
    }
}
