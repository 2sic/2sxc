using System.Collections.Specialized;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Constructor for DI
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class QueryStringLookUp(LazySvc<IHttp> httpLazy) : LookUpBase("QueryString")
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

        if (_originalParams == null)
        {
            var originalParams = new NameValueCollection { { OriginalParameters.NameInUrlForOriginalParameters, originalParametersQueryStringValue } };
            _originalParams = OriginalParameters.GetOverrideParams(originalParams);
        }

        return _originalParams[key];
    }
}