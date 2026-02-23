using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtExecutionContext<TModel, TServiceKit> : ExecutionContext<TModel, TServiceKit> where TServiceKit : ServiceKit where TModel : class
{
    private readonly LazySvc<AliasResolver> _aliasResolverLazy;
    public OqtExecutionContext(Dependencies services, LazySvc<AliasResolver> aliasResolverLazy) : base(services, OqtConstants.OqtLogPrefix)
    {
        ConnectLogs([
            _aliasResolverLazy = aliasResolverLazy
        ]);
    }

    public override IExecutionContext Setup(ExecutionContextOptions options)
    {
        _aliasResolverLazy.Value.InitIfEmpty(options.BlockOrNull?.Context?.Site?.Id);
        return base.Setup(options);
    }
}