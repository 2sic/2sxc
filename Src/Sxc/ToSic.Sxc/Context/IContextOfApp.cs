using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context
{
    public interface IContextOfApp: IContextOfSite
    {
        void ResetApp(IAppIdentity appIdentity);

        void ResetApp(int appId);

        AppState AppState { get; }
    }
}
