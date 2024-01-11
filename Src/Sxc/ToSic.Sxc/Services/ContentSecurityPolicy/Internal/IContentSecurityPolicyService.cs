namespace ToSic.Sxc.Services.Internal;

[PrivateApi("The service isn't publicly documented, as the functionality happens on the IPageService object")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IContentSecurityPolicyService
{
    /// <summary>
    /// You can determine if CSP should report only. You cannot change it in code.
    ///
    /// To enable, do this in the settings (Global or Site)
    /// </summary>
    bool IsEnforced { get; }

    /// <summary>
    /// Tells you if CSP is enabled or not. You cannot change it in code.
    ///
    /// To enable, do this in the settings (Global or Site)
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Add a CSP rule where you also specify the name.
    ///
    /// Example: `cspService.Add("default-src", "'self'")`
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="values"></param>
    void Add(string name, params string[] values);

    //[PrivateApi]
    //IHybridHtmlString WhitelistAttribute

    #region Specific Commands, probably not needed - will disable for now while it's not yet a public API

    //void DefaultSrc(params string[] values);
    //void ChildSrc(params string[] values);
    //void ConnectSrc(params string[] values);
    //void FontSrc(params string[] values);
    //void FrameSrc(params string[] values);
    //void ImgSrc(params string[] values);
    //void ManifestSrc(params string[] values);
    //void MediaSrc(params string[] values);
    //void ObjectSrc(params string[] values);
    //void PrefetchSrc(params string[] values);
    //void ScriptSrc(params string[] values);
    //void ScriptSrcElem(params string[] values);
    //void ScriptSrcAttr(params string[] values);
    //void StyleSrc(params string[] values);
    //void StyleSrcElem(params string[] values);
    //void StyleSrcAttr(params string[] values);
    //void WorkerSrc(params string[] values);
    //void BaseUri(params string[] values);
    //void FormAction(params string[] values);
    //void FrameAncestors(params string[] values);
    //void NavigateTo(params string[] values);
    //void ReportTo(params string[] values);
    //void RequireTrustedTypesFor(params string[] values);
    //void Sandbox(params string[] values);
    //void TrustedTypes(params string[] values);
    //void UpgradeInsecureRequests();

    #endregion
}