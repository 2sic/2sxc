using System.Net;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.Context.Internal;

partial record Parameters: ITyped
{
    [PrivateApi]
    bool ITyped.ContainsKey(string name)
        => OriginalsAsDic.ContainsKey(name);

    [PrivateApi]
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => !OriginalsAsDic.TryGetValue(name, out var result) || HasKeysHelper.IsEmpty(result, default);

    [PrivateApi]
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => OriginalsAsDic.TryGetValue(name, out var result) && HasKeysHelper.IsNotEmpty(result, default);

    [PrivateApi]
    IEnumerable<string> ITyped.Keys(NoParamOrder noParamOrder, IEnumerable<string> only)
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, OriginalsAsDic?.Keys);
    [PrivateApi]
    IEnumerable<string> IHasKeys.Keys(NoParamOrder noParamOrder, IEnumerable<string> only)
        => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, OriginalsAsDic?.Keys);


    [PrivateApi]
    object ITyped.Get(string name, NoParamOrder noParamOrder, bool? required, string language /* ignore */)
        => TryGetAndLog(name, out var value) ? value : null;

    [PrivateApi]
    bool ITyped.Bool(string name, NoParamOrder noParamOrder, bool fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    DateTime ITyped.DateTime(string name, NoParamOrder noParamOrder, DateTime fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    string ITyped.String(string name, NoParamOrder noParamOrder, string fallback, bool? required, object scrubHtml)
    {
        var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
        if (scrubHtml != default)
            throw new NotSupportedException($"{nameof(scrubHtml)} is not supported on this object");
        return value;
    }

    [PrivateApi]
    int ITyped.Int(string name, NoParamOrder noParamOrder, int fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    long ITyped.Long(string name, NoParamOrder noParamOrder, long fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    float ITyped.Float(string name, NoParamOrder noParamOrder, float fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    decimal ITyped.Decimal(string name, NoParamOrder noParamOrder, decimal fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    double ITyped.Double(string name, NoParamOrder noParamOrder, double fallback, bool? required)
        => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

    [PrivateApi]
    string ITyped.Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        var url = GetV(name, noParamOrder: noParamOrder, fallback);
        return Tags.SafeUrl(url).ToString();
    }

    [PrivateApi]
    IRawHtmlString ITyped.Attribute(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        // Note: we won't do special date processing, since all values in the Parameters are strings
        var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
        return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
    }
}