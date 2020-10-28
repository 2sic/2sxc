using System;
using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Oqt.Server.Wip;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtTempInstanceContext
    {
        private readonly OqtaneContainer _oqtaneContainer;
        private readonly OqtaneTenantSite _oqtaneTenantSite;

        public OqtTempInstanceContext(OqtaneContainer oqtaneContainer, OqtaneTenantSite oqtaneTenantSite)
        {
            _oqtaneContainer = oqtaneContainer;
            _oqtaneTenantSite = oqtaneTenantSite;
        }

        public InstanceContext CreateContext(HttpContext http, int zoneId, int pageId, int containerId, int appId,
            Guid blockGuid)
            => new InstanceContext(
                 _oqtaneTenantSite, //new WipTenant(http).Init(zoneId),
                //_zoneMapper.TenantOfZone(zoneId),
                new OqtanePage(pageId, null),
                _oqtaneContainer.Init(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
                new OqtaneUser(WipConstants.NullUser)
            );

    }
}
