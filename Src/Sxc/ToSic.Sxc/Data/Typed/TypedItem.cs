using System;
using ToSic.Eav.Data;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public partial class TypedItem : Wrapper<IEntity>, ITypedItem
    {
        // ReSharper disable once InconsistentNaming
        protected readonly DynamicEntity.MyServices _Services;

        internal TypedItem(IEntity baseEntity, DynamicEntity.MyServices services) : base(baseEntity)
        {
            _Services = services;
            Entity = baseEntity;
        }
        internal TypedItem(IDynamicEntity dynEntity) : this(dynEntity.Entity, dynEntity._Services)
        {
            _dynamicEntity = dynEntity;
        }

        public int EntityId => Entity.EntityId;

        public Guid EntityGuid => Entity.EntityGuid;

        [PrivateApi]
        IEntity ICanBeEntity.Entity => Entity;
        protected readonly IEntity Entity;

        /// <inheritdoc />
        public dynamic Dyn => DynEntity;
        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        protected IDynamicEntity DynEntity => _dynamicEntity ?? (_dynamicEntity = new DynamicEntity(Entity, _Services));
        private IDynamicEntity _dynamicEntity;

        /// <inheritdoc />
        public IDynamicField Field(string name) => new DynamicField(Dyn, name);

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        /// <inheritdoc />
        public ITypedItem Presentation => (DynEntity as ITypedItem).Presentation;
        //    _presentation.Get(() =>
        //{
        //    var dynPres = DynEntity.Presentation;
        //    return dynPres == null ? null : new TypedItem((IDynamicEntity)dynPres);
        //});
        //private readonly GetOnce<ITypedItem> _presentation = new GetOnce<ITypedItem>();

        public bool IsDemoItem => DynEntity.IsDemoItem;
    }
}
