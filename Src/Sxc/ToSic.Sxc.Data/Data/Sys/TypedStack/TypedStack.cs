using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Eav.Data.Sys.PropertyStack;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Options;
using ToSic.Sxc.Data.Sys.Dynamic;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Stack;
using static ToSic.Sxc.Data.Sys.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Sys.TypedStack;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class TypedStack: IWrapper<IPropertyStack>, ITypedStack, IHasPropLookup
{
    public TypedStack(string name, ICodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
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

    public ICodeDataFactory Cdf { get; }

    public bool Debug { get; set; }

    #region GetByName - to allow this to be used for image settings etc.

    object? ICanGetByName.Get(string name) => (this as ITyped).Get(name, required: false);

    #endregion


    #region ITyped.Keys and Dyn - both not implemented

    [PrivateApi]
    public bool ContainsKey(string name)
        => throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");

    public bool IsEmpty(string name, NoParamOrder npo = default, string? language = default)
        => _itemHelper.IsEmpty(name, npo, isBlank: default, language: language);

    public bool IsNotEmpty(string name, NoParamOrder npo = default, string? language = default)
        => _itemHelper.IsNotEmpty(name, npo, isBlank: default, language: language);

    // TODO: Keys()
    public IEnumerable<string> Keys(NoParamOrder npo = default, IEnumerable<string>? only = default)
    {
        //var keys = _stack.Sources;
        throw new NotImplementedException();
    }

    #endregion

    #region ITyped Value Get Methods

    [PrivateApi]
    object? ITyped.Get(string name, NoParamOrder npo, bool? required, string? language)
        => _itemHelper.Get(name: name, npo: npo, required: required, language: language);

    [PrivateApi]
    TValue? ITyped.Get<TValue>(string name, NoParamOrder npo, TValue? fallback, bool? required, string? language)
        where TValue : default
        => _itemHelper.GetT(name, npo, fallback: fallback, required: required, language: language);


    [PrivateApi]
    IRawHtmlString? ITyped.Attribute(string name, NoParamOrder npo, string? fallback, bool? required)
        => _itemHelper.Attribute(name, npo, fallback, required);


    [PrivateApi]
    DateTime ITyped.DateTime(string name, NoParamOrder npo, DateTime fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    string? ITyped.String(string name, NoParamOrder npo, string? fallback, bool? required, object? scrubHtml)
        => _itemHelper.String(name, npo, fallback, required, scrubHtml);

    [PrivateApi]
    int ITyped.Int(string name, NoParamOrder npo, int fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    bool ITyped.Bool(string name, NoParamOrder npo, bool fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    long ITyped.Long(string name, NoParamOrder npo, long fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    float ITyped.Float(string name, NoParamOrder npo, float fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    decimal ITyped.Decimal(string name, NoParamOrder npo, decimal fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    double ITyped.Double(string name, NoParamOrder npo, double fallback, bool? required)
        => _itemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    [PrivateApi]
    string? ITyped.Url(string name, NoParamOrder npo, string? fallback, bool? required)
        => _itemHelper.Url(name, npo, fallback, required);

    [PrivateApi]
    string ITyped.ToString() => "test / debug: " + ToString();

    #endregion

    #region Add-Ons for ITypedStack

    ITypedItem? /*ITypedStack*/ITypedItem.Child(string name, NoParamOrder npo, bool? required, GetRelatedOptions? options)
    {
        var findResult = _helper.TryGet(name);
        return IsErrStrict(findResult.Found, required, _helper.PropsRequired)
            ? throw ErrStrict(name)
            : Cdf.AsItem(findResult.Result!, new() { EntryPropIsRequired = required ?? true, ItemIsStrict = _helper.PropsRequired });
    }

    IEnumerable<ITypedItem> /*ITypedStack*/ITypedItem.Children(string? field, NoParamOrder npo, string? type, bool? required, GetRelatedOptions? options)
    {
        var findResult = _helper.TryGet(field);
        return IsErrStrict(findResult.Found, required, _helper.PropsRequired)
            ? throw ErrStrict(field)
            // TODO: #ConvertItemSettings - not sure if this default is correct
            : Cdf.AsItems(findResult.Result!, new() { EntryPropIsRequired = required ?? true, ItemIsStrict = _helper.PropsRequired })
                // Apply type filter - even if a bit "late"
                .Where(i => i.Entity.Type.Is(type))
                .ToList();
    }

    #endregion
}