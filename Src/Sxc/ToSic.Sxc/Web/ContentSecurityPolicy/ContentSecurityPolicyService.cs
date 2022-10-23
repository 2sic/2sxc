using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    /// <summary>
    /// Very experimental, do not use
    /// </summary>
    [PrivateApi]
    public class ContentSecurityPolicyService : ContentSecurityPolicyServiceBase, IContentSecurityPolicyService
    {
        public ContentSecurityPolicyService(PageServiceShared pageServiceShared)
        {
            _pageSvcShared = pageServiceShared;
            pageServiceShared.Csp.AddCspService(this);
        }
        private readonly PageServiceShared _pageSvcShared;

        public override bool IsEnforced => _pageSvcShared.Csp.IsEnforced;

        public override bool IsEnabled => _pageSvcShared.Csp.IsEnabled;
        
        
    }
}
