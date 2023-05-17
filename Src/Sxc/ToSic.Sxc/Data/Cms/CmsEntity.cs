using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP")]
    public partial class CmsEntity: TypedEntity, ICmsEntity
    {
        internal CmsEntity(IEntity baseEntity, DynamicEntity.MyServices dynamicEntityServices) : base(baseEntity, dynamicEntityServices)
        {
        }
        internal CmsEntity(IDynamicEntity dynEntity) : base(dynEntity)
        {
        }

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        public new CmsEntity Presentation => _presentation.Get(() =>
        {
            var dynPres = DynEntity.Presentation;
            return dynPres == null ? null : new CmsEntity(dynPres);
        });
        private readonly GetOnce<CmsEntity> _presentation = new GetOnce<CmsEntity>();
    }
}
