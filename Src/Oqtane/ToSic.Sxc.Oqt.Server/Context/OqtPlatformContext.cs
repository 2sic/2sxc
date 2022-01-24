using System;
using Oqtane.Infrastructure;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtPlatformContext: Platform, IPlatformInfo
    {
        public OqtPlatformContext(Lazy<IConfigManager> configManager) => _configManager = configManager;
        private readonly Lazy<IConfigManager> _configManager;

        public override PlatformType Type => PlatformType.Oqtane;

        public override Version Version => new(Oqtane.Shared.Constants.Version);

        string IPlatformInfo.Identity => _configManager.Value.GetInstallationId();
    }
}
