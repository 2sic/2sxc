using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps.Work
{
    public class WorkBlocksMod : WorkUnitBase<IAppWorkCtxWithDb>
    {
        private readonly AppWork _appWork;

        public WorkBlocksMod(AppWork appWork) : base("AWk.EntCre")
        {
            ConnectServices(
                _appWork = appWork
            );
        }


        public Guid UpdateOrCreateContentGroup(BlockConfiguration blockConfiguration, int templateId)
        {
            var l = Log.Fn<Guid>();

            // #ExtractEntitySave - context

            if (!blockConfiguration.Exists)
            {
                l.A($"doesn't exist, will create new CG with template#{templateId}");
                // #ExtractEntitySave - verified
                var guid = _appWork.EntityCreate(AppWorkCtx).Create(WorkBlocks.BlockTypeName, new Dictionary<string, object>
                {
                    {ViewParts.TemplateContentType, new List<int> {templateId}},
                    {ViewParts.Content, new List<int>()},
                    {ViewParts.Presentation, new List<int>()},
                    {ViewParts.FieldHeader, new List<int>()},
                    {ViewParts.FieldHeaderPresentation, new List<int>()}
                }).EntityGuid; // new guid
                return l.ReturnAndLog(guid, "created");
            }

            l.A($"exists, create for group#{blockConfiguration.Guid} with template#{templateId}");
            // #ExtractEntitySave - verified
            _appWork.EntityUpdate(AppWorkCtx).UpdateParts(blockConfiguration.Entity.EntityId,
                new Dictionary<string, object> { { ViewParts.TemplateContentType, new List<int?> { templateId } } });

            return l.ReturnAndLog(blockConfiguration.Guid); // guid didn't change
        }

        public void AddEmptyItem(BlockConfiguration block, int? index, bool forceDraft)
        {
            _appWork.EntityFieldList(appState: AppWorkCtx.AppState)
                .FieldListUpdate(block.Entity, ViewParts.ContentPair, forceDraft,
                lists =>
                {
                    // hitting (+) if the list is empty add two demo items (because we already see one demo item)
                    if (lists.Lists.First().Value.Count == 0) // on non, add 2 null items
                        lists.Add(0, new int?[] { null, null });
                    return lists.Add(index, new int?[] { null, null });
                });
        }


        public int NewBlockReference(int parentId, string field, int index, string app = "", Guid? guid = null)
        {
            var l = Log.Fn<int>($"get CB parent:{parentId}, field:{field}, order:{index}, app:{app}, guid:{guid}");
            var contentTypeName = AppConstants.ContentGroupRefTypeName;
            var values = new Dictionary<string, object>
            {
                {BlockFromEntity.CbPropertyTitle, ""},
                {BlockFromEntity.CbPropertyApp, app},
            };
            var newGuid = guid ?? Guid.NewGuid();
            var entityId = CreateItemAndAddToList(parentId, field, index, contentTypeName, values, newGuid);

            return l.ReturnAndLog(entityId);
        }

        private int CreateItemAndAddToList(int parentId, string field, int index, string typeName, Dictionary<string, object> values, Guid newGuid)
        {
            var l = Log.Fn<int>($"{nameof(parentId)}:{parentId}, {nameof(field)}:{field}, {nameof(index)}, {index}, {nameof(typeName)}:{typeName}");
            // #ExtractEntitySave - context
            var workCtx = _appWork.CtxWithDb(AppWorkCtx.AppState);

            // create the new entity 
            // #ExtractEntitySave - should be ok
            var entityId = _appWork.EntityCreate(workCtx).GetOrCreate(newGuid, typeName, values);

            #region attach to the current list of items

            var cbEnt = AppWorkCtx.AppState.List.One(parentId);
            var blockList = cbEnt.Children(field);

            var intList = blockList.Select(b => b.EntityId).ToList();
            // add only if it's not already in the list (could happen if http requests are run again)
            if (!intList.Contains(entityId))
            {
                if (index > intList.Count) index = intList.Count;
                intList.Insert(index, entityId);
            }
            var updateDic = new Dictionary<string, object> { { field, intList } };
            // #ExtractEntitySave - should be ok
            _appWork.EntityUpdate(workCtx).UpdateParts(cbEnt.EntityId, updateDic);

            #endregion

            return l.ReturnAndLog(entityId);
        }

    }
}
