using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Run
{
    public interface IPlatformAppInstaller : IHasLog
    {
        string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
    }
}
