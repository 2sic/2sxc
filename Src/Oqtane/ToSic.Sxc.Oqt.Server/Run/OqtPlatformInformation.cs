using Oqtane.Infrastructure;
using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    [PrivateApi("Hide implementation")]
    public class OqtPlatformInformation: PlatformInformationBase
    {
        public OqtPlatformInformation(IConfigManager configManager) => _configManager = configManager;

        private readonly IConfigManager _configManager;

        public override string Name => "Oqt";

        public override Version Version => new(Oqtane.Shared.Constants.Version);

        public override string Identity => _configManager.GetInstallationId();
    }
}
