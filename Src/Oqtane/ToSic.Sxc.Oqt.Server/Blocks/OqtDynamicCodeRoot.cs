using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks;

[PrivateApi]
internal class OqtDynamicCodeRoot : OqtDynamicCodeRoot<object, ServiceKit>
{
    public OqtDynamicCodeRoot(MyServices services, LazySvc<SiteStateInitializer> siteStateInitializerLazy) : base(services, siteStateInitializerLazy)
    {
    }
}