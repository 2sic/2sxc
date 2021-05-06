using System;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public partial class OqtState: HasLog<OqtState>
    {
        public OqtState(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base($"{OqtConstants.OqtLogPrefix}.State")
        {
            _httpContextAccessor = httpContextAccessor;
            ServiceProvider = serviceProvider;

            //InitServices();

            //// Default implementation
            //GetRequest = GetRequestDefault;
        }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IServiceProvider ServiceProvider { get; }

        private IModuleRepository ModuleRepository => _moduleRepository ??= ServiceProvider.Build<IModuleRepository>();
        private IModuleRepository _moduleRepository;
        //private OqtTempInstanceContext _oqtTempInstanceContext;


        //private void InitServices()
        //{
        //    _moduleRepository = ServiceProvider.Build<IModuleRepository>();
        //    //_oqtTempInstanceContext = ServiceProvider.Build<OqtTempInstanceContext>();
        //}

        // Default implementation for GetRequest().
        //private HttpRequest Request => _httpContextAccessor?.HttpContext?.Request;

        //public OqtState Init(Func<HttpRequest> getRequest)
        //{
        //    Request = getRequest; // Replace default implementation.

        //    return this;
        //}

        //public IContextOfSite GetSiteContext() => ServiceProvider.Build<IContextOfSite>();

        //public IContextOfApp GetAppContext(int appId)
        //{
        //    // First get a normal basic context which is initialized with site, etc.
        //    var appContext = ServiceProvider.Build<IContextOfApp>();
        //    appContext.Init(Log);
        //    appContext.ResetApp(appId);
        //    return appContext;
        //}

        public IContextOfBlock GetContext() => _context ??= GetBlock()?.Context ?? ServiceProvider.Build<IContextOfBlock>().Init(Log) as IContextOfBlock;
        private IContextOfBlock _context;

        public IBlock GetBlockOfModule(int pageId, Module module)
        {
            //var ctx = TempInstanceContext_CreateContext(pageId, module);
            var ctx = ServiceProvider.Build<IContextOfBlock>();
            ctx.Init(Log);
            ((OqtPage)ctx.Page).Init(pageId);
            ((OqtModule)ctx.Module).Init(module, Log);

            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            ctx.Page.ParametersInternalOld = OriginalParameters.GetOverrideParams(ctx.Page.ParametersInternalOld);
            _block = ServiceProvider.Build<BlockFromModule>().Init(ctx, Log);
            return _block;
        }


        //public IContextOfBlock TempInstanceContext_CreateContext(int pageId, Module module)
        //{
        //    var ctx = ServiceProvider.Build<IContextOfBlock>();
        //    ctx.Init(Log);
        //    ((OqtPage)ctx.Page).Init(pageId);
        //    ((OqtModule)ctx.Module).Init(module, Log);
        //    return ctx;
        //}

        public IBlock GetBlock(bool allowNoContextFound = true)
        {
            if (_block != null || _triedToGetBlock) return _block;
            _block = InitializeBlock(allowNoContextFound);
            _triedToGetBlock = true;
            return _block;
        }
        private IBlock _block;
        private bool _triedToGetBlock;

        private IBlock InitializeBlock(bool allowNoContextFound)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var moduleId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderInstanceId, -1);
            var pageId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderPageId, -1);

            if (moduleId == -1 || pageId == -1)
            {
                moduleId = GetQueryString(WebApiConstants.ModuleId, GetRouteValuesString(WebApiConstants.ModuleId, -1));
                pageId = GetQueryString(WebApiConstants.PageId,GetRouteValuesString(WebApiConstants.PageId, -1));

                if (moduleId == -1 || pageId == -1)
                {
                    if (allowNoContextFound) return wrapLog("not found", null);
                    throw new Exception("No context found, cannot continue");

                }

                Log.Add($"Found page/module {pageId}/{moduleId} in route");
                //var moduleQs = ModuleRepository.GetModule(moduleId);
                //var blockQs = GetBlockOfModule(pageId, moduleQs);
                //return wrapLog("found in route values", blockQs);
            }

            var module = ModuleRepository.GetModule(moduleId);
            var block = GetBlockOfModule(pageId, module);

            // only if it's negative, do we load the inner block
            var contentBlockId = GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            if (contentBlockId >= 0) return wrapLog("found block", block);

            Log.Add($"Inner Content: {contentBlockId}");
            block = ServiceProvider.Build<BlockFromEntity>().Init(block, contentBlockId, Log);
            return wrapLog("found inner block", block);
        }


        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public IApp GetApp(int appId) => ServiceProvider.Build<App>().Init(ServiceProvider, appId, Log, GetBlock());
    }
}