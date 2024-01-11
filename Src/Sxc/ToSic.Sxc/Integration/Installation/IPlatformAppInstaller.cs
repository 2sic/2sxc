using ToSic.Eav.Context;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Integration.Installation;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPlatformAppInstaller : IHasLog
{
    string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
}