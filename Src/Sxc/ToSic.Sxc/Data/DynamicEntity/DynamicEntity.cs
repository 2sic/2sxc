using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Data.Typed;
using IEntity = ToSic.Eav.Data.IEntity;
using static ToSic.Eav.Parameters;
using System.Dynamic;
using ToSic.Sxc.Blocks;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
    /// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
    /// </summary>
    [PrivateApi("Changed to private in v16.01, previously was public/stable")]
    public partial class DynamicEntity : DynamicObject, IDynamicEntity, IHasMetadata, IHasPropLookup, ISxcDynamicObject, ICanDebug, ICanHaveBlockContext, ICanGetByName
    {
        #region Constructor / Setup

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        [PrivateApi]
        public DynamicEntity(IEntity entity, CodeDataFactory cdf, bool strict)
            : this(cdf, strict, entity)
        {
            ListHelper = new DynamicEntityListHelper(this, () => Debug, strictGet: strict, cdf);
        }

        internal DynamicEntity(IEnumerable<IEntity> list, IEntity parent, string field, int? appIdOrNull, bool strict, CodeDataFactory cdf)
            : this(cdf, strict,
                // Set the entity - if there was one, or if the list is empty, create a dummy Entity so toolbars will know what to do
                list.FirstOrDefault() ?? cdf.PlaceHolderInBlock(appIdOrNull, parent, field))
        {
            ListHelper = new DynamicEntityListHelper(list, parent, field, () => Debug, strictGet: strict, cdf);
        }
        /// <summary>
        /// Internal helper to make a entity behave as a list, new in 12.03
        /// </summary>
        [PrivateApi]
        internal readonly DynamicEntityListHelper ListHelper;

        private DynamicEntity(CodeDataFactory cdf, bool strict, IEntity entity)
        {
            Cdf = cdf;
            _strict = strict;
            Entity = entity;
            var entAsWrapper = Entity as IEntityWrapper;
            RootContentsForEqualityCheck = entAsWrapper?.RootContentsForEqualityCheck ?? Entity;
            Decorators = entAsWrapper?.Decorators ?? new List<IDecorator<IEntity>>();
        }


        // ReSharper disable once InconsistentNaming
        [PrivateApi] public CodeDataFactory Cdf { get; }
        [PrivateApi] public IEntity Entity { get; }
        private readonly bool _strict;

        [PrivateApi]
        IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ?? (_propLookup = new PropLookupWithPathEntity(Entity, canDebug: this));
        private PropLookupWithPathEntity _propLookup;

        [PrivateApi]
        internal GetAndConvertHelper GetHelper => _getHelper ?? (_getHelper = new GetAndConvertHelper(this, Cdf, _strict, childrenShouldBeDynamic: true, canDebug: this));
        private GetAndConvertHelper _getHelper;

        [PrivateApi]
        internal SubDataFactory SubDataFactory => _subData ?? (_subData = new SubDataFactory(Cdf, _strict, canDebug: this));
        private SubDataFactory _subData;

        [PrivateApi]
        internal CodeDynHelper DynHelper => _dynHelper ?? (_dynHelper = new CodeDynHelper(Entity, SubDataFactory));
        private CodeDynHelper _dynHelper;

        [PrivateApi]
        internal ITypedItem TypedItem => _typedItem ?? (_typedItem = new TypedItemOfEntity(this, Entity, Cdf, _strict));
        private TypedItemOfEntity _typedItem;


        /// <inheritdoc />
        public bool Debug { get; set; }

        #endregion

        #region Basic Entity props: Id, Guid, Title, Type

        /// <inheritdoc />
        public int EntityId => Entity?.EntityId ?? 0;

        /// <inheritdoc />
        public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;

        /// <inheritdoc />
        public string EntityTitle => Entity?.GetBestTitle(Cdf.Dimensions);

        /// <inheritdoc />
        public string EntityType => Entity?.Type?.Name;

        #endregion

        #region Advanced: Fields, Html

        /// <inheritdoc />
        public IField Field(string name) => Cdf.Field(TypedItem, name, _strict);

        /// <inheritdoc/>
        [PrivateApi("Should not be documented here, as it should only be used on ITyped")]
        public IHtmlTag Html(
            string name,
            string noParamOrder = Protector,
            object container = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool debug = default
        ) => Cdf.CompatibilityLevel < Constants.CompatibilityLevel12
            // Only do compatibility check if used on DynamicEntity
            ? throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Razor14, RazorPro or newer.")
            : TypedItemHelpers.Html(Cdf, this.TypedItem, name: name, noParamOrder: noParamOrder, container: container,
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
        public IMetadata Metadata => DynHelper.Metadata;

        /// <summary>
        /// Explicit implementation, so it's not really available on DynamicEntity, only when cast to IHasMetadata
        /// This is important, because it uses the same name "Metadata"
        /// </summary>
        [PrivateApi]
        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;


        #endregion

        #region Relationships: Presentation, Children, Parents

        /// <inheritdoc />
        public dynamic Presentation => DynHelper.Presentation;

        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => GetHelper.Parents(entity: Entity, type: type, field: field);

        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => GetHelper.Children(Entity, field: field, type: type);

        #endregion

        #region Publishing: IsPublished, GetDraft(), GetPublished()

        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        /// <inheritdoc />
        public dynamic GetDraft() => SubDataFactory.SubDynEntityOrNull(Entity == null ? null : Cdf.BlockOrNull?.App.AppState?.GetDraft(Entity));

        /// <inheritdoc />
        public dynamic GetPublished() => SubDataFactory.SubDynEntityOrNull(Entity == null ? null : Cdf.BlockOrNull?.App.AppState?.GetPublished(Entity));

        #endregion

        #region Get / Get<T>

        public dynamic Get(string name) => GetHelper.Get(name);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public dynamic Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null)
            => GetHelper.Get(name, noParamOrder, language, convertLinks, debug);

        public TValue Get<TValue>(string name)
            => GetHelper.Get<TValue>(name);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default)
            => GetHelper.Get(name, noParamOrder, fallback);

        #endregion


        #region Any*** properties just for documentation

        public bool AnyBooleanProperty => true;
        public DateTime AnyDateTimeProperty => DateTime.Now;
        public IEnumerable<IDynamicEntity> AnyChildrenProperty => null;
        public string AnyJsonProperty => null;
        public string AnyLinkOrFileProperty => null;
        public decimal AnyNumberProperty => 0;
        public string AnyStringProperty => null;
        //public IEnumerable<DynamicEntity> AnyTitleOfAnEntityInTheList => null;

        #endregion

        [PrivateApi]
        IBlock ICanHaveBlockContext.TryGetBlockContext() => Cdf?.BlockOrNull;
    }
}