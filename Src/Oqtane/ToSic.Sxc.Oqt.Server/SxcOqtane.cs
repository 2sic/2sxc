using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.Wip;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Server
{
    public class SxcOqtane: HasLog, IRenderTestIds
    {
        #region Constructor and DI
        
        public SxcOqtane(IHttpContextAccessor httpContextAccessor, OqtanePageProperties pageProperties, IZoneMapper zoneMapper) : base("Mvc.View")
        {
            PageProperties = pageProperties;
            _zoneMapper = zoneMapper;
            _httpContext = httpContextAccessor.HttpContext;
            // add log to history!
            History.Add("sxc-mvc-view", Log);
        }

        private readonly HttpContext _httpContext;
        public OqtanePageProperties PageProperties;
        private readonly IZoneMapper _zoneMapper;

        #endregion

        public HtmlString Render(InstanceId id) => new HtmlString(RenderString(id));

        public MarkupString RenderHtml(InstanceId id) => (MarkupString)RenderString(id);

        public MarkupString RenderModule(Site site, Oqtane.Models.Page page, Module module)
        {
            if (module.ModuleId == 27)
                return RenderHtml(TestIds.Token);
            if (module.ModuleId == 28)
                return RenderHtml(TestIds.Blog);
            return (MarkupString) $"Error - module id {module} not found";
        }

        private string RenderString(InstanceId id)
        {
            var blockBuilder = CreateBlock(id.Zone, id.Page, id.Container, id.App, id.Block, Log);

            var result = blockBuilder.BlockBuilder.Render();

            // todo: set parameters for loading scripts etc.
            //PageProperties.Headers += "hello!!!";
            return result;
        }

        public DynamicCodeRoot CreateDynCode(InstanceId id, ILog log) =>
            new OqtaneDynamicCode().Init(CreateBlock(id.Zone, id.Page, id.Container, id.App, id.Block, log), log);


        public IBlock CreateBlock(int zoneId, int pageId, int containerId, int appId, Guid blockGuid, ILog log)
        {
            var context = CreateContext(zoneId, pageId, containerId, appId, blockGuid);
            var block = new BlockFromModule().Init(context, log);
            return block;
        }

        private InstanceContext CreateContext(int zoneId, int pageId, int containerId, int appId, Guid blockGuid)
            => CreateContext(_httpContext, zoneId, pageId, containerId, appId, blockGuid);

        public InstanceContext CreateContext(HttpContext http, int zoneId, int pageId, int containerId, int appId,
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
