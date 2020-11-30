using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Context
{
    public interface IContextOfAppData
    {
        void InitApp(IAppIdentity appIdentity, ILog parentLog);

        bool EditAllowed { get; }
    }
}
