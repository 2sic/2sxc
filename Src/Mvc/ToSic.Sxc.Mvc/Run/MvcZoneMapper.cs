using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcZoneMapper : ZoneMapperBase
    {
        /// <inheritdoc />
        public MvcZoneMapper(IServiceProvider serviceProvider) : base("Mvc.ZoneMp")
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;

        public override int GetZoneId(int tenantId) => tenantId;

        
        public override ISite SiteOfZone(int zoneId) => _serviceProvider.Build<ISite>().Init(zoneId);


        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            return new List<TempTempCulture>
            {
                new TempTempCulture("en-us", "English USA", true)
            };
        }

    }
}
