#if NETFRAMEWORK
using System.Collections.Specialized;
using System.Web;

namespace ToSic.Sxc.Web.Internal.DotNet;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HttpNetFramework: HttpAbstractionBase, IHttp
{
    /// <summary>
    /// Empty constructor for DI
    /// </summary>
    public HttpNetFramework() => Current = HttpContext.Current;

    #region Request and properties thereof

    public override NameValueCollection QueryStringParams
    {
        get
        {
            if (_queryStringValues != null) return _queryStringValues;
            return _queryStringValues = Request == null
                ? []
                : Request.QueryString;
        }
    }
    private NameValueCollection _queryStringValues;
    #endregion Request

}
#endif