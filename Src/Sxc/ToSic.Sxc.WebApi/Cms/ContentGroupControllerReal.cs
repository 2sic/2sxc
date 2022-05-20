using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.ItemLists;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Cms
{
    public class ContentGroupControllerReal: HasLog<ContentGroupControllerReal>, IContentGroupController
    {
        public const string LogSuffix = "CntGrp";
        public ContentGroupControllerReal(LazyInitLog<IPagePublishing> publishing, GeneratorLog<IPagePublishing> versioning, LazyInit<CmsManager> cmsManagerLazy, IContextResolver ctxResolver) : base("Api.CntGrpRl")
        {
            CtxResolver = ctxResolver;
            _cmsManagerLazy = cmsManagerLazy.SetInit(m => m.Init(Context, Log));
            _publishing = publishing.SetLog(Log);
            _versioning = versioning.SetLog(Log);
        }

        public IContextResolver CtxResolver { get; }

        #region Constructor / di
        private readonly LazyInit<CmsManager> _cmsManagerLazy;
        private readonly LazyInitLog<IPagePublishing> _publishing;
        private readonly GeneratorLog<IPagePublishing> _versioning;
        private CmsManager CmsManager => _cmsManagerLazy.Ready;


        private IContextOfBlock Context => _context ?? (_context = CtxResolver.BlockRequired());
        private IContextOfBlock _context;

        #endregion

        public EntityInListDto Header(Guid guid)
        {
            Log.A($"header for:{guid}");
            var cg = GetContentGroup(guid, false);

            // new in v11 - this call might be run on a non-content-block, in which case we return null
            if (cg.Entity == null) return null;
            if (cg.Entity.Type.Name != BlocksRuntime.BlockTypeName) return null;

            var header = cg.Header.FirstOrDefault();

            return new EntityInListDto
            {
                Index = 0,
                Id = header?.EntityId ?? 0,
                Guid = header?.EntityGuid ?? Guid.Empty,
                Title = header?.GetBestTitle() ?? "",
                Type = header?.Type.NameId ?? cg.View.HeaderType
            };
        }


        // TODO: probably should move to a Manager
        public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
        {
            var wrapLog = Log.Call($"target:{guid}, part:{part}, index:{index}, id:{entityId}");

            void InternalSave(VersioningActionInfo args)
            {
                var entity = CmsManager.AppState.List.One(guid);
                if (entity == null) throw new Exception($"Can't find item '{guid}'");

                // correct casing of content / listcontent for now - TODO should already happen in JS-Call
                if (entity.Type.Name == BlocksRuntime.BlockTypeName)
                {
                    if (string.Equals(part, ViewParts.Content, OrdinalIgnoreCase)) part = ViewParts.Content;
                    if (string.Equals(part, ViewParts.FieldHeader, OrdinalIgnoreCase)) part = ViewParts.FieldHeader;
                }

                var forceDraft = Context.Publishing.ForceDraft;
                if (add)
                    CmsManager.Entities.FieldListAdd(entity, new[] { part }, index, new int?[] { entityId }, forceDraft);
                else
                    CmsManager.Entities.FieldListReplaceIfModified(entity, new[] { part }, index, new int?[] { entityId },
                        forceDraft);
            }

            // use dnn versioning - this is always part of page
            _versioning.New.DoInsidePublishing(Context, InternalSave);
            wrapLog(null);
        }


        // TODO: WIP changing this from ContentGroup editing to any list editing
        public ReplacementListDto Replace(Guid guid, string part, int index)
        {
            var wrapLog = Log.Fn<ReplacementListDto>($"target:{guid}, part:{part}, index:{index}");
            part = part.ToLowerInvariant();

            var itemList = FindContentGroupAndTypeName(guid, part, out var typeName)
                           ?? FindItemAndFieldTypeName(guid, part, out typeName);

            // if no type was defined in this set, then return an empty list as there is nothing to choose from
            if (string.IsNullOrEmpty(typeName))
                return null;

            var appState = Context.AppState;// State.Get(_app);
            var ct = appState.GetContentType(typeName);

            var listTemp = CmsManager.Read.Entities.Get(typeName);

            var results = listTemp.ToDictionary(p => p.EntityId,
                p => p.GetBestTitle() ?? "");

            // if list is empty or shorter than index (would happen in an add-to-end-request) return null
            var selectedId = itemList.Count > index
                ? itemList[index]?.EntityId
                : null;

            var result = new ReplacementListDto { SelectedId = selectedId, Items = results, ContentTypeName = ct.NameId };
            return wrapLog.Return(result);
        }


        public List<EntityInListDto> ItemList(Guid guid, string part)
        {
            Log.A($"item list for:{guid}");
            var cg = Context.AppState.List.One(guid);
            var itemList = cg.Children(part);

            var list = itemList.Select((c, index) => new EntityInListDto
            {
                Index = index,
                Id = c?.EntityId ?? 0,
                Guid = c?.EntityGuid ?? Guid.Empty,
                Title = c?.GetBestTitle() ?? "",
            }).ToList();

            return list;
        }


        // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
        public bool ItemList(/*IContextOfBlock context,*/ Guid guid, List<EntityInListDto> list,  string part = null)
        {
            Log.A($"list for:{guid}, items:{list?.Count}");
            if (list == null) throw new ArgumentNullException(nameof(list));

            _publishing.Ready.DoInsidePublishing(Context, args =>
            {
                //var cms = new CmsManager().Init(_app, Log);
                var entity = CmsManager.Read.AppState.List.One(guid);

                var sequence = list.Select(i => i.Index).ToArray();
                var fields = part == ViewParts.ContentLower ? ViewParts.ContentPair : new[] {part};
                CmsManager.Entities.FieldListReorder(entity, fields, sequence, Context.Publishing.ForceDraft);
            });

            return true;
        }


        private List<IEntity> FindItemAndFieldTypeName(Guid guid, string part, out string attributeSetName)
        {
            var parent = Context.AppState.List.One(guid);
            if (parent == null) throw new Exception($"No item found for {guid}");
            if (!parent.Attributes.ContainsKey(part)) throw new Exception($"Could not find field {part} in item {guid}");
            var itemList = parent.Children(part);

            // find attribute-type-name
            var attribute = parent.Type[part];
            if (attribute == null) throw new Exception($"Attribute definition for '{part}' not found on the item {guid}");
            attributeSetName = attribute.EntityFieldItemTypePrimary();
            return itemList;
        }

        private List<IEntity> FindContentGroupAndTypeName(Guid guid, string part, out string attributeSetName)
        {
            var wrapLog = Log.Fn<List<IEntity>>($"{guid}, {part}");
            var contentGroup = GetContentGroup(guid, true);
            attributeSetName = null;
            var partIsContent = string.Equals(part, ViewParts.ContentLower, OrdinalIgnoreCase);
            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
            var itemList = partIsContent ? contentGroup.Content : contentGroup.Header;

            if (itemList == null) return wrapLog.ReturnNull();

            // not sure what this check is for, just leaving it in for now (2015-09-19 2dm)
            if (contentGroup.View == null)
                return wrapLog.ReturnNull("Something found, but doesn't seem to be a content-group. Cancel.");

            attributeSetName = partIsContent ? contentGroup.View.ContentType : contentGroup.View.HeaderType;
            return wrapLog.Return(itemList);
        }

        private BlockConfiguration GetContentGroup(Guid contentGroupGuid, bool throwIfNotFound)
        {
            Log.A($"get group:{contentGroupGuid}");
            var contentGroup = CmsManager.Read.Blocks.GetBlockConfig(contentGroupGuid);

            // Note 2022-05-05 2dm - this can never be null, the check should be updated to check if the .Entity is null if this is important
            if (contentGroup?.Entity == null && throwIfNotFound)
                throw new Exception("BlockConfiguration with Guid " + contentGroupGuid + " does not exist.");
            return contentGroup;
        }

    }
}
