using System.Runtime.CompilerServices;
using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TypedModel(
    CodeHelperSpecs helperSpecs,
    IDictionary<string, object> paramsDictionary,
#pragma warning disable CS9113 // Parameter is unread.
    bool isRazor,
#pragma warning restore CS9113 // Parameter is unread.
    string razorFileName)
    : ITypedModel
{
    private readonly IDictionary<string, object> _paramsDictionary = paramsDictionary?.ToInvariant() ?? new Dictionary<string, object>();
    private readonly TypedConverter _converter = new(helperSpecs.CodeApiSvc.Cdf);

    #region Check if parameters were supplied

    public bool ContainsKey(string name) => !name.IsEmptyOrWs() && _paramsDictionary.ContainsKey(name);

    [PrivateApi]
    public bool IsEmpty(string name, NoParamOrder protector = default, string language = default /* ignore */)
        => HasKeysHelper.IsEmpty(Get(name, required: false), blankIsEmpty: default);

    [PrivateApi]
    public bool IsNotEmpty(string name, NoParamOrder protector = default, string language = default /* ignore */)
        => HasKeysHelper.IsNotEmpty(Get(name, required: false), blankIsEmpty: default);

    public IEnumerable<string> Keys(NoParamOrder protector = default, IEnumerable<string> only = default) 
        => TypedHelpers.FilterKeysIfPossible(protector, only, _paramsDictionary?.Keys);

    #endregion

    #region Get and GetInternal

    public object Get(string name, NoParamOrder protector = default, bool? required = default) 
        => GetInternalObj(name, protector, required);

    public T Get<T>(string name, NoParamOrder protector = default, T fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback, fallbackAsObj: fallback, required: required);

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

        return found switch
        {
            null => fallback,
            // Already matching type OR Interface (because ConvertOrFallback can't handle interface)
            T typed => typed,
            _ => typeof(T).IsInterface ? fallback : found.ConvertOrFallback(fallback)
        };
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

    public dynamic Code(string name, NoParamOrder protector = default, object fallback = default, bool? required = default)
        => GetInternal(name: name, noParamOrder: protector, fallback: fallback, fallbackAsObj: fallback, required: required);

    #region Numbers

    public int Int(string name, NoParamOrder protector = default, int? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    public float Float(string name, NoParamOrder protector = default, float? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    public double Double(string name, NoParamOrder protector = default, double? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    public decimal Decimal(string name, NoParamOrder protector = default, decimal? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion

    #region Standard value types

    public string String(string name, NoParamOrder protector = default, string fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback, fallbackAsObj: fallback, required: required);

    public Guid Guid(string name, NoParamOrder protector = default, Guid? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    public bool Bool(string name, NoParamOrder protector = default, bool? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    public DateTime DateTime(string name, NoParamOrder protector = default, DateTime? fallback = default, bool? required = default) 
        => GetInternal(name, protector, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion


    //#region Stacks

    //public ITypedStack Stack(string name, NoParamOrder protector = default, ITypedStack fallback = default, bool? required = default) 
    //    => _converter.Stack(GetInternal(name, required, noParamOrder), fallback);

    //#endregion

    #region Adam

    public IFile File(string name, NoParamOrder protector = default, IFile fallback = default, bool? required = default) 
        => _converter.File(GetInternalObj(name, protector, required), fallback);

    public IEnumerable<IFile> Files(string name, NoParamOrder protector = default, IEnumerable<IFile> fallback = default, bool? required = default) 
        => _converter.Files(GetInternalObj(name, protector, required), fallback);

    public IFolder Folder(string name, NoParamOrder protector = default, IFolder fallback = default, bool? required = default)
        => _converter.Folder(GetInternalObj(name, protector, required), fallback);

    public IEnumerable<IFolder> Folders(string name, NoParamOrder protector = default, IEnumerable<IFolder> fallback = default, bool? required = default)
        => _converter.Folders(GetInternalObj(name, protector, required), fallback);

    public GpsCoordinates Gps(string name, NoParamOrder protector = default, GpsCoordinates fallback = default, bool? required = default)
        => GetInternal(name: name, noParamOrder: protector, fallback: fallback, fallbackAsObj: fallback, required: required);

    #endregion

    #region Entity and Item(s)

    public ITypedItem Item(string name, NoParamOrder protector = default, ITypedItem fallback = default, bool? required = default)
        => _converter.Item(GetInternalObj(name, protector, required), protector, fallback);

    public IEnumerable<ITypedItem> Items(string name, NoParamOrder protector = default, IEnumerable<ITypedItem> fallback = default, bool? required = default)
        => _converter.Items(GetInternalObj(name, protector, required), protector, fallback);

    #endregion

    #region HtmlTags

    public IHtmlTag HtmlTag(string name, NoParamOrder protector = default, IHtmlTag fallback = default, bool? required = default)
        => _converter.HtmlTag(GetInternalObj(name, protector, required), fallback);

    public IEnumerable<IHtmlTag> HtmlTags(string name, NoParamOrder protector = default, IEnumerable<IHtmlTag> fallback = default, bool? required = default)
        => _converter.HtmlTags(GetInternalObj(name, protector, required), fallback);


    #endregion

    #region Toolbar

    public IToolbarBuilder Toolbar(string name, NoParamOrder protector = default, IToolbarBuilder fallback = default, bool? required = default)
        => _converter.Toolbar(GetInternalObj(name, protector, required), fallback);

    #endregion

}