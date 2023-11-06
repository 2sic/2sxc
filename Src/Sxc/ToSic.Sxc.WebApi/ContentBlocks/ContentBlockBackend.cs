using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Work;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.ClientAssets;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.Url;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.ContentBlocks
{
    public class ContentBlockBackend : BlockWebApiBackendBase
    {
        private readonly LazySvc<WorkBlocksMod> _workBlocksMod;
        private readonly LazySvc<BlockEditorSelector> _blockEditorSelectorLazy;
        private readonly Generator<BlockFromEntity> _entityBlockGenerator;

        #region constructor / DI

        public ContentBlockBackend(
            LazySvc<AppWorkSxc> appSysSxc,
            Generator<MultiPermissionsApp> multiPermissionsApp, 
            IPagePublishing publishing, 
            LazySvc<CmsManager> cmsManagerLazy,
            LazySvc<WorkBlocksMod> workBlocksMod,
            IContextResolver ctxResolver, 
            LazySvc<IBlockResourceExtractor> optimizerLazy,
            LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
            Generator<BlockFromEntity> entityBlockGenerator)
            : base(multiPermissionsApp, cmsManagerLazy, appSysSxc, ctxResolver, "Bck.FldLst")
        {
            ConnectServices(
                _optimizer = optimizerLazy,
                _workBlocksMod = workBlocksMod,
                _publishing = publishing,
                _entityBlockGenerator = entityBlockGenerator,
                _blockEditorSelectorLazy = blockEditorSelectorLazy
            );
        }

        private readonly LazySvc<IBlockResourceExtractor> _optimizer;
        private readonly IPagePublishing _publishing;


        #endregion


        public IRenderResult NewBlockAndRender(int parentId, string field, int index, string app = "", Guid? guid = null) 
        {
            var entityId = NewBlock(parentId, field, index, app, guid);

            // now return a rendered instance
            var newContentBlock = _entityBlockGenerator.New().Init(Block, null, entityId);
            return newContentBlock.BlockBuilder.Run(true, null);
        }

        // todo: probably move to CmsManager.Block
        public int NewBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
            => _workBlocksMod.Value.InitContext(AppWorkCtxDb)/* CmsManagerOfBlock.Blocks*/.NewBlockReference(parentId, field, sortOrder, app, guid);

        public void AddItem(int? index = null)
        {
            Log.A($"add order:{index}");
            // use dnn versioning - this is always part of page
            _publishing.DoInsidePublishing(ContextOfBlock, _ 
                => _workBlocksMod.Value.InitContext(AppWorkCtxDb)/*CmsManagerOfBlock.Blocks*/.AddEmptyItem(Block.Configuration, index, Block.Context.Publishing.ForceDraft));
        }

        
        public bool PublishPart(string part, int index)
        {
            Log.A($"try to publish #{index} on '{part}'");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            return _blockEditorSelectorLazy.Value.GetEditor(Block).Publish(part, index);
        }

        public AjaxRenderDto RenderForAjax(int templateId, string lang, string root, string edition)
        {
            var wrapLog = Log.Fn<AjaxRenderDto>();
            Log.A("1. Get Render result");
            var result = RenderToResult(templateId, lang, edition);

            Log.A("2.1. Build Resources");
            var resources = new List<AjaxResourceDtoWIP>();
            var ver = EavSystemInfo.VersionWithStartUpBuild;
            if (result.Features.Contains(BuiltInFeatures.TurnOn))
                resources.Add(new AjaxResourceDtoWIP
                    { Url = UrlHelpers.QuickAddUrlParameter(root.SuffixSlash() + BuiltInFeatures.TurnOn.UrlWip, "v", ver) });

            Log.A("2.2. Add JS & CSS which were stripped before");
            resources.AddRange(result.Assets.Select(asset => new AjaxResourceDtoWIP
            {
                // Note: Url can be empty if it has contents
                Url = string.IsNullOrWhiteSpace(asset.Url) ? null : UrlHelpers.QuickAddUrlParameter(asset.Url, "v", ver), 
                Type = asset.IsJs ? "js" : "css",
                Contents = asset.Content,
                Attributes = asset.HtmlAttributes,
            }));

            Log.A("3. Add manual resources (fancybox etc.)");
            // First get all the parts out of HTML, as the configuration is still stored as plain HTML
            var mergedFeatures  = string.Join("\n", result.FeaturesFromSettings.Select(mc => mc.Html));

            Log.A("4.1. Process optimizers");
            var renderResult = _optimizer.Value.Process(mergedFeatures, new ClientAssetsExtractSettings(extractAll: true));
            var rest = renderResult.Html;
            if (!string.IsNullOrWhiteSpace(rest)) 
                Log.A("Warning: Rest after extraction should be empty - not handled ATM");

            Log.A("4.2. Add more resources based on processed");
            resources.AddRange(renderResult.Assets.Select(asset => new AjaxResourceDtoWIP
            {
                Url = asset.Url,
                Type = asset.IsJs ? "js" : "css",
                Attributes = asset.HtmlAttributes,
            }));

            return wrapLog.ReturnAsOk(new AjaxRenderDto
            {
                Html = result.Html,
                Resources = resources
            });
        }

        private IRenderResult RenderToResult(int templateId, string lang, string edition)
        {
            var callLog = Log.Fn<IRenderResult>($"{nameof(templateId)}:{templateId}, {nameof(lang)}:{lang}");

            // if a preview templateId was specified, swap to that
            if (templateId > 0)
            {
                var template = AppSysSxc.Value.AppViews(AppWorkCtxPlus).Get(templateId);
                template.Edition = edition;
                Block.View = template;
            }

            var result = Block.BlockBuilder.Run(true, null);
            return callLog.ReturnAsOk(result);
        }

    }
}
