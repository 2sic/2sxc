using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.Url;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.ContentBlocks
{
    public class ContentBlockBackend : BlockWebApiBackendBase<ContentBlockBackend>
    {
        #region constructor / DI

        public ContentBlockBackend(IServiceProvider sp, 
            IPagePublishing publishing, 
            Lazy<CmsManager> cmsManagerLazy, 
            IContextResolver ctxResolver, 
            Lazy<IBlockResourceExtractor> optimizerLazy)
            : base(sp, cmsManagerLazy, ctxResolver, "Bck.FldLst")
        {
            _optimizer = optimizerLazy;
            _publishing = publishing.Init(Log);
        }

        private readonly Lazy<IBlockResourceExtractor> _optimizer;
        private readonly IPagePublishing _publishing;

        #endregion


        public RenderResult NewBlockAndRender(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
        {
            var entityId = NewBlock(parentId, field, sortOrder, app, guid);

            // now return a rendered instance
            var newContentBlock = ServiceProvider.Build<BlockFromEntity>().Init(Block, entityId, Log);
            return newContentBlock.BlockBuilder.Run(true);
        }

        // todo: probably move to CmsManager.Block
        public int NewBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
            => CmsManagerOfBlock.Blocks.NewBlockReference(parentId, field, sortOrder, app, guid);

        public void AddItem(int? index = null)
        {
            Log.Add($"add order:{index}");
            // use dnn versioning - this is always part of page
            _publishing.DoInsidePublishing(ContextOfBlock, _ 
                => CmsManagerOfBlock.Blocks.AddEmptyItem(Block.Configuration, index, Block.Context.Publishing.ForceDraft));
        }

        
        public bool PublishPart(string part, int index)
        {
            Log.Add($"try to publish #{index} on '{part}'");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            return BlockEditorBase.GetEditor(Block).Publish(part, index);
        }

        public AjaxRenderDto RenderV2(int templateId, string lang, string root)
        {
            var wrapLog = Log.Call<AjaxRenderDto>();
            Log.Add("1. Get Render result");
            var result = RenderToResult(templateId, lang);

            Log.Add("2.1. Build Resources");
            var resources = new List<AjaxResourceDtoWIP>();
            var ver = Settings.Version.ToString();
            if (result.Features.Contains(BuiltInFeatures.TurnOn))
                resources.Add(new AjaxResourceDtoWIP
                    { Url = UrlHelpers.QuickAddUrlParameter(root.SuffixSlash() + InpageCms.TurnOnJs, "v", ver) });

            Log.Add("2.2. Add JS & CSS which were stripped before");
            resources.AddRange(result.Assets.Select(asset => new AjaxResourceDtoWIP
            {
                // Note: Url can be empty if it has contents
                Url = string.IsNullOrWhiteSpace(asset.Url) ? null : UrlHelpers.QuickAddUrlParameter(asset.Url, "v", ver), 
                Type = asset.IsJs ? "js" : "css",
                Contents = asset.Content
            }));

            Log.Add("3. Add manual resources (fancybox etc.)");
            // First get all the parts out of HTML, as the configuration is still stored as plain HTML
            var mergedFeatures  = string.Join("\n", result.ManualChanges.Select(mc => mc.Html));
            var optimizer = _optimizer.Value;
            if(optimizer is BlockResourceExtractor withInternal) 
                withInternal.ExtractOnlyEnableOptimization = false;

            Log.Add("4.1. Process optimizers");
            var rest = optimizer.Process(mergedFeatures).Item1;
            if (!string.IsNullOrWhiteSpace(rest)) 
                Log.Add("Warning: Rest after extraction should be empty - not handled ATM");

            Log.Add("4.2. Add more resources based on processed");
            resources.AddRange(optimizer.Assets.Select(asset => new AjaxResourceDtoWIP
            {
                Url = asset.Url,
                Type = asset.IsJs ? "js" : "css"
            }));

            return wrapLog("ok", new AjaxRenderDto
            {
                Html = result.Html,
                Resources = resources
            });
        }

        private RenderResult RenderToResult(int templateId, string lang)
        {
            var callLog = Log.Call<RenderResult>($"{nameof(templateId)}:{templateId}, {nameof(lang)}:{lang}");
            //SetThreadCulture(lang);

            // if a preview templateId was specified, swap to that
            if (templateId > 0)
            {
                var template = CmsManagerOfBlock.Read.Views.Get(templateId);
                Block.View = template;
            }

            var result = Block.BlockBuilder.Run(true);
            return callLog("ok", result);
        }

    }
}
