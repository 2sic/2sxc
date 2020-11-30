using System;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatefulControllerBase: OqtStatelessControllerBase
    {

        protected OqtStatefulControllerBase(StatefulControllerDependencies dependencies) : base()
        {
            ServiceProvider = dependencies.ServiceProvider;
            _moduleRepository = dependencies.ModuleRepository;
            _oqtTempInstanceContext = dependencies.OqtTempInstanceContext;
        }
        private readonly IModuleRepository _moduleRepository;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;
        protected readonly IServiceProvider ServiceProvider;

        protected IContextOfBlock GetContext() => GetBlock()?.Context ?? ServiceProvider.Build<IContextOfBlock>();

        protected IBlock GetBlock(bool allowNoContextFound = true) => _block ??= InitializeBlock(allowNoContextFound);
        private IBlock _block;

        private IBlock InitializeBlock(bool allowNoContextFound)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var moduleId = GetTypedHeader(WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentBlockId =
                GetTypedHeader(WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
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

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        internal IApp GetApp(int appId)
        {
            return ServiceProvider.Build<App>().Init(ServiceProvider, appId, Log, GetBlock());
        }
    }
}
