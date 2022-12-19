using System;
using Microsoft.Extensions.Primitives;
using ToSic.Eav.Apps;
using ToSic.Lib.Logging;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Dev;


namespace ToSic.Sxc.Mvc.WebApi
{
    public abstract class SxcStatefulControllerBase: SxcStatelessControllerBase
    {
        protected IContextOfSite GetSiteContext()
        {
            return HttpContext.RequestServices.Build<IContextOfSite>();
        }


        protected IContextOfApp GetAppContext(int appId)
        {
            // First get a normal basic context which is initialized with site, etc.
            var appContext = HttpContext.RequestServices.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        protected IContextOfBlock GetContext()
        {
            //var publishing = HttpContext.RequestServices.Build<IPagePublishingResolver>();

            // in case the initial request didn't yet find a block builder, we need to create it now
            //var site = HttpContext.RequestServices.Build<ISite>().Init(TestIds.PrimaryZone);
            var ctx = HttpContext.RequestServices.Build<IContextOfBlock>();
            ctx.Init(Log);
            ctx.Site.Init(TestIds.PrimaryZone);
            //ctx.Init(new PageNull(), new ContainerNull());
            //var context = // BlockBuilder?.Context ??
            //    new ContextOfBlock(HttpContext.RequestServices, site, new MvcUser())
            //        .Init(new PageNull(), new ContainerNull(), new BlockPublishingState());
            return ctx;
        }

        protected IBlock GetBlock(bool allowNoContextFound = true)
        {
            var wrapLog = Log.Fn<IBlock>(parameters: $"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var containerId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentblockId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            var pageId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderPageId, -1);
            var instance = TestIds.FindInstance(containerId);

            if (containerId == -1 || pageId == -1 || instance == null)
            {
                if (allowNoContextFound) return wrapLog.ReturnNull("not found");
                throw new Exception("No context found, cannot continue");
            }

            var ctx = SxcMvc.CreateContext(HttpContext.RequestServices, instance.Zone, pageId, containerId, instance.App,
                instance.Block);
            IBlock block = HttpContext.RequestServices.Build<BlockFromModule>().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            if (contentblockId > 0) return wrapLog.Return(block, "found");

            Log.A($"Inner Content: {contentblockId}");
            block = HttpContext.RequestServices.Build<BlockFromEntity>().Init(block, contentblockId, Log);

            return wrapLog.Return(block, "found");
        }

        private T GetTypedHeader<T>(string headerName, T fallback)
        {
            var valueString = Request.Headers[headerName];
            if (valueString == StringValues.Empty) return fallback;

            try
            {
                return (T) Convert.ChangeType(valueString.ToString(), typeof(T));
            }
            catch
            {
                return fallback;
            }

        }
    }
}
