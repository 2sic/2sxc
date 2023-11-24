using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Edit.Toolbar;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypedModel : ITypedModel
{
    private readonly bool _isRazor;
    private readonly string _razorFileName;
    private readonly IDictionary<string, object> _paramsDictionary;
    private readonly TypedConverter _converter;

    internal TypedModel(IDictionary<string, object> paramsDictionary, IDynamicCodeRoot codeRoot, bool isRazor, string razorFileName)
    {
        _isRazor = isRazor;
        _razorFileName = razorFileName;
        _paramsDictionary = paramsDictionary?.ToInvariant() ?? new Dictionary<string, object>();
        _converter = new TypedConverter(codeRoot.Cdf);
    }

    #region Check if parameters were supplied

    public bool ContainsKey(string name) => !name.IsEmptyOrWs() && _paramsDictionary.ContainsKey(name);

    [PrivateApi]
    public bool IsEmpty(string name, string noParamOrder = Protector) //, bool? blankIs = default)
        => HasKeysHelper.IsEmpty(Get(name, required: false), default /*blankIs*/);

    [PrivateApi]
    public bool IsNotEmpty(string name, string noParamOrder = Protector) //, bool? blankIs = default)
        => HasKeysHelper.IsNotEmpty(Get(name, required: false), default /*blankIs*/);

    public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default) 
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, _paramsDictionary?.Keys);

    #endregion

    #region Get and GetInternal

    public object Get(string name, string noParamOrder = Protector, bool? required = default) 
        => GetInternalObj(name, noParamOrder, required);

    public T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback, fallbackAsObj: fallback, required: required);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="noParamOrder"></param>
    /// <param name="fallback"></param>
    /// <param name="fallbackAsObj">Untyped fallback, for special null-checks</param>
    /// <param name="required"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private T GetInternal<T>(string name, string noParamOrder, T fallback, object fallbackAsObj, bool? required, [CallerMemberName] string method = default)
    {
        // If we have a clear fallback, don't make it required
        if (!(fallbackAsObj is null) || (fallback != null && fallback.IsNotDefault()))
            required = false;

        var found = GetInternalObj(name, noParamOrder, required, method: method);
            
        if (found == null) return fallback;

        // Already matching type OR Interface (because ConvertOrFallback can't handle interface)
        if (found is T typed) return typed;

        return typeof(T).IsInterface ? fallback : found.ConvertOrFallback(fallback);
    }

    private object GetInternalObj(string name, string noParamOrder, bool? required, [CallerMemberName] string method = default)
    {
        Protect(noParamOrder, "required, fallback", method);

        if (_paramsDictionary.TryGetValue(name, out var result))
            return result;
        if (required == false)
            return null;

        var call = $"{nameof(TypedModel)}.{method}(\"{name}\")";
        var callReqFalse = call.Replace(")", ", required: false)");
        throw new ArgumentException($@"Tried to get parameter with {call} but parameter '{name}' not provided. 
Either change the calling Html.Partial(""{_razorFileName}"", {{ {name} = ... }} ) or use {callReqFalse} to make it optional.", nameof(name));
    }

    #endregion

    public dynamic Code(string name, string noParamOrder = Protector, object fallback = default, bool? required = default)
        => GetInternal(name: name, noParamOrder: noParamOrder, fallback: fallback, fallbackAsObj: fallback, required: required);

    #region Numbers

    public int Int(string name, string noParamOrder = Protector, int? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public float Float(string name, string noParamOrder = Protector, float? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public double Double(string name, string noParamOrder = Protector, double? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public decimal Decimal(string name, string noParamOrder = Protector, decimal? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion

    #region Standard value types

    public string String(string name, string noParamOrder = Protector, string fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback, fallbackAsObj: fallback, required: required);

    public Guid Guid(string name, string noParamOrder = Protector, Guid? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public bool Bool(string name, string noParamOrder = Protector, bool? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public DateTime DateTime(string name, string noParamOrder = Protector, DateTime? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion


    //#region Stacks

    //public ITypedStack Stack(string name, string noParamOrder = Protector, ITypedStack fallback = default, bool? required = default) 
    //    => _converter.Stack(GetInternal(name, required, noParamOrder), fallback);

    //#endregion

    #region Adam

    public IFile File(string name, string noParamOrder = Protector, IFile fallback = default, bool? required = default) 
        => _converter.File(GetInternalObj(name, noParamOrder, required), fallback);

    public IEnumerable<IFile> Files(string name, string noParamOrder = Protector, IEnumerable<IFile> fallback = default, bool? required = default) 
        => _converter.Files(GetInternalObj(name, noParamOrder, required), fallback);

    public IFolder Folder(string name, string noParamOrder = Protector, IFolder fallback = default, bool? required = default)
        => _converter.Folder(GetInternalObj(name, noParamOrder, required), fallback);

    public IEnumerable<IFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<IFolder> fallback = default, bool? required = default)
        => _converter.Folders(GetInternalObj(name, noParamOrder, required), fallback);

    #endregion

    #region Entity and Item(s)

    public ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool? required = default)
        => _converter.Item(GetInternalObj(name, noParamOrder, required), noParamOrder, fallback);

    public IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool? required = default)
        => _converter.Items(GetInternalObj(name, noParamOrder, required), noParamOrder, fallback);

    #endregion

    #region HtmlTags

    public IHtmlTag HtmlTag(string name, string noParamOrder = Protector, IHtmlTag fallback = default, bool? required = default)
        => _converter.HtmlTag(GetInternalObj(name, noParamOrder, required), fallback);

    public IEnumerable<IHtmlTag> HtmlTags(string name, string noParamOrder = Protector, IEnumerable<IHtmlTag> fallback = default, bool? required = default)
        => _converter.HtmlTags(GetInternalObj(name, noParamOrder, required), fallback);


    #endregion

    #region Toolbar

    public IToolbarBuilder Toolbar(string name, string noParamOrder = Protector, IToolbarBuilder fallback = default, bool? required = default)
        => _converter.Toolbar(GetInternalObj(name, noParamOrder, required), fallback);

    #endregion
}