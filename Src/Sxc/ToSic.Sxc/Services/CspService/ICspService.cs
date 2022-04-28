namespace ToSic.Sxc.Services
{
    public interface ICspService
    {
        /// <summary>
        /// Name of the CSP header to be added, based on the report-only aspect
        /// </summary>
        string Name { get; }

        /// <summary>
        /// You can determine if CSP should report only. 
        /// </summary>
        bool IsEnforced { get; }

        /// <summary>
        /// Tells you if CSP is enabled or not. You cannot change it.
        /// </summary>
        bool IsEnabled { get; }

        //void Activate();
        //void AddPolicy(string policy);
        //void AddPolicy(CspParameters policy);
        //void AddReport(string policy);
        //void AddReport(CspParameters policy);

        /// <summary>
        /// Add a CSP rule where you also specify the name.
        /// Usually you should use the direct name like <see cref="DefaultSrc"/> instead, but this is for cases where the API doesn't have a command for that setting. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        void Add(string name, params string[] values);


        void DefaultSrc(params string[] values);
        void ChildSrc(params string[] values);
        void ConnectSrc(params string[] values);
        void FontSrc(params string[] values);
        void FrameSrc(params string[] values);
        void ImgSrc(params string[] values);
        void ManifestSrc(params string[] values);
        void MediaSrc(params string[] values);
        void ObjectSrc(params string[] values);
        void PrefetchSrc(params string[] values);
        void ScriptSrc(params string[] values);
        void ScriptSrcElem(params string[] values);
        void ScriptSrcAttr(params string[] values);
        void StyleSrc(params string[] values);
        void StyleSrcElem(params string[] values);
        void StyleSrcAttr(params string[] values);
        void WorkerSrc(params string[] values);
        void BaseUri(params string[] values);
        void FormAction(params string[] values);
        void FrameAncestors(params string[] values);
        void NavigateTo(params string[] values);
        void ReportTo(params string[] values);
        void RequireTrustedTypesFor(params string[] values);
        void Sandbox(params string[] values);
        void TrustedTypes(params string[] values);
        void UpgradeInsecureRequests();
    }
}