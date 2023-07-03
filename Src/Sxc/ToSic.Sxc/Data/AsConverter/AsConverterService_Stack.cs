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

        public ITypedStack AsStack(string name, params object[] parts)
        {
            name = name ?? Eav.Constants.NullNameId;
            var l = Log.Fn<ITypedStack>($"'{name}', {parts?.Length}");

            // Error if nothing
            if (parts == null || !parts.SafeAny())
                throw l.Ex(new ArgumentNullException(nameof(parts)));

            // Return if already correct (this is just to type-cast the object from dynamic code)
            if (parts.Length == 1 && parts[0] is ITypedStack alreadyStack)
                return alreadyStack;
            
            // Must create a stack
            var sources = parts
                .Select(e => e as IPropertyLookup)
                .Where(e => e != null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e))
                .ToList();
            return l.ReturnAsOk(AsStack(name, sources));
        }

        public DynamicStack AsStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources) 
            => new DynamicStack(name, DynamicEntityServices, sources);
    }
}
