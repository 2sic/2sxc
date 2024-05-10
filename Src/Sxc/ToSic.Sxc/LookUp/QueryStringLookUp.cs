using System.Collections.Specialized;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// LookUp provider for query string parameters.
/// It handles the normal `key=value` query string parameters and also the special `OriginalParameters` query string parameter.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class QueryStringLookUp(LazySvc<IHttp> httpLazy) : LookUpBase(LookUpConstants.SourceQueryString, "LookUp in QueryString")
{
    private NameValueCollection _source;
    private NameValueCollection _originalParams;


    public override string Get(string key, string format)
    {
        _source ??= httpLazy.Value?.QueryStringParams ?? [];

        // Special handling when having original parameters in query string.
        var originalParametersQueryStringValue = _source[OriginalParameters.NameInUrlForOriginalParameters];
        var overrideParam = GetOverrideParam(key, originalParametersQueryStringValue);

        return !string.IsNullOrEmpty(overrideParam) ? overrideParam : _source[key];
    }

    private string GetOverrideParam(string key, string originalParametersQueryStringValue)
    {
        if (string.IsNullOrEmpty(originalParametersQueryStringValue)) return string.Empty;

        _originalParams ??= OriginalParameters.GetOverrideParams(originalParametersQueryStringValue);

        return _originalParams[key];
    }
}