using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.TestStuff;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcZoneMapper : ZoneMapperBase
    {
        /// <inheritdoc />
        public MvcZoneMapper() : base("DNN.ZoneMp")
        {
        }

        public override int GetZoneId(int tenantId) => tenantId;

        //public override int GetZoneId(ITenant tenant) => tenant.Id;

        //public override IAppIdentity IdentityFromTenant(int tenantId, int appId) => new AppIdentity(tenantId, appId);
        
        public override ITenant TenantOfZone(int zoneId) => new MvcTenant(new MvcPortalSettings(zoneId));


        //public ITenant Tenant(int zoneId) => new MvcTenant(new MvcPortalSettings());

        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            return new List<TempTempCulture>
            {
                new TempTempCulture("en-us", "English USA", true)
            };
        }

    }
}
