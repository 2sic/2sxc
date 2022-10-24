using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.ItemLists;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Cms
{
    public class ContentGroupControllerReal: HasLog<ContentGroupControllerReal>, IContentGroupController
    {
        #region Constructor / di
        public const string LogSuffix = "CntGrp";
        public ContentGroupControllerReal(
            LazyInitLog<IPagePublishing> publishing, 
            LazyInit<CmsManager> cmsManagerLazy, 
            IContextResolver ctxResolver, 
            LazyInitLog<ListControllerReal> listController) : base("Api.CntGrpRl")
        {
            CtxResolver = ctxResolver;
            _cmsManagerLazy = cmsManagerLazy.SetInit(m => m.Init(Context, Log));
            _publishing = publishing.SetLog(Log);
            _listController = listController.SetLog(Log);
        }

        public IContextResolver CtxResolver { get; }

        private readonly LazyInitLog<ListControllerReal> _listController;
        private readonly LazyInit<CmsManager> _cmsManagerLazy;
        private readonly LazyInitLog<IPagePublishing> _publishing;
        private CmsManager CmsManager => _cmsManagerLazy.Ready;


        private IContextOfBlock Context => _context ?? (_context = CtxResolver.BlockRequired());
        private IContextOfBlock _context;

        #endregion

        public EntityInListDto Header(Guid guid)
        {
            Log.A($"header for:{guid}");
            var cg = CmsManager.Read.Blocks.GetBlockConfig(guid);

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
        

        public void Replace(Guid guid, string part, int index, int entityId, bool add = false) 
            => _listController.Ready.Replace(guid, part, index, entityId, add);


        /// <summary>
        /// Special Replace just like list-replace, but with content type name coming from View definition
        /// </summary>
        public ReplacementListDto Replace(Guid guid, string part, int index)
        {
            var wrapLog = Log.Fn<ReplacementListDto>($"target:{guid}, part:{part}, index:{index}");
            var typeNameOfField = FindTypeNameOnContentGroup(guid, part);
            var result = _listController.Ready.BuildReplaceList(guid, part, index, typeNameOfField);
            return wrapLog.Return(result);
        }


        private string FindTypeNameOnContentGroup(Guid guid, string part)
        {
            var wrapLog = Log.Fn<string>($"{guid}, {part}");

            var contentGroup = CmsManager.Read.Blocks.GetBlockConfig(guid);
            if (contentGroup?.Entity == null || contentGroup.View == null)
                return wrapLog.ReturnNull("Doesn't seem to be a content-group. Cancel.");

            var typeNameForField = string.Equals(part, ViewParts.ContentLower, OrdinalIgnoreCase)
                ? contentGroup.View.ContentType
                : contentGroup.View.HeaderType;

            return wrapLog.Return(typeNameForField);
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
        public bool ItemList(Guid guid, List<EntityInListDto> list,  string part = null)
        {
            Log.A($"list for:{guid}, items:{list?.Count}");
            if (list == null) throw new ArgumentNullException(nameof(list));

            _publishing.Ready.DoInsidePublishing(Context, args =>
            {
                var entity = CmsManager.Read.AppState.List.One(guid);

                var sequence = list.Select(i => i.Index).ToArray();
                var fields = part == ViewParts.ContentLower ? ViewParts.ContentPair : new[] {part};
                CmsManager.Entities.FieldListReorder(entity, fields, sequence, Context.Publishing.ForceDraft);
            });

            return true;
        }


    }
}
