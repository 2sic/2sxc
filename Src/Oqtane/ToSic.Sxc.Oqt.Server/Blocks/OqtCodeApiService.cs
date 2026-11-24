using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtCodeContext(
    CodeContext.Dependencies services,
    LazySvc<AliasResolver> aliasResolverLazy)
    : OqtCodeContext<object, ServiceKit>(services, aliasResolverLazy);