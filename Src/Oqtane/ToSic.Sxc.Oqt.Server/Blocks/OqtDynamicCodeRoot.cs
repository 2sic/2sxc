using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
public class OqtDynamicCodeRoot : OqtDynamicCodeRoot<object, ServiceKit>
{
    //private readonly LazySvc<SiteStateInitializer> _siteStateInitializerLazy;
    public OqtDynamicCodeRoot(MyServices services, LazySvc<SiteStateInitializer> siteStateInitializerLazy) : base(services, siteStateInitializerLazy)
    {
        //ConnectServices(
        //    _siteStateInitializerLazy = siteStateInitializerLazy
        //);
    }

    //public override IDynamicCodeRoot InitDynCodeRoot(IBlock block, ILog parentLog, int compatibility = Constants.CompatibilityLevel12)
    //{
    //    _siteStateInitializerLazy.Value.InitIfEmpty(block?.Context?.Site?.Id);
    //    return base.InitDynCodeRoot(block, parentLog, compatibility);
    //}
}