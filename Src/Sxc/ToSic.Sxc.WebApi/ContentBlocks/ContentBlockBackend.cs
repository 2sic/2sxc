using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.Url;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.ContentBlocks
{
    public class ContentBlockBackend : BlockWebApiBackendBase<ContentBlockBackend>
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public ContentBlockBackend(IServiceProvider sp, IPagePublishing publishing, Lazy<CmsManager> cmsManagerLazy, IContextResolver ctxResolver)
            : base(sp, cmsManagerLazy, ctxResolver, "Bck.FldLst")
        {
            _publishing = publishing.Init(Log);
        }

        #endregion


        public RenderResultWIP NewBlockAndRender(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
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

        public static AjaxRenderDto RenderV2(RenderResultWIP result, string root)
        {
            var resources = new List<AjaxResourceDtoWIP>();
            var ver = Settings.Version.ToString();
            if (result.Features.Contains(BuiltInFeatures.TurnOn))
                resources.Add(new AjaxResourceDtoWIP { Url = UrlHelpers.QuickAddUrlParameter(root + InpageCms.TurnOnJs, "v", ver) });

            resources.AddRange(result.Assets.Select(asset => new AjaxResourceDtoWIP { Url = UrlHelpers.QuickAddUrlParameter(asset.Url, "v", ver), Type = asset.IsJs ? "js" : "css" }));
            return new AjaxRenderDto
            {
                Html = result.Html,
                Resources = resources
            };

        }
    }
}
