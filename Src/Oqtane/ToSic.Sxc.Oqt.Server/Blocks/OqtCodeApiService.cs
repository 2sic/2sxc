using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtExecutionContext(
    ExecutionContext.MyServices services,
    LazySvc<AliasResolver> aliasResolverLazy)
    : OqtExecutionContext<object, ServiceKit>(services, aliasResolverLazy);