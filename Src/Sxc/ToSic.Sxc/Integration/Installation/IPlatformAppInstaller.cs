using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Run;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPlatformAppInstaller : IHasLog
{
    string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
}