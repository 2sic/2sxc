using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ItemListActions;
using ToSic.Eav.Data;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ModuleController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string GenerateContentBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            Log.Add($"get CB parent:{parentId}, field:{field}, order:{sortOrder}, app:{app}, guid:{guid}");
            var contentTypeName = Settings.AttributeSetStaticNameContentBlockTypeName;
            var values = new Dictionary<string, object>
            {
                {BlockFromEntity.CbPropertyTitle, ""},
                {BlockFromEntity.CbPropertyApp, app},
                // 2020-08-14 #2146 2dm believe unused
                //{BlockFromEntity.CbPropertyShowChooser, true},
            };
            var newGuid = guid ?? Guid.NewGuid();
            var entityId = CreateItemAndAddToList(parentId, field, sortOrder, contentTypeName, values, newGuid);

            // now return a rendered instance
            var newContentBlock = new BlockFromEntity(BlockBuilder.Block, entityId, Log);
            return newContentBlock.BlockBuilder.Render().ToString();

        }

        private int CreateItemAndAddToList(int parentId, string field, int sortOrder, string contentTypeName,
            Dictionary<string, object> values, Guid newGuid)
        {
            var cgApp = BlockBuilder.App;

            // create the new entity 
            var entityId = new AppManager(cgApp, Log).Entities.GetOrCreate(newGuid, contentTypeName, values);

            #region attach to the current list of items

            var cbEnt = BlockBuilder.App.Data.List.One(parentId);
            var blockList = ((IEnumerable<IEntity>)cbEnt.GetBestValue(field))?.ToList() ?? new List<IEntity>();

            var intList = blockList.Select(b => b.EntityId).ToList();
            // add only if it's not already in the list (could happen if http requests are run again)
            if (!intList.Contains(entityId))
            {
                if (sortOrder > intList.Count) sortOrder = intList.Count;
                intList.Insert(sortOrder, entityId);
            }
            var updateDic = new Dictionary<string, object> { { field, intList } };
            new AppManager(cgApp, Log).Entities.UpdateParts(cbEnt.EntityId, updateDic);
            #endregion

            return entityId;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void MoveItemInList(int parentId, string field, int indexFrom, int indexTo, [FromUri] bool partOfPage = false)
        {
            Log.Add($"move item in list parent:{parentId}, field:{field}, from:{indexFrom}, to:{indexTo}, partOfpage:{partOfPage}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) => new AppManager(BlockBuilder.App, Log)
                .Entities.ModifyItemList(parentId, field, new Move(indexFrom, indexTo));

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
            else InternalSave(null);
        }

        /// <summary>
        /// Delete a content-block inside a list of content-blocks
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <param name="partOfPage"></param>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveItemInList(int parentId, string field, int index, [FromUri] bool partOfPage = false)
        {
            Log.Add($"remove item: parent{parentId}, field:{field}, index:{index}, partOfPage{partOfPage}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) => new AppManager(BlockBuilder.App, Log)
                .Entities.ModifyItemList(parentId, field, new Remove(index));

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
            else InternalSave(null);
        }
    }
}
