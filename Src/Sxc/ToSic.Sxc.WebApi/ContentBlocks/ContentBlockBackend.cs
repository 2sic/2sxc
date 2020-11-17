using System;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.WebApi.ContentBlocks
{
    public class ContentBlockBackend : BlockWebApiBackendBase<ContentBlockBackend>
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public ContentBlockBackend(IPagePublishing publishing, Lazy<CmsManager> cmsManagerLazy) : base(cmsManagerLazy, "Bck.FldLst") 
            => _publishing = publishing.Init(Log);

        #endregion

        // todo: probably move to CmsManager.Block
        public int NewBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
            => CmsManager.Blocks.NewBlockReference(parentId, field, sortOrder, app, guid);

        public void AddItem(int? index = null)
        {
            Log.Add($"add order:{index}");
            // use dnn versioning - this is always part of page
            _publishing.DoInsidePublishing(_context, _ 
                => CmsManager.Blocks.AddEmptyItem(_block.Configuration, index, _block.Context.Publishing.ForceDraft));
        }

        
        public bool PublishPart(string part, int index)
        {
            Log.Add($"try to publish #{index} on '{part}'");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            return BlockEditorBase.GetEditor(_block).Publish(part, index);
        }
    }
}
