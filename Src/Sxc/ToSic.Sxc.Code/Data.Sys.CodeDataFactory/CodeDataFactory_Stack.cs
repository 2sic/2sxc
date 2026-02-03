using System.Collections;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Eav.Models;
using ToSic.Eav.Sys;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    [PrivateApi]
    public ITypedStack AsStack(object[] parts)
        => AsStack(null, parts, strictTypes: true, AsTypedStack);

    [PrivateApi]
    public T AsStack<T>(object[] parts)
        where T : class, IModelOfData, new() 
        => AsCustom<T>(AsStack(parts));

    private TStackType AsStack<TStackType>(string? name, object[] parts, bool strictTypes, Func<string, List<KeyValuePair<string, IPropertyLookup>>, TStackType> generate)
    {
        name ??= EavConstants.NullNameId;
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
                                 ? GetPropertyLookupOrNull(maybePlEnum
                                     .Cast<object>()
                                     .FirstOrDefault(mpl => mpl is IHasPropLookup or IPropertyLookup)
                                 )
                                 : null);
                return new { index, original, lookup };
            })
            .Where(s => s.original is not null)
            .ToList();

        // If strict (new implementation for typed) throw error if unexpected data arrived
        if (strictTypes)
        {
            var unexpected = cleaned
                .Where(s => s.lookup is null)
                .ToList();
            if (unexpected.Any())
            {
                var names = string.Join(", ", unexpected.Select(s => s.index + ":" + s.original.GetType()));
                throw l.Ex(new ArgumentException($"Tried to do {nameof(AsStack)} but got some objects are not {nameof(IPropertyLookup)}/{nameof(IHasPropLookup)}. Index/Type: {names}"));
            }
        }

        // Must create a stack
        var sources = cleaned
            .Where(s => s.lookup != null)
            .Select(s => new KeyValuePair<string, IPropertyLookup>($"lookup-{s.index}", s.lookup!))
            .ToList();
        return l.ReturnAsOk(generate(name, sources));
    }

    private static IPropertyLookup? GetPropertyLookupOrNull(object? original)
        => original switch
        {
            IPropertyLookup pl => pl,
            IHasPropLookup hasPl => hasPl.PropertyLookup,
            _ => null
        };

    public IDynamicStack AsDynStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources)
        => new DynamicStack.DynamicStack(name, this, sources);

    public ITypedStack AsTypedStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources)
        => new TypedStack.TypedStack(name, this, sources);
}