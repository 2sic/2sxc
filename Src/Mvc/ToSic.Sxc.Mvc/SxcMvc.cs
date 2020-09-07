using System;
using Microsoft.AspNetCore.Html;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Mvc.Code;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.TestStuff;
using ToSic.Sxc.Mvc.Web;

namespace ToSic.Sxc.Mvc
{
    public class SxcMvc: HasLog
    {
        #region Constructor and DI
        
        public SxcMvc(MvcPageProperties pageProperties) : base("Mvc.View")
        {
            PageProperties = pageProperties;

            // add log to history!
            History.Add("sxc-mvc-view", Log);
        }

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

        public static DynamicCodeRoot CreateDynCode(InstanceId id, ILog log) =>
            new MvcDynamicCode().Init(CreateBlock(id.Zone, id.Page, id.Container, id.App, id.Block, log), log);


        public static IBlock CreateBlock(int zoneId, int pageId, int containerId, int appId, Guid blockGuid, ILog log)
        {
            var context = CreateContext(zoneId, pageId, containerId, appId, blockGuid);
            var block = new BlockFromModule().Init(context, log);
            return block;
        }

        private static InstanceContext CreateContext(int zoneId, int pageId, int containerId, int appId, Guid blockGuid)
            => new InstanceContext(
                new MvcTenant(new MvcPortalSettings(zoneId)),
                new MvcPage(pageId, null),
                new MvcContainer(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
                new MvcUser()
            );
    }
}
