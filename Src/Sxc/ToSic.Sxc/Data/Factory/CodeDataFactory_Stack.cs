using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    public partial class CodeDataFactory
    {
        [PrivateApi]
        public ITypedStack AsStack(object[] parts) => AsStack(null, parts, AsTypedStack);

        //public ITypedStack AsStack(string name, params object[] parts) 
        //    => AsStack(name, parts, AsTypedStack);

        public TStackType AsStack<TStackType>(string name, object[] parts, Func<string, List<KeyValuePair<string, IPropertyLookup>>, TStackType> generate)
        {
            name = name ?? Eav.Constants.NullNameId;
            var l = Log.Fn<TStackType>($"'{name}', {parts?.Length}");

            // Error if nothing
            if (parts == null || !parts.SafeAny())
                throw l.Ex(new ArgumentNullException(nameof(parts)));

            // Return if already correct (this is just to type-cast the object from dynamic code)
            if (parts.Length == 1 && parts[0] is TStackType alreadyStack)
                return alreadyStack;
            
            // Must create a stack
            var sources = parts
                .Select(e => e as IPropertyLookup)
                .Where(e => e != null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e))
                .ToList();
            return l.ReturnAsOk(generate(name, sources));
        }

        

        public DynamicStack AsDynStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources) 
            => new DynamicStack(name, this, sources);

        public ITypedStack AsTypedStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources)
            => new TypedStack(name, this, sources);
    }
}
