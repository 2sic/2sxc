using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {
        [PrivateApi]
        public ITypedStack AsStack(params object[] parts) => AsStack(null, parts);

        public DynamicStack AsStack(string name, params object[] parts)
        {
            name = name ?? Eav.Constants.NullNameId;
            var l = Log.Fn<DynamicStack>($"'{name}', {parts?.Length}");

            if (parts == null || !parts.SafeAny()) throw l.Ex(new ArgumentNullException(nameof(parts)));
            
            // Must create a stack
            var sources = parts
                .Select(e => e as IPropertyLookup)
                .Where(e => e != null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e))
                .ToList();
            return l.ReturnAsOk(new DynamicStack(name, DynamicEntityServices, sources));

        }
    }
}
