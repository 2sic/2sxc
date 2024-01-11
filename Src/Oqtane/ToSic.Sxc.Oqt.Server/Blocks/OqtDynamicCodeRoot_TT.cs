using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtDynamicCodeRoot<TModel, TServiceKit> : DynamicCodeRoot<TModel, TServiceKit> where TServiceKit : ServiceKit where TModel : class
{
    private readonly LazySvc<SiteStateInitializer> _siteStateInitializerLazy;
    public OqtDynamicCodeRoot(MyServices services, LazySvc<SiteStateInitializer> siteStateInitializerLazy) : base(services, OqtConstants.OqtLogPrefix)
    {
        ConnectServices(
            _siteStateInitializerLazy = siteStateInitializerLazy
        );
    }

    public override IDynamicCodeRoot InitDynCodeRoot(IBlock block, ILog parentLog)
    {
        _siteStateInitializerLazy.Value.InitIfEmpty(block?.Context?.Site?.Id);
        return base.InitDynCodeRoot(block, parentLog);
    }
}