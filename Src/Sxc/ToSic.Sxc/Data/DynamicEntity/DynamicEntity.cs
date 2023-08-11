using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Data.Typed;
using IEntity = ToSic.Eav.Data.IEntity;
using static ToSic.Eav.Parameters;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
    /// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
    /// </summary>
    [PrivateApi("Changed to private in v16.01, previously was public/stable")]
    public partial class DynamicEntity : DynamicEntityBase, IDynamicEntity, IHasMetadata, ISxcDynamicObject
    {
        #region Constructor / Setup

        [PrivateApi]
        public IEntity Entity { get; private set; }

        [PrivateApi]
        internal PreWrapEntity PreWrap { get; private set; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        [PrivateApi]
        public DynamicEntity(IEntity entity, CodeDataFactory cdf, bool strict): base(cdf, strict: strict)
        {
            SetEntity(entity);

            // WIP new in 12.03
            ListHelper = new DynamicEntityListHelper(this, () => Debug, strictGet: strict, cdf);
        }

        internal DynamicEntity(IEnumerable<IEntity> list, IEntity parent, string field, int? appIdOrNull, bool strict, CodeDataFactory cdf): base(cdf, strict: strict)
        {
            // Set the entity - if there was one, or if the list is empty, create a dummy Entity so toolbars will know what to do
            SetEntity(list.FirstOrDefault() ?? Helper.PlaceHolder(appIdOrNull, parent, field));
            ListHelper = new DynamicEntityListHelper(list, parent, field, () => Debug, strictGet: strict, cdf);
        }
        [PrivateApi]
        internal readonly DynamicEntityListHelper ListHelper;

        [PrivateApi]
        protected void SetEntity(IEntity entity)
        {
            Entity = entity;
            PreWrap = new PreWrapEntity(entity, Helper, this);
            var entAsWrapper = Entity as IEntityWrapper;
            RootContentsForEqualityCheck = entAsWrapper?.RootContentsForEqualityCheck ?? Entity;
            Decorators = entAsWrapper?.Decorators ?? new List<IDecorator<IEntity>>();
        }

        #endregion

        #region Basic Entity props: Id, Guid, Title, Type

        /// <inheritdoc />
        public int EntityId => Entity?.EntityId ?? 0;

        /// <inheritdoc />
        public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;

        /// <inheritdoc />
        public string EntityTitle => Entity?.GetBestTitle(_Cdf.Dimensions);

        /// <inheritdoc />
        public string EntityType => Entity?.Type?.Name;

        #endregion

        #region Advanced: Fields, Html

        /// <inheritdoc />
        public IField Field(string name) => ItemHelper.Field(this, name);

        /// <inheritdoc/>
        [PrivateApi("Should not be documented here, as it should only be used on ITyped")]
        public IHtmlTag Html(
            string name,
            string noParamOrder = Protector,
            object container = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool debug = default
        ) => _Cdf.CompatibilityLevel < Constants.CompatibilityLevel12
            // Only do compatibility check if used on DynamicEntity
            ? throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Razor14, RazorPro or newer.")
            : TypedItemHelpers.Html(_Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
                toolbar: toolbar, imageSettings: imageSettings, required: false, debug: debug);

        #endregion

        #region Special: IsDemoItem, IsFake

        // ReSharper disable once InheritdocInvalidUsage
        /// <inheritdoc />
        public bool IsDemoItem => _isDemoItem ?? (_isDemoItem = Entity.IsDemoItemSafe()).Value;
        private bool? _isDemoItem;

        [PrivateApi("Not in use yet, and I believe not communicated")]
        public bool IsFake => _isFake ?? (_isFake = (Entity?.EntityId ?? DataConstants.DataFactoryDefaultEntityId) == DataConstants.DataFactoryDefaultEntityId).Value;
        private bool? _isFake;

        #endregion

        #region Metadata

        /// <inheritdoc />
        public IMetadata Metadata => PreWrap.Metadata;

        /// <summary>
        /// Explicit implementation, so it's not really available on DynamicEntity, only when cast to IHasMetadata
        /// This is important, because it uses the same name "Metadata"
        /// </summary>
        [PrivateApi]
        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;


        #endregion

        #region Relationships: Presentation, Children, Parents

        /// <inheritdoc />
        public dynamic Presentation => PreWrap.Presentation;

        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field).Select(e => Helper.SubDynEntityOrNull(e)).ToList();


        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => Entity.Children(field, type)
                .Select((e, i) => EntityInBlockDecorator.Wrap(e, Entity.EntityGuid, field, i))
                .Select(e => Helper.SubDynEntityOrNull(e))
                .ToList();


        #endregion

        #region Publishing: IsPublished, GetDraft(), GetPublished()

        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        /// <inheritdoc />
        public dynamic GetDraft() => Helper.SubDynEntityOrNull(Entity == null ? null : _Cdf.BlockOrNull?.App.AppState?.GetDraft(Entity));

        /// <inheritdoc />
        public dynamic GetPublished() => Helper.SubDynEntityOrNull(Entity == null ? null : _Cdf.BlockOrNull?.App.AppState?.GetPublished(Entity));

        #endregion


        #region Any*** properties just for documentation

        public bool AnyBooleanProperty => true;
        public DateTime AnyDateTimeProperty => DateTime.Now;
        public IEnumerable<DynamicEntity> AnyChildrenProperty => null;
        public string AnyJsonProperty => null;
        public string AnyLinkOrFileProperty => null;
        public double AnyNumberProperty => 0;
        public string AnyStringProperty => null;
        public IEnumerable<DynamicEntity> AnyTitleOfAnEntityInTheList => null;

        #endregion

    }
}