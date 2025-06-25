using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

interface IPageServiceSharedInternal: IPageServiceShared
{
    CspOfModule Csp { get; }

}