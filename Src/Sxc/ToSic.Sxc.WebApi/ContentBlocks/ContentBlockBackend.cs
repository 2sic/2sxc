using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks.Edit;

namespace ToSic.Sxc.WebApi.ContentBlocks
{
    internal class ContentBlockBackend : BlockWebApiBackendBase<ContentBlockBackend>
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public ContentBlockBackend(IPagePublishing publishing) : base("Bck.FldLst") 
            => _publishing = publishing.Init(Log);

        #endregion

        // todo: probably move to CmsManager.Block
        public int NewBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
            => _cmsManager.Blocks.NewBlockReference(parentId, field, sortOrder, app, guid);

        public void AddItem(int? index = null)
        {
            Log.Add($"add order:{index}");
            // use dnn versioning - this is always part of page
            _publishing.DoInsidePublishing(_context, _ => _cmsManager.Blocks.AddEmptyItem(_block.Configuration, index));
        }

        
        public bool PublishPart(string part, int index)
        {
            Log.Add($"try to publish #{index} on '{part}'");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            return BlockEditorBase.GetEditor(_block).Publish(part, index);
        }
    }
}
