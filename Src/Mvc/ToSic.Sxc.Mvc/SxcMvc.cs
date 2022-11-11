using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.Web;


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

        private readonly HttpContext _httpContext;
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

        private IContextOfBlock CreateContext(int zoneId, int pageId, int containerId, int appId, Guid blockGuid)
            => CreateContext(_httpContext.RequestServices, zoneId, appId, containerId, appId, blockGuid);

        public static IContextOfBlock CreateContext(IServiceProvider sp, int zoneId, int pageId, int containerId, int appId, Guid blockGuid)
        {

            var ctx = sp.Build<IContextOfBlock>();
            ctx.Init(null);
            ctx.Site.Init(zoneId);
            ctx.Page.Init(pageId);
            ((MvcModule) ctx.Module).Init(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid);
            return ctx;

            //var publishing = sp.Build<IPagePublishingResolver>();
            //return new ContextOfBlock(
            //    sp,
            //    sp.Build<ISite>().Init(zoneId),
            //    //new SxcPage(pageId, null, http.RequestServices.Build<IHttp>().QueryStringKeyValuePairs()),
            //    //new MvcContainer(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
            //    new MvcUser()
            //    //publishing.GetPublishingState(containerId)
            //).Init(
            //    sp.Build<SxcPage>().Init(pageId),
            //    new MvcContainer(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
            //    publishing.GetPublishingState(containerId)
            //);
        }
    }
}
