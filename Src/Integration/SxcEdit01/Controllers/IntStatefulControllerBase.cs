using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;


namespace IntegrationSamples.SxcEdit01.Controllers
{
    public abstract class IntStatefulControllerBase: IntStatelessControllerBase
    {
        public class Dependencies
        {
            public IServiceProvider ServiceProvider { get; }
            public IContextResolver ContextResolver { get; }

            public Dependencies(IServiceProvider serviceProvider, IContextResolver contextResolver)
            {
                ServiceProvider = serviceProvider;
                ContextResolver = contextResolver;
            }
        }

        protected IntStatefulControllerBase(Dependencies dependencies) : base()
        {
            ServiceProvider = dependencies.ServiceProvider;

            dependencies.ContextResolver.AttachRealBlock(() => GetBlock());
            dependencies.ContextResolver.AttachBlockContext(GetContext);
        }
        protected readonly IServiceProvider ServiceProvider;



        protected IContextOfApp GetAppContext(int appId)
        {
            // First get a normal basic context which is initialized with site, etc.
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }



        protected IContextOfBlock GetContext() => GetBlock()?.Context ?? ServiceProvider.Build<IContextOfBlock>().Init(Log) as IContextOfBlock;

        protected IBlock GetBlock(bool allowNoContextFound = true) => _block ??= InitializeBlock();
        private IBlock _block;

        private IBlock InitializeBlock()
        {
            return null;
            //var wrapLog = Log.Call<IBlock>();

            //var moduleId = GetTypedHeader(ToSic.Sxc.WebApi.WebApiConstants.HeaderInstanceId, -1);
            //var contentBlockId =
            //    GetTypedHeader(ToSic.Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            //var pageId = GetTypedHeader(ToSic.Sxc.WebApi.WebApiConstants.HeaderPageId, -1);

            //if (moduleId == -1 || pageId == -1)
            //{
            //    if (allowNoContextFound) return wrapLog("not found", null);
            //}

            //var module = _moduleRepository.GetModule(moduleId);
            //var ctx = _oqtTempInstanceContext.CreateContext(pageId, module, Log);
            //IBlock block = ServiceProvider.Build<BlockFromModule>().Init(ctx, Log);

            //// only if it's negative, do we load the inner block
            //if (contentBlockId > 0) return wrapLog("found", block);

            //Log.Add($"Inner Content: {contentBlockId}");
            //block = ServiceProvider.Build<BlockFromEntity>().Init(block, contentBlockId, Log);
            //return wrapLog("found", block);
        }

        //private T GetTypedHeader<T>(string headerName, T fallback)
        //{
        //    var valueString = Request.Headers[headerName];
        //    if (valueString == StringValues.Empty) return fallback;

        //    try
        //    {
        //        return (T) Convert.ChangeType(valueString.ToString(), typeof(T));
        //    }
        //    catch
        //    {
        //        return fallback;
        //    }

        //}

    }
}
