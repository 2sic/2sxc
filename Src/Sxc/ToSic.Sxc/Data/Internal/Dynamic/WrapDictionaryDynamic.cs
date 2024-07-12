using System.Dynamic;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data.Internal.Dynamic;

// WIP
// Inspired by https://stackoverflow.com/questions/46948289/how-do-you-convert-any-c-sharp-object-to-an-expandoobject
// That was more complex with ability so set new values and switch between case-insensitive or not but that's not the purpose of this
/// <summary>
/// 
/// </summary>
/// <remarks>
/// Will always return true even if the property doesn't exist, in which case it resolves to null.
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class WrapDictionaryDynamic<TKey, TVal>: DynamicObject, IWrapper<IDictionary<TKey, TVal>>, IHasKeys
{
    protected readonly IDictionary<TKey, TVal> UnwrappedDictionary;
    private readonly CodeDataWrapper _factory;

    [PrivateApi]
    public IDictionary<TKey, TVal> GetContents() => UnwrappedDictionary;
    private readonly Dictionary<string, object> _ignoreCaseLookup = new(StringComparer.InvariantCultureIgnoreCase);

    public WrapDictionaryDynamic(IDictionary<TKey, TVal> dictionary, CodeDataWrapper factory)
    {
        UnwrappedDictionary = dictionary;
        _factory = factory;
        if (dictionary == null) return;

        foreach (var de in dictionary) 
            _ignoreCaseLookup[de.Key.ToString()] = de.Value;
    }
        
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        // if nothing found, just return true/done
        if(!_ignoreCaseLookup.TryGetValue(binder.Name, out result))
            return true;

        // if result is an anonymous object, re-wrap again for consistency with other APIs
        if (result is null) return true;
        if (result.IsAnonymous())
            result = _factory.ChildNonJsonWrapIfPossible(data: result, wrapNonAnon: false, WrapperSettings.Dyn(children: true, realObjectsToo: false));

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value) 
        => throw new NotSupportedException($"Setting a value on DynamicReadDictionary is not supported");

    #region Typed



    #endregion

    [PrivateApi]
    bool IHasKeys.ContainsKey(string name) => _ignoreCaseLookup.ContainsKey(name);

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default /* ignore */)
        => !_ignoreCaseLookup.TryGetValue(name, out var result) || HasKeysHelper.IsEmpty(result, blankIsEmpty: default);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default /* ignore */)
        => _ignoreCaseLookup.TryGetValue(name, out var result) && HasKeysHelper.IsNotEmpty(result, blankIsEmpty: default);


    IEnumerable<string> IHasKeys.Keys(NoParamOrder noParamOrder, IEnumerable<string> only) 
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, _ignoreCaseLookup?.Keys);
}