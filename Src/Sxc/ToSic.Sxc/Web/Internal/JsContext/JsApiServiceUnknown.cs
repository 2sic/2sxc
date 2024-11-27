using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Web.Internal.JsContext;

internal class JsApiServiceUnknown(WarnUseOfUnknown<JsApiServiceUnknown> _) : ServiceBase($"{LogScopes.NotImplemented}.JsApi"), IJsApiService, IIsUnknown
{
    public string GetJsApiJson(int? pageId, string siteRoot = null, string rvt = null, bool withPublicKey = false) => null;

    public JsApi GetJsApi(int? pageId, string siteRoot, string rvt, bool withPublicKey = false) => null;
}