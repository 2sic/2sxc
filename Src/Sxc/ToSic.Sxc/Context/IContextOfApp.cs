using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context
{
    public interface IContextOfApp: IContextOfSite
    {
        void ResetApp(IAppIdentity appIdentity);

        bool EditAllowed { get; }

        AppState AppState { get; }
    }
}
