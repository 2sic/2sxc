using Oqtane.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Context.Sys.Platform;
using ToSic.Sys.Capabilities.Platform;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtPlatformContext(LazySvc<IConfigManager> configManager) : Platform, IPlatformInfo
{
    public override PlatformType Type => PlatformType.Oqtane;

    public override Version Version => new(Oqtane.Shared.Constants.Version);

    string IPlatformInfo.Identity => configManager.Value.GetInstallationId();
}