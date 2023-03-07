using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
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
        public DynamicEntity(IEntity entity, MyServices services): base(services)
        {
            SetEntity(entity);

            // WIP new in 12.03
            _ListHelper = new DynamicEntityListHelper(this, () => Debug, services);
        }

        internal DynamicEntity(IEnumerable<IEntity> list, IEntity parent, string field, int? appIdOrNull, MyServices services): base(services)
        {
            // Set the entity - if there was one, or if the list is empty, create a dummy Entity so toolbars will know what to do
            SetEntity(list.FirstOrDefault() ?? PlaceHolder(appIdOrNull, parent, field));
            _ListHelper = new DynamicEntityListHelper(list, parent, field, () => Debug, services);
        }

        private IEntity PlaceHolder(int? appIdOrNull, IEntity parent, string field)
        {
            var dummyEntity = _Services.DataBuilder.FakeEntity(appIdOrNull ?? parent.AppId);
            return parent == null 
                ? dummyEntity 
                : EntityInBlockDecorator.Wrap(dummyEntity, parent.EntityGuid, field);
        }


        [PrivateApi]
        protected void SetEntity(IEntity entity)
        {
            Entity = entity;
            var entAsWrapper = Entity as IEntityWrapper;
            RootContentsForEqualityCheck = entAsWrapper?.RootContentsForEqualityCheck ?? Entity;
            Decorators = entAsWrapper?.Decorators ?? new List<IDecorator<IEntity>>();
        }

        // ReSharper disable once InconsistentNaming
        internal readonly DynamicEntityListHelper _ListHelper;


        // ReSharper disable once InheritdocInvalidUsage
        /// <inheritdoc />
        public string EntityTitle => Entity?.GetBestTitle(_Services.Dimensions);


        // ReSharper disable once InheritdocInvalidUsage
        /// <inheritdoc />
        public bool IsDemoItem => _isDemoItem ?? (_isDemoItem = Entity?.GetDecorator<EntityInBlockDecorator>()?.IsDemoItem ?? false).Value;
        private bool? _isDemoItem;

        [PrivateApi("Not in use yet, and I believe not communicated")]
        public bool IsFake => _isFake ?? (_isFake = (Entity?.EntityId ?? DataConstants.DataFactoryDefaultEntityId) == DataConstants.DataFactoryDefaultEntityId).Value;
        private bool? _isFake;
    }
}