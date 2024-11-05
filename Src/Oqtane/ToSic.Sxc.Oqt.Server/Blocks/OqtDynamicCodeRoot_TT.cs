using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtCodeApiService<TModel, TServiceKit> : CodeApiService<TModel, TServiceKit> where TServiceKit : ServiceKit where TModel : class
{
    private readonly LazySvc<AliasResolver> _aliasResolverLazy;
    public OqtCodeApiService(MyServices services, LazySvc<AliasResolver> aliasResolverLazy) : base(services, OqtConstants.OqtLogPrefix)
    {
        ConnectServices(
            _aliasResolverLazy = aliasResolverLazy
        );
    }

    public override ICodeApiService InitDynCodeRoot(IBlock block, ILog parentLog)
    {
        _aliasResolverLazy.Value.InitIfEmpty(block?.Context?.Site?.Id);
        return base.InitDynCodeRoot(block, parentLog);
    }
}