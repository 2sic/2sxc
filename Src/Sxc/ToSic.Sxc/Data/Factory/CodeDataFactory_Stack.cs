using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data;

public partial class CodeDataFactory
{
    [PrivateApi]
    public ITypedStack AsStack(object[] parts) => AsStack(null, parts, strictTypes: true, AsTypedStack);

    public TStackType AsStack<TStackType>(string name, object[] parts, bool strictTypes, Func<string, List<KeyValuePair<string, IPropertyLookup>>, TStackType> generate)
    {
        name = name ?? Eav.Constants.NullNameId;
        var l = Log.Fn<TStackType>($"'{name}', {parts?.Length}");

        // Error if nothing
        if (parts == null || !parts.SafeAny())
            throw l.Ex(new ArgumentNullException(nameof(parts)));

        // Return if already correct (this is just to type-cast the object from dynamic code)
        if (parts.Length == 1 && parts[0] is TStackType alreadyStack)
            return alreadyStack;
            
        // Filter out unexpected
        var cleaned = parts
            .Select((original, index) =>
            {
                var lookup = GetPropertyLookupOrNull(original)
                             // This should cover the case where it's a list<ITypedItem> or similar
                             ?? (original is IEnumerable maybePlEnum
                                 ? GetPropertyLookupOrNull(maybePlEnum.Cast<object>()
                                     .FirstOrDefault(mpl => mpl is IHasPropLookup || mpl is IPropertyLookup))
                                 : null);
                return new { index, original, lookup };
            })
            .Where(s => !(s.original is null))
            .ToList();

        // If strict (new implementation for typed) throw error if unexpected data arrived
        if (strictTypes)
        {
            var unexpected = cleaned.Where(s => s.lookup is null).ToList();
            if (unexpected.Any())
            {
                var names = string.Join(", ", unexpected.Select(s => s.index + ":" + s.original.GetType()));
                throw l.Ex(new ArgumentException($"Tried to do {nameof(AsStack)} but got some objects are not {nameof(IPropertyLookup)}/{nameof(IHasPropLookup)}. Index/Type: {names}"));
            }
        }

        // Must create a stack
        var sources = cleaned
            .Select(s => new KeyValuePair<string, IPropertyLookup>(null, s.lookup))
            .ToList();
        return l.ReturnAsOk(generate(name, sources));
    }

    private IPropertyLookup GetPropertyLookupOrNull(object original) =>
        original is IPropertyLookup pl
            ? pl
            : original is IHasPropLookup hasPl
                ? hasPl.PropertyLookup
                : null;

    public DynamicStack AsDynStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources) =>
        new(name, this, sources);

    public ITypedStack AsTypedStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources)
        => new TypedStack(name, this, sources);
}