using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.OqtaneModule.Server.Repository;
using ToSic.Sxc.OqtaneModule.Server.Run;
using ToSic.Sxc.OqtaneModule.Shared.Dev;

namespace ToSic.Sxc.OqtaneModule.Server.Controllers
{
    public abstract class SxcStatefulControllerBase: SxcStatelessControllerBase
    {
        private readonly SxcOqtane _sxcOqtane;

        private readonly ITenantResolver _tenantResolver;

        //private readonly ITenantResolver _tenantResolver;
        private readonly OqtaneZoneMapper _zoneMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAliasRepository _aliasRepository;

        protected SxcStatefulControllerBase(SxcOqtane sxcOqtane, IZoneMapper zoneMapper, ITenantResolver tenantResolver, IUserResolver userResolver) : base(userResolver)
        {
            _sxcOqtane = sxcOqtane;
            _tenantResolver = tenantResolver;
            _zoneMapper = zoneMapper as OqtaneZoneMapper;
        }

        protected IInstanceContext GetContext()
        {
            var alias = _tenantResolver.GetAlias();
            var context = new InstanceContext(_zoneMapper.TenantOfSite(alias.SiteId), WipConstants.NullPage, WipConstants.NullContainer, GetUser());
            return context;
        }

        protected IBlock GetBlock(bool allowNoContextFound = true)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var containerId = GetTypedHeader(WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentblockId = GetTypedHeader(WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            var pageId = GetTypedHeader(WebApi.WebApiConstants.HeaderPageId, -1);
            var instance = TestIds.FindInstance(containerId);

            if (containerId == -1 || pageId == -1 || instance == null)
            {
                if (allowNoContextFound) return wrapLog("not found", null);
                throw new Exception("No context found, cannot continue");
            }

            var ctx = _sxcOqtane.CreateContext(HttpContext, instance.Zone, pageId, containerId, instance.App, instance.Block);
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
