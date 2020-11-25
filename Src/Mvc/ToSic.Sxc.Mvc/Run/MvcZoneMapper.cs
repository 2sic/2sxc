using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.TestStuff;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcZoneMapper : ZoneMapperBase
    {
        /// <inheritdoc />
        public MvcZoneMapper(IHttp http) : base("Mvc.ZoneMp")
        {
            _http = http;
        }
        private readonly IHttp _http;

        public override int GetZoneId(int tenantId) => tenantId;

        
        public override ISite SiteOfZone(int zoneId) => new MvcSite(_http.Current, new MvcPortalSettings(zoneId));


        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            return new List<TempTempCulture>
            {
                new TempTempCulture("en-us", "English USA", true)
            };
        }

    }
}
