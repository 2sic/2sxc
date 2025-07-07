using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtExecutionContext(
    ExecutionContext.Dependencies services,
    LazySvc<AliasResolver> aliasResolverLazy)
    : OqtExecutionContext<object, ServiceKit>(services, aliasResolverLazy);