using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.PageServiceShared.Internal;

interface IPageServiceSharedInternal: IPageServiceShared
{
    CspOfModule Csp { get; }

}