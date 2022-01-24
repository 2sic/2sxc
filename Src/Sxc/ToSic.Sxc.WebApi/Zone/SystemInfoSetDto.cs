using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.Zone
{
    public class SystemInfoSetDto
    {
        public SiteStatsDto Site { get; internal set; }

        public SystemInfoDto System { get; internal set; }

        public LicenseInfoDto License { get; internal set; }
    }

    public class SystemInfoDto
    {
        public string Fingerprint { get; internal set; }

        public string EavVersion { get; internal set; }

        public string Platform { get; internal set; }

        public string PlatformVersion { get; internal set; }

        public int Zones { get; internal set; }
    }

    public class LicenseInfoDto
    {
        public string Main { get; internal set; }

        public int Count { get; internal set; }
    }
    

    public class SiteStatsDto
    {
        public int SiteId { get; internal set; }

        public int ZoneId { get; internal set; }

        public int Apps { get; internal set; }

        public int Languages { get; internal set; }
    }
}
