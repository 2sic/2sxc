using System;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService: HasLog, 
            // Important: Write with namespace, because it's easy to confuse with IPageService it supports
            ToSic.Sxc.Services.IPageService, 
            INeedsDynamicCodeRoot,
#pragma warning disable CS0618
            // Important: Write with namespace, because it's easy to confuse with IPageService it supports
            ToSic.Sxc.Web.IPageService    // Keep for compatibility with some Apps released in v12
#pragma warning restore CS0618
    {

        public PageService(PageServiceShared pageServiceShared, Lazy<ContentSecurityPolicyService> cspServiceLazy) : base("2sxc.PgeSrv")
        {
            _cspServiceLazy = cspServiceLazy;
            PageServiceShared = pageServiceShared;
        }
        private readonly Lazy<ContentSecurityPolicyService> _cspServiceLazy;
        public PageServiceShared PageServiceShared { get; }

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
            Log.LinkTo(codeRoot?.Log);
            Log.Fn(message: $"Linked {nameof(PageService)}").Done();
        }

        public IDynamicCodeRoot CodeRoot;

        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [PrivateApi("not final yet, will probably change")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;


        public bool CspIsEnabled => _cspServiceLazy.Value.IsEnabled;

        public bool CspIsEnforced => _cspServiceLazy.Value.IsEnforced;

        public string AddCsp(string name, params string[] values)
        {
            _cspServiceLazy.Value.Add(name, values);
            return "";
        }
    }
}
