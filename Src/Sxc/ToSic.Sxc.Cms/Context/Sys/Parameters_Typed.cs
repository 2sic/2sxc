using System.Net;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Context.Sys;

partial record Parameters: ITyped
{
    [PrivateApi]
    bool ITyped.ContainsKey(string name)
        => OriginalsAsDic.ContainsKey(name);

    [PrivateApi]
    public bool IsEmpty(string name, NoParamOrder npo = default, string? language = default)
        => !OriginalsAsDic.TryGetValue(name, out var result) || HasKeysHelper.IsEmpty(result, default);

    [PrivateApi]
    public bool IsNotEmpty(string name, NoParamOrder npo = default, string? language = default)
        => OriginalsAsDic.TryGetValue(name, out var result) && HasKeysHelper.IsNotEmpty(result, default);

    [PrivateApi]
    IEnumerable<string> ITyped.Keys(NoParamOrder npo, IEnumerable<string>? only)
        => TypedHelpers.FilterKeysIfPossible(npo, only, OriginalsAsDic?.Keys);
    [PrivateApi]
    IEnumerable<string> IHasKeys.Keys(NoParamOrder npo, IEnumerable<string>? only)
        => TypedHelpers.FilterKeysIfPossible(npo, only, OriginalsAsDic?.Keys);


    [PrivateApi]
    object? ITyped.Get(string name, NoParamOrder npo, bool? required, string? language /* ignore */)
        => TryGetAndLog(name, out var value) ? value : null;

    [PrivateApi]
    bool ITyped.Bool(string name, NoParamOrder npo, bool fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    DateTime ITyped.DateTime(string name, NoParamOrder npo, DateTime fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    string? ITyped.String(string name, NoParamOrder npo, string? fallback, bool? required, object? scrubHtml)
    {
        var value = GetV(name, npo: npo, fallback: fallback);
        if (scrubHtml != default)
            throw new NotSupportedException($"{nameof(scrubHtml)} is not supported on this object");
        return value;
    }

    [PrivateApi]
    int ITyped.Int(string name, NoParamOrder npo, int fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    long ITyped.Long(string name, NoParamOrder npo, long fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    float ITyped.Float(string name, NoParamOrder npo, float fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    decimal ITyped.Decimal(string name, NoParamOrder npo, decimal fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    double ITyped.Double(string name, NoParamOrder npo, double fallback, bool? required)
        => GetV(name, npo: npo, fallback: fallback);

    [PrivateApi]
    string ITyped.Url(string name, NoParamOrder npo, string? fallback, bool? required)
    {
        var url = GetV(name, npo: npo, fallback);
        return Tags.SafeUrl(url).ToString();
    }

    [PrivateApi]
    IRawHtmlString? ITyped.Attribute(string name, NoParamOrder npo, string? fallback, bool? required)
    {
        // Note: we won't do special date processing, since all values in the Parameters are strings
        var value = GetV(name, npo: npo, fallback: fallback);
        return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
    }
}