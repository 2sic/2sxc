using ToSic.Lib.Services;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// Very experimental, do not use
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ContentSecurityPolicyServiceBase() : ServiceBase($"{SxcLogName}.CspSvc"), IContentSecurityPolicyService
{
    public CspParameters Policy = [];

    public virtual bool IsEnforced => false;

    public virtual bool IsEnabled => false;

    public void Add(string name, params string[] values)
    {
        if (values == null || values.Length == 0)
            values = [null];

        // Split values, so that each value is standalone - in case future merging requires clean-up
        var valuesSplit = values.SelectMany(v => v == null ? [null] : v.Split(' '));
        foreach (var v in valuesSplit)
            Policy.Add(name, v);
    }

    //#region Src commands

    //public void AllSrc(params string[] values) => Add(CspConstants.AllSrcName, values);

    //public void DefaultSrc(params string[] values) => Add(CspConstants.DefaultSrcName, values);
    //public void ChildSrc(params string[] values) => Add("child-src", values);
    //public void ConnectSrc(params string[] values) => Add("connect-src", values);
    //public void FontSrc(params string[] values) => Add("font-src", values);
    //public void FrameSrc(params string[] values) => Add("frame-src", values);
    //public void ImgSrc(params string[] values) => Add("img-src", values);
    //public void ManifestSrc(params string[] values) => Add("manifest-src", values);
    //public void MediaSrc(params string[] values) => Add("media-src", values);
    //public void ObjectSrc(params string[] values) => Add("object-src", values);
    //public void PrefetchSrc(params string[] values) => Add("prefetch-src", values);
    //public void ScriptSrc(params string[] values) => Add("script-src", values);
    //public void ScriptSrcElem(params string[] values) => Add("script-src-elem", values);
    //public void ScriptSrcAttr(params string[] values) => Add("script-src-attr", values);
    //public void StyleSrc(params string[] values) => Add("style-src", values);
    //public void StyleSrcElem(params string[] values) => Add("style-src-elem", values);
    //public void StyleSrcAttr(params string[] values) => Add("style-src-attr", values);
    //public void WorkerSrc(params string[] values) => Add("worker-src", values);
    //#endregion

    //#region Other Commands
    //public void BaseUri(params string[] values) => Add("base-uri", values);
    //// Deprecated according to https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/block-all-mixed-content
    ////public void BlockAllMixedContent(params string[] values) => Add("block-all-mixed-content", values);
    //public void FormAction(params string[] values) => Add("form-action", values);
    //public void FrameAncestors(params string[] values) => Add("frame-ancestors", values);
    //public void NavigateTo(params string[] values) => Add("navigate-to", values);
    //// Deprecated https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/plugin-types
    ////public void plugin-types(params string[] values) => Add("plugin-types", values);
    //// Deprecated https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/referrer
    ////public void referrer(params string[] values) => Add("referrer", values);
    //public void ReportTo(params string[] values) => Add("report-to", values);
    //// Deprecated https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/report-uri
    ////public void report-uri(params string[] values) => Add("report-uri", values);

    //// Deprecated https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/require-sri-for
    ////public void require-sri-for(params string[] values) => Add("require-sri-for", values);

    //public void RequireTrustedTypesFor(params string[] values) => Add("require-trusted-types-for", values);
    //public void Sandbox(params string[] values) => Add("sandbox", values);
    //public void TrustedTypes(params string[] values) => Add("trusted-types", values);

    //public void UpgradeInsecureRequests() => Add("upgrade-insecure-requests");

    //#endregion
        
}