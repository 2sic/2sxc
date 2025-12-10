using System.Runtime.CompilerServices;

using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code;

/// <summary>
/// Implementation of the MyModel which has .Get() .String() etc. methods to get parameters from the Razor model.
/// </summary>
/// <param name="helperSpecs"></param>
/// <param name="paramsDictionary"></param>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class TypedRazorModel(CompileCodeHelperSpecs helperSpecs, IDictionary<string, object> paramsDictionary) : ITypedRazorModel
{
    private readonly IDictionary<string, object> _paramsDictionary = paramsDictionary?.ToInvariant() ?? new Dictionary<string, object>();
    private readonly TypedConverter _converter = new(helperSpecs.ExCtx.GetCdf());

    #region Check if parameters were supplied

    public bool ContainsKey(string name) => !name.IsEmptyOrWs() && _paramsDictionary.ContainsKey(name);

    [PrivateApi]
    public bool IsEmpty(string name, NoParamOrder npo = default, string? language = default /* ignore */)
        => HasKeysHelper.IsEmpty(Get(name, required: false), blankIsEmpty: default);

    [PrivateApi]
    public bool IsNotEmpty(string name, NoParamOrder npo = default, string? language = default /* ignore */)
        => HasKeysHelper.IsNotEmpty(Get(name, required: false), blankIsEmpty: default);

    public IEnumerable<string> Keys(NoParamOrder npo = default, IEnumerable<string>? only = default) 
        => TypedHelpers.FilterKeysIfPossible(npo, only, _paramsDictionary.Keys);

    #endregion

    #region Get and GetInternal

    public object? Get(string name, NoParamOrder npo = default, bool? required = default) 
        => GetInternalObj(name, required);

    public T? Get<T>(string name, NoParamOrder npo = default, T? fallback = default, bool? required = default) 
        => GetInternal(name, fallback, fallbackAsObj: fallback, required: required);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="fallback"></param>
    /// <param name="fallbackAsObj">Untyped fallback, for special null-checks</param>
    /// <param name="required"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private T GetInternal<T>(string name, T fallback, object? fallbackAsObj, bool? required, [CallerMemberName] string? method = default)
    {
        // If we have a clear fallback, don't make it required
        if (fallbackAsObj is not null || (fallback != null && fallback.IsNotDefault()))
            required = false;

        var found = GetInternalObj(name, required, method: method);

        return found switch
        {
            null => fallback,
            // Already matching type OR Interface (because ConvertOrFallback can't handle interface)
            T typed => typed,
            _ => typeof(T).IsInterface
                ? fallback
                : found.ConvertOrFallback(fallback)
        };
    }

    private object? GetInternalObj(string name, bool? required, [CallerMemberName] string? method = default)
    {
        if (_paramsDictionary.TryGetValue(name, out var result))
            return result;
        if (required == false)
            return null;

        var call = $"{nameof(TypedRazorModel)}.{method}(\"{name}\")";
        var callReqFalse = call.Replace(")", ", required: false)");
        throw new ArgumentException($@"Tried to get parameter with {call} but parameter '{name}' not provided. 
Either change the calling Html.Partial(""{helperSpecs.CodeFileName}"", {{ {name} = ... }} ) or use {callReqFalse} to make it optional.", nameof(name));
    }

    #endregion

    public dynamic? Code(string name, NoParamOrder npo = default, object? fallback = default, bool? required = default)
        => GetInternal(name: name, fallback: fallback, fallbackAsObj: fallback, required: required);

    #region Numbers

    public int Int(string name, NoParamOrder npo = default, int? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    public float Float(string name, NoParamOrder npo = default, float? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    public double Double(string name, NoParamOrder npo = default, double? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    public decimal Decimal(string name, NoParamOrder npo = default, decimal? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion

    #region Standard value types

    public string? String(string name, NoParamOrder npo = default, string? fallback = default, bool? required = default) 
        => GetInternal(name, fallback, fallbackAsObj: fallback, required: required);

    public Guid Guid(string name, NoParamOrder npo = default, Guid? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    public bool Bool(string name, NoParamOrder npo = default, bool? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    public DateTime DateTime(string name, NoParamOrder npo = default, DateTime? fallback = default, bool? required = default) 
        => GetInternal(name, fallback ?? default, fallbackAsObj: fallback, required: required);

    #endregion


    //#region Stacks

    //public ITypedStack Stack(string name, NoParamOrder npo = default, ITypedStack fallback = default, bool? required = default) 
    //    => _converter.Stack(GetInternal(name, required, noParamOrder), fallback);

    //#endregion

    #region Adam

    public IFile? File(string name, NoParamOrder npo = default, IFile? fallback = default, bool? required = default) 
        => _converter.File(GetInternalObj(name, required), fallback);

    public IEnumerable<IFile>? Files(string name, NoParamOrder npo = default, IEnumerable<IFile>? fallback = default, bool? required = default) 
        => _converter.Files(GetInternalObj(name, required), fallback);

    public IFolder? Folder(string name, NoParamOrder npo = default, IFolder? fallback = default, bool? required = default)
        => _converter.Folder(GetInternalObj(name, required), fallback);

    public IEnumerable<IFolder>? Folders(string name, NoParamOrder npo = default, IEnumerable<IFolder>? fallback = default, bool? required = default)
        => _converter.Folders(GetInternalObj(name, required), fallback);

    public GpsCoordinates? Gps(string name, NoParamOrder npo = default, GpsCoordinates? fallback = default, bool? required = default)
        => GetInternal(name: name, fallback: fallback, fallbackAsObj: fallback, required: required);

    #endregion

    #region Entity and Item(s)

    public ITypedItem? Item(string name, NoParamOrder npo = default, ITypedItem? fallback = default, bool? required = default)
        => _converter.Item(GetInternalObj(name, required), npo, fallback);

    public IEnumerable<ITypedItem>? Items(string name, NoParamOrder npo = default, IEnumerable<ITypedItem>? fallback = default, bool? required = default)
        => _converter.Items(GetInternalObj(name, required), npo, fallback);

    #endregion

    #region HtmlTags

    public IHtmlTag? HtmlTag(string name, NoParamOrder npo = default, IHtmlTag? fallback = default, bool? required = default)
        => _converter.HtmlTag(GetInternalObj(name, required), fallback);

    public IEnumerable<IHtmlTag>? HtmlTags(string name, NoParamOrder npo = default, IEnumerable<IHtmlTag>? fallback = default, bool? required = default)
        => _converter.HtmlTags(GetInternalObj(name, required), fallback);


    #endregion

    #region Toolbar

    public IToolbarBuilder? Toolbar(string name, NoParamOrder npo = default, IToolbarBuilder? fallback = default, bool? required = default)
        => _converter.Toolbar(GetInternalObj(name, required), fallback);

    #endregion

}