using System.Runtime.CompilerServices;
using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TypedModel(
    IDictionary<string, object> paramsDictionary,
    ICodeApiService codeApiSvc,
    bool isRazor,
    string razorFileName)
    : ITypedModel
{
    private readonly IDictionary<string, object> _paramsDictionary = paramsDictionary?.ToInvariant() ?? new Dictionary<string, object>();
    private readonly TypedConverter _converter = new(codeApiSvc._Cdf);

    #region Check if parameters were supplied

    public bool ContainsKey(string name) => !name.IsEmptyOrWs() && _paramsDictionary.ContainsKey(name);

    [PrivateApi]
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default) //, bool? blankIs = default)
        => HasKeysHelper.IsEmpty(Get(name, required: false), default /*blankIs*/);

    [PrivateApi]
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default) //, bool? blankIs = default)
        => HasKeysHelper.IsNotEmpty(Get(name, required: false), default /*blankIs*/);

    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) 
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, _paramsDictionary?.Keys);

    #endregion

    #region Get and GetInternal

    public object Get(string name, NoParamOrder noParamOrder = default, bool? required = default) 
        => GetInternalObj(name, noParamOrder, required);

    public T Get<T>(string name, NoParamOrder noParamOrder = default, T fallback = default, bool? required = default) 
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
    private T GetInternal<T>(string name, NoParamOrder noParamOrder, T fallback, object fallbackAsObj, bool? required, [CallerMemberName] string method = default)
    {
        // If we have a clear fallback, don't make it required
        if (fallbackAsObj is not null || (fallback != null && fallback.IsNotDefault()))
            required = false;

        var found = GetInternalObj(name, noParamOrder, required, method: method);
            
        if (found == null) return fallback;

        // Already matching type OR Interface (because ConvertOrFallback can't handle interface)
        if (found is T typed) return typed;

        return typeof(T).IsInterface ? fallback : found.ConvertOrFallback(fallback);
    }

    private object GetInternalObj(string name, NoParamOrder noParamOrder, bool? required, [CallerMemberName] string method = default)
    {
        if (_paramsDictionary.TryGetValue(name, out var result))
            return result;
        if (required == false)
            return null;

        var call = $"{nameof(TypedModel)}.{method}(\"{name}\")";
        var callReqFalse = call.Replace(")", ", required: false)");
        throw new ArgumentException($@"Tried to get parameter with {call} but parameter '{name}' not provided. 
Either change the calling Html.Partial(""{razorFileName}"", {{ {name} = ... }} ) or use {callReqFalse} to make it optional.", nameof(name));
    }

    #endregion

    public dynamic Code(string name, NoParamOrder noParamOrder = default, object fallback = default, bool? required = default)
        => GetInternal(name: name, noParamOrder: noParamOrder, fallback: fallback, fallbackAsObj: fallback, required: required);

    #region Numbers

    public int Int(string name, NoParamOrder noParamOrder = default, int? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public float Float(string name, NoParamOrder noParamOrder = default, float? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public double Double(string name, NoParamOrder noParamOrder = default, double? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public decimal Decimal(string name, NoParamOrder noParamOrder = default, decimal? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion

    #region Standard value types

    public string String(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback, fallbackAsObj: fallback, required: required);

    public Guid Guid(string name, NoParamOrder noParamOrder = default, Guid? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public bool Bool(string name, NoParamOrder noParamOrder = default, bool? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    public DateTime DateTime(string name, NoParamOrder noParamOrder = default, DateTime? fallback = default, bool? required = default) 
        => GetInternal(name, noParamOrder, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion


    //#region Stacks

    //public ITypedStack Stack(string name, NoParamOrder noParamOrder = default, ITypedStack fallback = default, bool? required = default) 
    //    => _converter.Stack(GetInternal(name, required, noParamOrder), fallback);

    //#endregion

    #region Adam

    public IFile File(string name, NoParamOrder noParamOrder = default, IFile fallback = default, bool? required = default) 
        => _converter.File(GetInternalObj(name, noParamOrder, required), fallback);

    public IEnumerable<IFile> Files(string name, NoParamOrder noParamOrder = default, IEnumerable<IFile> fallback = default, bool? required = default) 
        => _converter.Files(GetInternalObj(name, noParamOrder, required), fallback);

    public IFolder Folder(string name, NoParamOrder noParamOrder = default, IFolder fallback = default, bool? required = default)
        => _converter.Folder(GetInternalObj(name, noParamOrder, required), fallback);

    public IEnumerable<IFolder> Folders(string name, NoParamOrder noParamOrder = default, IEnumerable<IFolder> fallback = default, bool? required = default)
        => _converter.Folders(GetInternalObj(name, noParamOrder, required), fallback);

    #endregion

    #region Entity and Item(s)

    public ITypedItem Item(string name, NoParamOrder noParamOrder = default, ITypedItem fallback = default, bool? required = default)
        => _converter.Item(GetInternalObj(name, noParamOrder, required), noParamOrder, fallback);

    public IEnumerable<ITypedItem> Items(string name, NoParamOrder noParamOrder = default, IEnumerable<ITypedItem> fallback = default, bool? required = default)
        => _converter.Items(GetInternalObj(name, noParamOrder, required), noParamOrder, fallback);

    #endregion

    #region HtmlTags

    public IHtmlTag HtmlTag(string name, NoParamOrder noParamOrder = default, IHtmlTag fallback = default, bool? required = default)
        => _converter.HtmlTag(GetInternalObj(name, noParamOrder, required), fallback);

    public IEnumerable<IHtmlTag> HtmlTags(string name, NoParamOrder noParamOrder = default, IEnumerable<IHtmlTag> fallback = default, bool? required = default)
        => _converter.HtmlTags(GetInternalObj(name, noParamOrder, required), fallback);


    #endregion

    #region Toolbar

    public IToolbarBuilder Toolbar(string name, NoParamOrder noParamOrder = default, IToolbarBuilder fallback = default, bool? required = default)
        => _converter.Toolbar(GetInternalObj(name, noParamOrder, required), fallback);

    #endregion

    #region As Conversion (new v17.02) - turn off for now, GET<T> should do the job

    ///// <summary>
    ///// EXPERIMENTAL
    ///// </summary>
    ///// <returns></returns>
    //public T As<T>(string name, NoParamOrder protector = default, T fallback = default, bool? required = default)
    //    where T : class, ITypedItemWrapper16, ITypedItem, new()
    //    => Get(name, required: required ?? fallback != null) switch
    //    {
    //        T already => already,
    //        ICanBeEntity canBeEntity => codeApiSvc._Cdf.AsCustom<T>(canBeEntity, kit: codeApiSvc.GetKit<ServiceKit16>(),
    //            protector: protector, nullIfNull: true) ?? fallback,
    //        _ => fallback
    //    };

    ///// <summary>
    ///// EXPERIMENTAL
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="protector"></param>
    ///// <param name="nullIfNull"></param>
    ///// <typeparam name="T"></typeparam>
    ///// <returns></returns>
    //public IEnumerable<T> AsList<T>(string name, NoParamOrder protector = default, IEnumerable<T> fallback = default, bool? required = default)
    //    where T : class, ITypedItemWrapper16, ITypedItem, new()
    //{
    //    if (Get(name, required: required ?? fallback != null) is not IEnumerable<IEntity> maybe)
    //        return fallback;

    //    if (maybe is IEnumerable<T> already)
    //        return already;

    //    return codeApiSvc._Cdf.AsCustomList<T>(
    //               maybe,
    //               kit: codeApiSvc.GetKit<ServiceKit16>(),
    //               protector: protector,
    //               nullIfNull: true
    //           )
    //           ?? fallback;
    //}

    #endregion
}