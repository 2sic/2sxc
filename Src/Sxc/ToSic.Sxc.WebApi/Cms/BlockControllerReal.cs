using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.Cms
{
    public class BlockControllerReal : HasLog<BlockControllerReal>, IBlockController
    {
        public const string LogSuffix = "Block";

        public BlockControllerReal(
            LazyInitLog<IContextOfSite> context,
            LazyInitLog<ContentBlockBackend> blockBackend,
            LazyInitLog<AppViewPickerBackend> viewsBackend,
            Lazy<CmsZones> cmsZones
            ): base($"{LogNames.WebApi}.{LogSuffix}Rl")
        {
            _context = context.SetLog(Log);
            _blockBackendLazy = blockBackend.SetLog(Log);
            _viewsBackendLazy = viewsBackend.SetLog(Log);
            _cmsZones = cmsZones;
        }

        private readonly LazyInitLog<IContextOfSite> _context;
        private readonly LazyInitLog<ContentBlockBackend> _blockBackendLazy;
        private readonly LazyInitLog<AppViewPickerBackend> _viewsBackendLazy;
        private readonly Lazy<CmsZones> _cmsZones;


        #region Block

        private ContentBlockBackend Backend => _backend = _backend ?? _blockBackendLazy.Ready;
        private ContentBlockBackend _backend;

        /// <inheritdoc />
        public string Block(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
            => Backend.NewBlockAndRender(parentId, field, sortOrder, app, guid).Html;
        #endregion

        #region BlockItems
        /// <summary>
        /// used to be GET Module/AddItem
        /// </summary>
        public void Item(int? index = null)
        {
            Backend.AddItem(index);
        }

        #endregion


        #region App

        /// <summary>
        /// used to be GET Module/SetAppId
        /// </summary>
        /// <param name="appId"></param>

        public void App(int? appId) => _viewsBackendLazy.Ready.SetAppId(appId);

        /// <summary>
        /// used to be GET Module/GetSelectableApps
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        public IEnumerable<AppUiInfo> Apps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = _context.Ready.Site;
            return _cmsZones.Value.Init(tenant.ZoneId, Log).AppsRt.GetSelectableApps(tenant, apps)
                .ToList();
        }

        #endregion

        #region Types

        /// <inheritdoc />
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => _viewsBackendLazy.Ready.ContentTypes();

        #endregion

        #region Templates

        /// <summary>
        /// used to be GET Module/GetSelectableTemplates
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TemplateUiInfo> Templates() => _viewsBackendLazy.Ready.Templates();

        /// <summary>
        /// Used in InPage.js
        /// used to be GET Module/SaveTemplateId
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="forceCreateContentGroup"></param>
        /// <returns></returns>
        public Guid? Template(int templateId, bool forceCreateContentGroup)
            => _viewsBackendLazy.Ready
                .SaveTemplateId(templateId, forceCreateContentGroup);

        #endregion

        /// <inheritdoc />
        public AjaxRenderDto Render(int templateId, string lang)
        {
            Log.A($"render template:{templateId}, lang:{lang}");
            return Backend.RenderV2(templateId, lang, _moduleRoot);
        }
        public BlockControllerReal Set(string moduleRoot)
        {
            _moduleRoot = moduleRoot;
            return this;
        }
        private string _moduleRoot;


        /// <inheritdoc />
        public bool Publish(string part, int index) => Backend.PublishPart(part, index);
    }
}