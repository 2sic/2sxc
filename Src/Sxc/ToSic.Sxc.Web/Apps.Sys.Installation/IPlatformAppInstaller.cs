﻿using ToSic.Eav.Context;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps.Sys.Installation;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IPlatformAppInstaller : IHasLog
{
    string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
}