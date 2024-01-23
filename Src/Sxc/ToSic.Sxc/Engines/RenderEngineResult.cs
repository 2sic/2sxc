using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Internal;
using ToSic.Sxc.Web.Internal.ClientAssets;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RenderEngineResult(
    string html,
    bool activateJsApi,
    List<IClientAsset> assets,
    string errorCode = default,
    List<Exception> exsOrNull = default)
{
    public RenderEngineResult(
        RenderEngineResult original,
        string html = default,
        bool activateJsApi = default,
        List<IClientAsset> assets = default, 
        string errorCode = default,
        List<Exception> exsOrNull = default)
        : this(original?.Html ?? html,
            original?.ActivateJsApi ?? activateJsApi,
            original?.Assets ?? assets,
            original?.ErrorCode ?? errorCode,
            original?.ExceptionsOrNull ?? exsOrNull)
    {
    }

    public string Html { get; } = html;

    public bool ActivateJsApi { get; } = activateJsApi;

    public List<IClientAsset> Assets { get; } = assets ?? [];

    public string ErrorCode { get; } = errorCode;

    public List<Exception> ExceptionsOrNull { get; } = exsOrNull;
}