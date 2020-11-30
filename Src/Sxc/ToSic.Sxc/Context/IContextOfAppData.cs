using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Context
{
    public interface IContextOfAppData: IContextOfSite
    {
        void InitApp(IAppIdentity appIdentity, ILog parentLog);

        bool EditAllowed { get; }
    }
}
