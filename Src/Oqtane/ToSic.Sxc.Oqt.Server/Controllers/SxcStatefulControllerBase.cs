using System;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using ToSic.Eav;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class SxcStatefulControllerBase: SxcStatelessControllerBase
    {

        protected SxcStatefulControllerBase(StatefulControllerDependencies dependencies) : base(dependencies.UserResolver)
        {
            ServiceProvider = dependencies.ServiceProvider;
            _moduleRepository = dependencies.ModuleRepository;
            _oqtTempInstanceContext = dependencies.OqtTempInstanceContext;
        }
        private readonly IModuleRepository _moduleRepository;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;
        protected readonly IServiceProvider ServiceProvider;

        protected IContextOfBlock GetContext()
        {
            var block = GetBlock();
            if (block != null) return block.Context;

            throw new Exception("todo - must figure out - should always have a block state...?");

            //var alias = _tenantResolver.GetAlias();
            //var context = new InstanceContext(_zoneMapper.TenantOfSite(alias.SiteId), WipConstServer.NullPage, WipConstServer.NullContainer, GetUser(), ServiceProvider);
            //return context;
        }

        protected IBlock GetBlock(bool allowNoContextFound = true)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var containerId = GetTypedHeader(WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentblockId = GetTypedHeader(WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            var pageId = GetTypedHeader(WebApi.WebApiConstants.HeaderPageId, -1);
            //var instance = TestIds.FindInstance(containerId);

            if (containerId == -1 || pageId == -1)
            {
                if (allowNoContextFound) return wrapLog("not found", null);
                throw new Exception("No context found, cannot continue");
            }

            var module = _moduleRepository.GetModule(containerId);
            var ctx = _oqtTempInstanceContext.CreateContext(module, pageId, Log, ServiceProvider);
            IBlock block = ServiceProvider.Build<BlockFromModule>().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            if (contentblockId > 0) return wrapLog("found", block);

            Log.Add($"Inner Content: {contentblockId}");
            block = ServiceProvider.Build<BlockFromEntity>().Init(block, contentblockId, Log);
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
