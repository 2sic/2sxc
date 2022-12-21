using System;
using Oqtane.Infrastructure;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtPlatformContext: Platform, IPlatformInfo
    {
        public OqtPlatformContext(ILazySvc<IConfigManager> configManager) => _configManager = configManager;
        private readonly ILazySvc<IConfigManager> _configManager;

        public override PlatformType Type => PlatformType.Oqtane;

        public override Version Version => new(Oqtane.Shared.Constants.Version);

        string IPlatformInfo.Identity => _configManager.Value.GetInstallationId();
    }
}
