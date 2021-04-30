using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtState: HasLog
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Func<HttpRequest> GetRequest { get; private set; }
        public IServiceProvider ServiceProvider { get; }
        private IModuleRepository _moduleRepository;
        private OqtTempInstanceContext _oqtTempInstanceContext;
        private IBlock _block;

        public OqtState(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base($"{OqtConstants.OqtLogPrefix}.State")
        {
            _httpContextAccessor = httpContextAccessor;
            ServiceProvider = serviceProvider;

            InitServices();

            // Default implementation
            GetRequest = GetRequestDefault;
        }

        private void InitServices()
        {
            _moduleRepository = ServiceProvider.Build<IModuleRepository>();
            _oqtTempInstanceContext = ServiceProvider.Build<OqtTempInstanceContext>();
        }

        // Default implementation for GetRequest().
        private HttpRequest GetRequestDefault() => _httpContextAccessor?.HttpContext?.Request;

        public OqtState Init(Func<HttpRequest> getRequest)
        {
            GetRequest = getRequest; // Replace default implementation.

            return this;
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

        public IContextOfBlock GetContext() => _context ??= GetBlock()?.Context ?? ServiceProvider.Build<IContextOfBlock>().Init(Log) as IContextOfBlock;
        private IContextOfBlock _context;

        public IBlock GetBlock(int pageId, Oqtane.Models.Module module, ILog log)
        {
            var ctx = _oqtTempInstanceContext.CreateContext(pageId, module, log);
            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            ctx.Page.ParametersInternalOld = OriginalParameters.GetOverrideParams(ctx.Page.ParametersInternalOld);
            var block = ServiceProvider.Build<BlockFromModule>().Init(ctx, log);
            return block;
        }

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

            IBlock block = GetBlock(pageId, module, Log);

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