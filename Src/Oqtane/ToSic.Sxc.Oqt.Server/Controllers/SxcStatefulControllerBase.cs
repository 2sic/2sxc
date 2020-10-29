using System;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class SxcStatefulControllerBase: SxcStatelessControllerBase
    {

        protected SxcStatefulControllerBase(StatefulControllerDependencies dependencies) : base(dependencies.UserResolver)
        {
            _tenantResolver = dependencies.TenantResolver;
            _zoneMapper = dependencies.ZoneMapper as OqtaneZoneMapper;
            //_moduleDefinitionRepository = dependencies.ModuleDefinitionRepository;
            _moduleRepository = dependencies.ModuleRepository;
            //_settingRepository = dependencies.SettingRepository;
            _oqtTempInstanceContext = dependencies.OqtTempInstanceContext;
        }
        private readonly ITenantResolver _tenantResolver;
        private readonly OqtaneZoneMapper _zoneMapper;
        private readonly IModuleRepository _moduleRepository;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;

        protected IInstanceContext GetContext()
        {
            var alias = _tenantResolver.GetAlias();
            var context = new InstanceContext(_zoneMapper.TenantOfSite(alias.SiteId), WipConstServer.NullPage, WipConstServer.NullContainer, GetUser());
            return context;
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
            var ctx = _oqtTempInstanceContext.CreateContext(module, pageId, Log);
            IBlock block = new BlockFromModule().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            if (contentblockId > 0) return wrapLog("found", block);

            Log.Add($"Inner Content: {contentblockId}");
            block = new BlockFromEntity().Init(block, contentblockId, Log);

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
    }
}
