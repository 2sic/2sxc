using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
    /// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public partial class DynamicEntity : DynamicEntityBase, IDynamicEntity, ISxcDynamicObject
    {
        [PrivateApi]
        public IEntity Entity { get; private set; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        [PrivateApi]
        public DynamicEntity(IEntity entity, DynamicEntityDependencies dependencies): base(dependencies)
        {
            SetEntity(entity);

            // WIP new in 12.03
            _ListHelper = new DynamicEntityListHelper(this, () => _debug, dependencies);
        }

        internal DynamicEntity(IEnumerable<IEntity> list, IEntity parent, string field, int? appIdOrNull, DynamicEntityDependencies dependencies): base(dependencies)
        {
            // Set the entity - if there was one, or if the list is empty, create a dummy Entity so toolbars will know what to do
            SetEntity(list.FirstOrDefault() ?? PlaceHolder(appIdOrNull, parent, field));
            _ListHelper = new DynamicEntityListHelper(list, parent, field, () => _debug, dependencies);
        }

        private IEntity PlaceHolder(int? appIdOrNull, IEntity parent, string field)
        {
            var dummyEntity = _Dependencies.DataBuilder.FakeEntity(appIdOrNull ?? parent.AppId);
            return parent == null 
                ? dummyEntity 
                : EntityInBlockDecorator.Wrap(dummyEntity, parent.EntityGuid, field);
        }


        [PrivateApi]
        protected void SetEntity(IEntity entity)
        {
            Entity = entity;
            var entAsWrapper = Entity as IEntityWrapper;
            EntityForEqualityCheck = entAsWrapper?.EntityForEqualityCheck ?? Entity;
            Decorators = entAsWrapper?.Decorators ?? new List<IDecorator<IEntity>>();
        }

        // ReSharper disable once InconsistentNaming
        internal readonly DynamicEntityListHelper _ListHelper;


        // ReSharper disable once InheritdocInvalidUsage
        /// <inheritdoc />
        public object EntityTitle => Entity?.Title[_Dependencies.Dimensions];


        // ReSharper disable once InheritdocInvalidUsage
        /// <inheritdoc />
        public bool IsDemoItem => _isDemoItem ?? (_isDemoItem = Entity?.GetDecorator<EntityInBlockDecorator>()?.IsDemoItem ?? false).Value;
        private bool? _isDemoItem;

        public bool IsFake => _isFake ?? (_isFake = (Entity?.EntityId ?? DataBuilder.DefaultEntityId) == DataBuilder.DefaultEntityId).Value;
        private bool? _isFake;
    }
}