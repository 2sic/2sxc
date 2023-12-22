using ToSic.Eav.Internal.Unknown;
using ToSic.Eav.Run;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsApiServiceUnknown : ServiceBase, IJsApiService, IIsUnknown
{
    public JsApiServiceUnknown(WarnUseOfUnknown<BasicEnvironmentInstaller> _) : base($"{LogScopes.NotImplemented}.JsApi")
    { }
    public string GetJsApiJson(int? pageId, string siteRoot, string rvt) => null;
    public JsApi GetJsApi(int? pageId, string siteRoot, string rvt) => null;
}