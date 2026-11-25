using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtCodeContext<TModel, TServiceKit> : CodeContext<TModel, TServiceKit> where TServiceKit : ServiceKit where TModel : class
{
    private readonly LazySvc<AliasResolver> _aliasResolverLazy;
    public OqtCodeContext(Dependencies services, LazySvc<AliasResolver> aliasResolverLazy) : base(services, OqtConstants.OqtLogPrefix)
    {
        ConnectLogs([
            _aliasResolverLazy = aliasResolverLazy
        ]);
    }

    public override ICodeContext Setup(CodeContextOptions options)
    {
        _aliasResolverLazy.Value.InitIfEmpty(options.BlockOrNull?.Context?.Site?.Id);
        return base.Setup(options);
    }
}
