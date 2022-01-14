using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Zone
{
    public class ZoneBackend: HasLog<ZoneBackend>
    {
        public ZoneBackend(
            IAppStates appStates, 
            IFingerprint fingerprint,
            IZoneMapper zoneMapper,
            IPlatform platform
            ) : base("Bck.Zones")
        {
            _appStates = appStates;
            _fingerprint = fingerprint;
            _zoneMapper = zoneMapper;
            _platform = platform;
        }
        private readonly IAppStates _appStates;
        private readonly IFingerprint _fingerprint;
        private readonly IZoneMapper _zoneMapper;
        private readonly IPlatform _platform;

        public SystemInfoSetDto GetSystemInfo(int siteId)
        {
            var wrapLog = Log.Call<SystemInfoSetDto>($"{siteId}");

            var zoneId = _zoneMapper.GetZoneId(siteId);

            var siteStats = new SiteStatsDto
            {
                SiteId = siteId,
                ZoneId = zoneId, 
                Apps = _appStates.Apps(zoneId).Count,
                Languages = _zoneMapper.CulturesWithState(siteId, zoneId).Count, 
            };

            var sysInfo = new SystemInfoDto
            {
                EavVersion = Settings.ModuleVersion,
                Fingerprint = _fingerprint.GetSystemFingerprint(),
                Zones = _appStates.Zones.Count,
                Platform = _platform.Name,
                PlatformVersion = Settings.VersionToNiceFormat(_platform.Version)
            };

            var license = new LicenseInfoDto
            {
                Count = 0,
                Main = "none"
            };

            var info = new SystemInfoSetDto
            {
                License = license,
                Site = siteStats,
                System = sysInfo
            };

            return wrapLog("ok", info);
        }
    }
}
