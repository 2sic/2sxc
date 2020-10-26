using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.Wip;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Razor.Debug;

namespace ToSic.Sxc.Oqt.Server
{
    public class SxcOqtane: HasLog, IRenderTestIds
    {
        #region Constructor and DI
        
        public SxcOqtane(IHttpContextAccessor httpContextAccessor, OqtAssetsAndHeaders moduleAssets, RazorReferenceManager debugRefMan) : base("Mvc.View")
        {
            AssetsAndHeaders = moduleAssets;
            _debugRefMan = debugRefMan;
            _httpContext = httpContextAccessor.HttpContext;
            // add log to history!
            History.Add("sxc-mvc-view", Log);
        }

        private readonly HttpContext _httpContext;
        public IOqtAssetsAndHeader AssetsAndHeaders { get; }
        private readonly RazorReferenceManager _debugRefMan;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known. 
        /// </summary>
        public void Prepare(Site site, Oqtane.Models.Page page, Module module)
        {
            if (_renderDone) throw new Exception("already prepared this module");
            GeneratedHtml = RenderModule(site, page, module);
            _renderDone = true;
        }

        private bool _renderDone;
        public MarkupString GeneratedHtml { get; private set; }

        #endregion

        public MarkupString RenderModule(Site site, Oqtane.Models.Page page, Module module)
        {
            if (module.ModuleId == 27)
                return RenderHtml(TestIds.Token);
            if (module.ModuleId == 28 // test site 2dm, root site
                || module.ModuleId == 1049 // test sub-site 2dm "blogsite"
                || module.ModuleId == 56 // test sub-site Tonci "blogsite"
                )
                return RenderHtml(TestIds.Blog);
            return (MarkupString) $"Error - module id {module} not found";
        }

        private MarkupString RenderHtml(InstanceId id) => (MarkupString)RenderString(id);

        public string Test()
        {
            return _debugRefMan.CompilationReferences.Count.ToString();
        }

        private string RenderString(InstanceId id)
        {
            var block = CreateBlock(id.Zone, id.Page, id.Container, id.App, id.Block, Log);

            var result = block.BlockBuilder.Render();

            // todo: set parameters for loading scripts etc.
            //PageProperties.Headers += "hello!!!";

            return result;
        }

        public IBlock CreateBlock(int zoneId, int pageId, int containerId, int appId, Guid blockGuid, ILog log)
        {
            var context = CreateContext(_httpContext, zoneId, pageId, containerId, appId, blockGuid);
            var block = new BlockFromModule().Init(context, log);
            return block;
        }

        public InstanceContext CreateContext(HttpContext http, int zoneId, int pageId, int containerId, int appId,
            Guid blockGuid)
            => new InstanceContext(
                new WipTenant(http).Init(zoneId), 
                //_zoneMapper.TenantOfZone(zoneId),
                new OqtanePage(pageId, null),
                new OqtaneContainer(tenantId: zoneId, id: containerId, appId: appId, block: blockGuid),
                new OqtaneUser(WipConstants.NullUser)
            );
    }
}
