using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// Very experimental, do not use
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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