using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// Very experimental, do not use
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentSecurityPolicyService : ContentSecurityPolicyServiceBase, IContentSecurityPolicyService
{
    public ContentSecurityPolicyService(IPageServiceShared pageServiceShared)
    {
        _pageSvcShared = pageServiceShared;
        pageServiceShared.Csp.AddCspService(this);
    }
    private readonly IPageServiceShared _pageSvcShared;

    public override bool IsEnforced => _pageSvcShared.Csp.IsEnforced;

    public override bool IsEnabled => _pageSvcShared.Csp.IsEnabled;
        
        
}