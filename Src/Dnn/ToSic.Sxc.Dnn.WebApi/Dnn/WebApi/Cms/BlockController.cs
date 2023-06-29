using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Apps.Ui;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules(DnnSupportedModuleNames)]
    public class BlockController : SxcApiControllerBase<BlockControllerReal>, IBlockController
    {
        public BlockController() : base(BlockControllerReal.LogSuffix) { }

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
            => SysHlp.Real.Block(parentId, field, index, app, guid);


        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Item([FromUri] int? index = null) => SysHlp.Real.Item(index);


        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void App(int? appId) => SysHlp.Real.App(appId);

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> Apps(string apps = null) => SysHlp.Real.Apps(apps);


        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => SysHlp.Real.ContentTypes();


        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> Templates() => SysHlp.Real.Templates();

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? Template(int templateId, bool forceCreateContentGroup) => SysHlp.Real.Template(templateId, forceCreateContentGroup);


        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public AjaxRenderDto Render([FromUri] int templateId, [FromUri] string lang, [FromUri] string edition) 
            => SysHlp.Real.Set(DnnConstants.SysFolderRootVirtual.Trim('~')).Render(templateId, lang, edition);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index) => SysHlp.Real.Publish(part, index);
    }
}