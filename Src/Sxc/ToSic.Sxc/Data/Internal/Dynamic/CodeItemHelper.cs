using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Internal.Typed;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Internal.Dynamic;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeItemHelper(GetAndConvertHelper helper, ITyped data)
{
    public ITyped Data { get; } = data;
    public readonly GetAndConvertHelper Helper = helper;

    #region Keys
    public bool IsEmpty(string name, NoParamOrder noParamOrder, bool? isBlank, string language)
    {
        var result = Get(name, noParamOrder, required: false, language: language);
        return HasKeysHelper.IsEmpty(result, isBlank);
    }

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder, bool? isBlank, string language)
    {
        var result = Get(name, noParamOrder, required: false, language: language);
        return HasKeysHelper.IsNotEmpty(result, isBlank);
    }


    #endregion

    #region Get

    public object Get(string name, NoParamOrder noParamOrder, bool? required, string language = default, [CallerMemberName] string cName = default)
    {
        var findResult = Helper.TryGet(name, language);
        return IsErrStrict(findResult.Found, required, Helper.PropsRequired)
            ? throw ErrStrictForTyped(Data, name, cName: cName)
            : findResult.Result;
    }

    public TValue G4T<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default)
    {
        var findResult = Helper.TryGet(name);
        return IsErrStrict(findResult.Found, required, Helper.PropsRequired)
            ? throw ErrStrictForTyped(Data, name, cName: cName)
            : findResult.Result.ConvertOrFallback(fallback);
    }

    public TValue GetT<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, string language = default, [CallerMemberName] string cName = default)
    {
        var findResult = Helper.TryGet(field: name, language: language);
        return IsErrStrict(findResult.Found, required, Helper.PropsRequired)
            ? throw ErrStrictForTyped(Data, name, cName: cName)
            : findResult.Result.ConvertOrFallback(fallback);
    }


    #endregion

    public IRawHtmlString Attribute(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        var result = Get(name, noParamOrder, required);
        var strValue = Helper.Cdf.Services.ForCode.ForCode(result, fallback: fallback);
        return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
    }

    public string String(string name, NoParamOrder noParamOrder, string fallback, bool? required, object scrubHtml = default)
    {
        var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
        return TypedItemHelpers.MaybeScrub(value, scrubHtml, () => Helper.Cdf.Services.Scrub);
    }


    public string Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        // TODO: STRICT
        var url = Helper.GetInternal(name, lookupLink: true).Result as string;
        return Tags.SafeUrl(url).ToString();
    }

}