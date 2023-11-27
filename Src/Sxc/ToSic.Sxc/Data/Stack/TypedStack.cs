using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Coding;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Markup;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TypedStack: IWrapper<IPropertyStack>, ITypedStack, IHasPropLookup, ICanDebug, ICanGetByName
{
    public TypedStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
    {
        _stack = new PropertyStack().Init(name, sources);
        Cdf = cdf;
        PropertyLookup = new PropLookupStack(_stack, () => Debug);
        _helper = new GetAndConvertHelper(this, cdf, propsRequired: false, childrenShouldBeDynamic: false, canDebug: this);
        _itemHelper = new CodeItemHelper(_helper, this);
    }

    private readonly IPropertyStack _stack;
    [PrivateApi]
    public IPropertyLookup PropertyLookup { get; }
    private readonly GetAndConvertHelper _helper;
    private readonly CodeItemHelper _itemHelper;

    public IPropertyStack GetContents() => _stack;

    public CodeDataFactory Cdf { get; }

    public bool Debug { get; set; }

    #region GetByName - to allow this to be used for image settings etc.

    object ICanGetByName.Get(string name) => (this as ITyped).Get(name);

    #endregion


    #region ITyped.Keys and Dyn - both not implemented

    [PrivateApi]
    public bool ContainsKey(string name) 
        => throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default)
        => _itemHelper.IsEmpty(name, noParamOrder, default);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default)
        => _itemHelper.IsFilled(name, noParamOrder, default);

    // TODO: Keys()
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default)
    {
        //var keys = _stack.Sources;
        throw new NotImplementedException();
    }

    #endregion

    #region ITyped

    [PrivateApi]
    object ITyped.Get(string name, NoParamOrder noParamOrder, bool? required)
        => _itemHelper.Get(name, noParamOrder, required);

    [PrivateApi]
    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required)
        => _itemHelper.G4T(name, noParamOrder, fallback: fallback, required: required);

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

    ITypedItem ITypedStack.Child(string name, NoParamOrder noParamOrder, bool? required)
    {
        var findResult = _helper.TryGet(name);
        return IsErrStrict(findResult.Found, required, _helper.PropsRequired)
            ? throw ErrStrict(name)
            : Cdf.AsItem(findResult.Result, noParamOrder);
    }

    IEnumerable<ITypedItem> ITypedStack.Children(string field, NoParamOrder noParamOrder, string type, bool? required)
    {
        var findResult = _helper.TryGet(field);
        return IsErrStrict(findResult.Found, required, _helper.PropsRequired)
            ? throw ErrStrict(field)
            : Cdf.AsItems(findResult.Result, noParamOrder)
                // Apply type filter - even if a bit "late"
                .Where(i => i.Entity.Type.Is(type))
                .ToList();
    }

    #endregion
}