using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.PageServiceShared.Internal;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

/// <summary>
/// Transient CSP Service.
/// </summary>
/// <remarks>
/// Will pick up the shared page state and make sure that any activity on this is projected to the page.
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentSecurityPolicyService : ContentSecurityPolicyServiceBase, IContentSecurityPolicyService
{
    public ContentSecurityPolicyService(IPageServiceShared pageServiceShared)
    {
        PageCsp = ((IPageServiceSharedInternal)pageServiceShared).Csp;

        // Register this transient copy to the page, so it will pick up any changes made to this
        // transient instance later on when rendering
        PageCsp.AddCspService(this);
    }

    private CspOfModule PageCsp { get; }

    public override bool IsEnforced => PageCsp.IsEnforced;

    public override bool IsEnabled => PageCsp.IsEnabled;
        
        
}