using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtCodeApiService(
    CodeApiService.MyServices services,
    LazySvc<SiteStateInitializer> siteStateInitializerLazy)
    : OqtCodeApiService<object, ServiceKit>(services, siteStateInitializerLazy);