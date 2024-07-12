using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Internal.Dynamic;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Internal.Stack;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class TypedStack: IWrapper<IPropertyStack>, ITypedStack, IHasPropLookup, ICanDebug, ICanGetByName
{
    public TypedStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
    {
        _stack = new PropertyStack().Init(name, sources);
        Cdf = cdf;
        _stackPropLookup = new(_stack, () => Debug);
        _helper = new(this, cdf, propsRequired: false, childrenShouldBeDynamic: false, canDebug: this);
        _itemHelper = new(_helper, this);
    }

    private readonly IPropertyStack _stack;
    IPropertyLookup IHasPropLookup.PropertyLookup => _stackPropLookup;
    private readonly PropLookupStack _stackPropLookup;
    private readonly GetAndConvertHelper _helper;
    private readonly CodeItemHelper _itemHelper;

    public IPropertyStack GetContents() => _stack;

    public CodeDataFactory Cdf { get; }

    public bool Debug { get; set; }

    #region GetByName - to allow this to be used for image settings etc.

    object ICanGetByName.Get(string name) => (this as ITyped).Get(name, required: false);

    #endregion


    #region ITyped.Keys and Dyn - both not implemented

    [PrivateApi]
    public bool ContainsKey(string name)
        => throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => _itemHelper.IsEmpty(name, noParamOrder, isBlank: default, language: language);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => _itemHelper.IsNotEmpty(name, noParamOrder, isBlank: default, language: language);

    // TODO: Keys()
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default)
    {
        //var keys = _stack.Sources;
        throw new NotImplementedException();
    }

    #endregion

    #region ITyped

    [PrivateApi]
    object ITyped.Get(string name, NoParamOrder noParamOrder, bool? required, string language)
        => _itemHelper.Get(name: name, noParamOrder: noParamOrder, required: required, language: language);

    [PrivateApi]
    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, string language)
        => _itemHelper.GetT(name, noParamOrder, fallback: fallback, required: required, language: language);


    [PrivateApi]
    IRawHtmlString ITyped.Attribute(string name, NoParamOrder noParamOrder, string fallback, bool? required)
        => _itemHelper.Attribute(name, noParamOrder, fallback, required);


    [PrivateApi]
    DateTime ITyped.DateTime(string name, NoParamOrder noParamOrder, DateTime fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    string ITyped.String(string name, NoParamOrder noParamOrder, string fallback, bool? required, object scrubHtml)
        => _itemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

    [PrivateApi]
    int ITyped.Int(string name, NoParamOrder noParamOrder, int fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    bool ITyped.Bool(string name, NoParamOrder noParamOrder, bool fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    long ITyped.Long(string name, NoParamOrder noParamOrder, long fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    float ITyped.Float(string name, NoParamOrder noParamOrder, float fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    decimal ITyped.Decimal(string name, NoParamOrder noParamOrder, decimal fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    double ITyped.Double(string name, NoParamOrder noParamOrder, double fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    string ITyped.Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
        => _itemHelper.Url(name, noParamOrder, fallback, required);

    [PrivateApi]
    string ITyped.ToString() => "test / debug: " + ToString();



    #endregion

    #region Add-Ons for ITypedStack

    ITypedItem /*ITypedStack*/ITypedItem.Child(string name, NoParamOrder noParamOrder, bool? required)
    {
        var findResult = _helper.TryGet(name);
        return IsErrStrict(findResult.Found, required, _helper.PropsRequired)
            ? throw ErrStrict(name)
            : Cdf.AsItem(findResult.Result);
    }

    IEnumerable<ITypedItem> /*ITypedStack*/ITypedItem.Children(string field, NoParamOrder noParamOrder, string type, bool? required)
    {
        var findResult = _helper.TryGet(field);
        return IsErrStrict(findResult.Found, required, _helper.PropsRequired)
            ? throw ErrStrict(field)
            : Cdf.AsItems(findResult.Result)
                // Apply type filter - even if a bit "late"
                .Where(i => i.Entity.Type.Is(type))
                .ToList();
    }

    #endregion
}