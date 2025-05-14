using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Web.PageServiceShared.Internal;

interface IPageServiceSharedInternal: IPageServiceShared
{
    CspOfModule Csp { get; }

}