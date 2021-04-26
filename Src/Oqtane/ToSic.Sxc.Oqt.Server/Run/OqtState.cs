using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtState
    {
        public Func<HttpRequest> GetRequest { get; }
        public ILog Log { get; }
        public IServiceProvider ServiceProvider { get; }
        private IModuleRepository _moduleRepository;
        private OqtTempInstanceContext _oqtTempInstanceContext;
        private IBlock _block;

        public OqtState(Func<HttpRequest> getRequest, IServiceProvider serviceProvider, ILog log)
        {
            GetRequest = getRequest;
            ServiceProvider = serviceProvider;
            Log = log;

            InitServices();
        }

        private void InitServices()
        {
            _moduleRepository = ServiceProvider.Build<IModuleRepository>();
            _oqtTempInstanceContext = ServiceProvider.Build<OqtTempInstanceContext>();
        }

        public IContextOfSite GetSiteContext() => ServiceProvider.Build<IContextOfSite>();

        public IContextOfApp GetAppContext(int appId)
        {
            // First get a normal basic context which is initialized with site, etc.
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        public IContextOfBlock GetContext() => GetBlock()?.Context ?? ServiceProvider.Build<IContextOfBlock>().Init(Log) as IContextOfBlock;

        public IBlock GetBlock(bool allowNoContextFound = true) => _block ??= InitializeBlock(allowNoContextFound);

        private IBlock InitializeBlock(bool allowNoContextFound)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var moduleId = GetTypedHeader(WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentBlockId = GetTypedHeader(WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            var pageId = GetTypedHeader(WebApi.WebApiConstants.HeaderPageId, -1);

            if (moduleId == -1 || pageId == -1)
            {
                if (allowNoContextFound) return wrapLog("not found", null);
                throw new Exception("No context found, cannot continue");
            }

            var module = _moduleRepository.GetModule(moduleId);
            var ctx = _oqtTempInstanceContext.CreateContext(pageId, module, Log);
            IBlock block = ServiceProvider.Build<BlockFromModule>().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            if (contentBlockId > 0) return wrapLog("found", block);

            Log.Add($"Inner Content: {contentBlockId}");
            block = ServiceProvider.Build<BlockFromEntity>().Init(block, contentBlockId, Log);
            return wrapLog("found", block);
        }

        private T GetTypedHeader<T>(string headerName, T fallback)
        {
            var valueString = GetRequest().Headers[headerName];
            if (valueString == StringValues.Empty) return fallback;

            try
            {
                return (T)Convert.ChangeType(valueString.ToString(), typeof(T));
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public IApp GetApp(int appId) => ServiceProvider.Build<App>().Init(ServiceProvider, appId, Log, GetBlock());
    }
}