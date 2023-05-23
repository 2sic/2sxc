using System;
using ToSic.Eav.Data;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Data
{
    [PrivateApi("WIP v16")]
    public partial class TypedEntity : Wrapper<IEntity>, ITypedEntity
    {
        // ReSharper disable once InconsistentNaming
        protected readonly DynamicEntity.MyServices _Services;

        internal TypedEntity(IEntity baseEntity, DynamicEntity.MyServices dynamicEntityServices) : base(baseEntity)
        {
            _Services = dynamicEntityServices;
            Entity = baseEntity;
        }
        internal TypedEntity(IDynamicEntity dynEntity) : this(dynEntity.Entity, dynEntity._Services)
        {
            _dynamicEntity = dynEntity;
        }

        public int EntityId => Entity.EntityId;

        public Guid EntityGuid => Entity.EntityGuid;

        [PrivateApi]
        IEntity ICanBeEntity.Entity => Entity.Entity;
        protected readonly IEntity Entity;

        public dynamic Dyn => DynEntity;
        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        protected IDynamicEntity DynEntity => _dynamicEntity ?? (_dynamicEntity = new DynamicEntity(Entity, _Services));
        private IDynamicEntity _dynamicEntity;

        public IDynamicField Field(string name) => new DynamicField(Dyn, name);

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        public ITypedEntity Presentation => _presentation.Get(() =>
        {
            var dynPres = DynEntity.Presentation;
            return dynPres == null ? null : new TypedEntity(dynPres);
        });
        private readonly GetOnce<ITypedEntity> _presentation = new GetOnce<ITypedEntity>();
    }
}
