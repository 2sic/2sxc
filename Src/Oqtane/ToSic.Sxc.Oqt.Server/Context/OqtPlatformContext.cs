using System;
using Oqtane.Infrastructure;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtPlatformContext(LazySvc<IConfigManager> configManager) : Platform, IPlatformInfo
{
    public override PlatformType Type => PlatformType.Oqtane;

    public override Version Version => new(Oqtane.Shared.Constants.Version);

    string IPlatformInfo.Identity => configManager.Value.GetInstallationId();
}