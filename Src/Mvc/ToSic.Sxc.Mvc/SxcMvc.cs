using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.Web;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc
{
    public class SxcMvc: HasLog
    {
        #region Constructor and DI
        
        public SxcMvc(IHttpContextAccessor httpContextAccessor, MvcPageProperties pageProperties) : base("Mvc.View")
        {
            PageProperties = pageProperties;
            _httpContext = httpContextAccessor.HttpContext;
            // add log to history!
            History.Add("sxc-mvc-view", Log);
        }

        private HttpContext _httpContext;
        public MvcPageProperties PageProperties;

        #endregion

        public HtmlString Render(InstanceId id)
        {
            var blockBuilder = CreateBlock(id.Zone, id.Page, id.Container, id.App, id.Block, Log);
            
            var result = blockBuilder.BlockBuilder.Render();

            // todo: set parameters for loading scripts etc.
            //PageProperties.Headers += "hello!!!";
            return new HtmlString(result);
        }

        //public DynamicCodeRoot CreateDynCode(InstanceId id, ILog log) =>
        //    new MvcDynamicCode().Init(CreateBlock(id.Zone, id.Page, id.Container, id.App, id.Block, log), log);


        public IBlock CreateBlock(int zoneId, int pageId, int containerId, int appId, Guid blockGuid, ILog log)
        {
            var context = CreateContext(zoneId, pageId, containerId, appId, blockGuid);
            var block = _httpContext.RequestServices.Build<BlockFromModule>().Init(context, log);
            return block;
        }

        private InstanceContext CreateContext(int zoneId, int pageId, int containerId, int appId, Guid blockGuid)
            => CreateContext(_httpContext, zoneId, appId, containerId, appId, blockGuid);

        public static InstanceContext CreateContext(HttpContext http, int zoneId, int pageId, int containerId, int appId, Guid blockGuid)
        {
            var publishing = http.RequestServices.Build<IPagePublishingResolver>();

            return new InstanceContext(
                new MvcSite(http).Init(zoneId),
                new SxcPage(pageId, null, http.RequestServices.Build<IHttp>().QueryStringKeyValuePairs()),
                new MvcContainer(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
                new MvcUser(),
                http.RequestServices, 
                publishing.GetPublishingState(containerId)
            );
        }
    }
}
