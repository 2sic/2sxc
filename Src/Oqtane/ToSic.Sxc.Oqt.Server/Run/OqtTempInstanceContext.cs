using System;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Oqt.Server.Wip;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    internal class OqtTempInstanceContext
    {
        public OqtTempInstanceContext()
        {

        }

        public static InstanceContext CreateContext(HttpContext http, int zoneId, int pageId, int containerId, int appId,
            Guid blockGuid)
            => new InstanceContext(
                new WipTenant(http).Init(zoneId),
                //_zoneMapper.TenantOfZone(zoneId),
                new OqtanePage(pageId, null),
                new OqtaneContainer(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
                new OqtaneUser(WipConstants.NullUser)
            );

    }
}
