using ToSic.Eav.Apps;
using ToSic.Eav.Context;
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
            IPlatform platform,
            ISite site
            ) : base("Bck.Zones")
        {
            _appStates = appStates;
            _fingerprint = fingerprint;
            _zoneMapper = zoneMapper;
            _platform = platform;
            _site = site;
        }
        private readonly IAppStates _appStates;
        private readonly IFingerprint _fingerprint;
        private readonly IZoneMapper _zoneMapper;
        private readonly IPlatform _platform;
        private readonly ISite _site;

        public SystemInfoSetDto GetSystemInfo()
        {
            var wrapLog = Log.Call<SystemInfoSetDto>($"{_site.Id}");

            var zoneId = _site.ZoneId;

            var siteStats = new SiteStatsDto
            {
                SiteId = _site.Id,
                ZoneId = _site.ZoneId, 
                Apps = _appStates.Apps(zoneId).Count,
                Languages = _zoneMapper.CulturesWithState(_site.Id, _site.ZoneId).Count, 
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
