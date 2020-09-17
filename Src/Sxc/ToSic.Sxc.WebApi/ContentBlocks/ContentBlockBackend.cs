using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ItemListActions;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.ContentBlocks
{
    internal class ContentBlockBackend : HasLog
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public ContentBlockBackend(IPagePublishing publishing) : base("Bck.FldLst")
        {
            _publishing = publishing;
        }

        public ContentBlockBackend Init(IInstanceContext context, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _block = block;
            _context = context;
            _cmsManager = new CmsManager(_block.App, Log);
            return this;
        }
        private IInstanceContext _context;
        private IBlock _block;
        private CmsManager _cmsManager;
        #endregion

        // todo: probably move to CmsManager.Block
        public int NewBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null) 
            => _cmsManager.Blocks.NewBlockReference(parentId, field, sortOrder, app, guid);

        public void AddItem(int? index = null)
        {
            Log.Add($"add order:{index}");
            // use dnn versioning - this is always part of page
            _publishing.DoInsidePublishing(_context, args => _cmsManager.Blocks.AddEmptyItem(_block.Configuration, index));
        }


        public void MoveInList(int parentId, string field, int indexFrom, int indexTo, bool partOfPage = false)
        {
            var callLog = Log.Call($"parent:{parentId}, field:{field}, from:{indexFrom}, to:{indexTo}, partOfpage:{partOfPage}");

            void InternalSave(VersioningActionInfo _)
            {
                _cmsManager.Entities.ModifyItemList(parentId, field, new Move(indexFrom, indexTo));
            }

            // use dnn versioning if partOfPage
            if (partOfPage) _publishing.DoInsidePublishing(_context, InternalSave);
            else InternalSave(null);
            callLog("ok");
        }

        public void RemoveBlockInList(int parentId, string field, int index, bool partOfPage = false)
        {
            var callLog = Log.Call($"parent{parentId}, field:{field}, index:{index}, partOfPage{partOfPage}");
            void InternalSave(VersioningActionInfo _)
            {
                _cmsManager.Entities.ModifyItemList(parentId, field, new Remove(index));
            }

            // use dnn versioning if partOfPage
            if (partOfPage) _publishing.DoInsidePublishing(_context, InternalSave);
            else InternalSave(null);
            callLog("ok");
        }


    }
}
