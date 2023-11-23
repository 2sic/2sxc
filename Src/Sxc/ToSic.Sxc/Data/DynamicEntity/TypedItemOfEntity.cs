using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Images;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;
using static ToSic.Eav.Data.Shared.WrapperEquality;

namespace ToSic.Sxc.Data
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class TypedItemOfEntity: ITypedItem, IHasPropLookup, ICanDebug, ICanBeItem, ICanGetByName,
        IWrapper<IEntity>, IHasMetadata
    {
        #region Setup

        public TypedItemOfEntity(DynamicEntity dyn, IEntity entity, CodeDataFactory cdf, bool propsRequired)
        {
            Entity = entity;
            Cdf = cdf;
            _dyn = dyn;
            _propsRequired = propsRequired;
        }

        public IEntity Entity { get; }
        private readonly DynamicEntity _dyn;
        private CodeDataFactory Cdf { get; }
        private readonly bool _propsRequired;

        #endregion

        public bool Debug { get; set; }

        IEntity IWrapper<IEntity>.GetContents() => Entity;


        #region Helpers / Services

        [PrivateApi]
        private GetAndConvertHelper GetHelper => _getHelper ??= new GetAndConvertHelper(this, Cdf, _propsRequired, childrenShouldBeDynamic: false, canDebug: this);
        private GetAndConvertHelper _getHelper;

        [PrivateApi]
        private SubDataFactory SubDataFactory => _subData ??= new SubDataFactory(Cdf, _propsRequired, canDebug: this);
        private SubDataFactory _subData;

        [PrivateApi]
        private CodeDynHelper DynHelper => _dynHelper ??= new CodeDynHelper(Entity, SubDataFactory);
        private CodeDynHelper _dynHelper;

        [PrivateApi]
        private CodeItemHelper ItemHelper => _itemHelper ??= new CodeItemHelper(GetHelper, this);
        private CodeItemHelper _itemHelper;

        #endregion

        #region Special Interface Implementations: IHasPropLookup, IJsonSource, ICanBeItem

        [PrivateApi]
        IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new PropLookupWithPathEntity(Entity, canDebug: this);
        private PropLookupWithPathEntity _propLookup;

        [PrivateApi] IBlock ICanBeItem.TryGetBlockContext() => Cdf?.BlockOrNull;
        [PrivateApi] ITypedItem ICanBeItem.Item => this;

        #endregion

        #region Equality

        /// <summary>
        /// This is used by various equality comparison. 
        /// Since we define two object to be equal when they host the same contents, this determines the hash based on the contents
        /// </summary>
        [PrivateApi]
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => GetWrappedHashCode(this);

        public override bool Equals(object b)
        {
            if (b is null) return false;
            if (ReferenceEquals(this, b)) return true;
            if (b.GetType() != GetType()) return false;
            return EqualsWrapper(this, (TypedItemOfEntity)b);
        }

        #endregion

        #region Keys

        [PrivateApi]
        public bool ContainsKey(string name) =>
            TypedHelpers.ContainsKey(name, Entity,
                (e, k) => e.Attributes.ContainsKey(k),
                (e, k) => e.Children(k)?.FirstOrDefault()
            );

        public bool IsEmpty(string name, string noParamOrder = Protector)
            => ItemHelper.IsEmpty(name, noParamOrder, default);

        public bool IsNotEmpty(string name, string noParamOrder = Protector)
            => ItemHelper.IsFilled(name, noParamOrder, default);

        [PrivateApi]
        public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
            => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

        #endregion

        #region ITyped

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
            => ItemHelper.Get(name, noParamOrder, required);

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
            => ItemHelper.Attribute(name, noParamOrder, fallback, required);

        [PrivateApi]
        dynamic ITypedItem.Dyn => _dyn;


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml)
            => ItemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
            => ItemHelper.Url(name, noParamOrder, fallback, required);


        [PrivateApi]
        string ITyped.ToString() => "test / debug: " + ToString();

        dynamic ICanGetByName.Get(string name) => (this as ITyped).Get(name);

        #endregion

        #region Basic Props like Id, Guid, Title, Type, IsDemoItem

        [PrivateApi]
        int ITypedItem.Id => Entity?.EntityId ?? 0;

        [PrivateApi]
        Guid ITypedItem.Guid => Entity?.EntityGuid ?? Guid.Empty;

        [PrivateApi]
        string ITypedItem.Title => Entity?.GetBestTitle(Cdf.Dimensions);

        [PrivateApi]
        IContentType ITypedItem.Type => Entity?.Type;

        public bool IsDemoItem => _isDemoItem ?? (_isDemoItem = Entity.IsDemoItemSafe()).Value;
        private bool? _isDemoItem;

        #endregion
        
        #region ADAM

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, GetHelper.PropsRequired)
                ? throw ErrStrictForTyped(this, name)
                : _adamCache.Get(name, () => Cdf.Folder(Entity, name, (this as ITypedItem).Field(name, required: false)));
        }
        private readonly GetOnceNamed<IFolder> _adamCache = new();

        IFile ITypedItem.File(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            var typedThis = this as ITypedItem;
            // Case 1: The field contains a direct reference to a file
            var field = typedThis.Field(name, required: required);
            var file = Cdf.GetServiceKitOrThrow().Adam.File(field);
            // Case 2: No direct reference, just get the first file in the folder of this field
            return file ?? typedThis.Folder(name).Files.FirstOrDefault();
        }

        #endregion

        #region Relationship properties Presentation, Metadata, Child, Children, Parents

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Presentation => (DynHelper.Presentation as DynamicEntity)?.TypedItem;

        /// <inheritdoc />
        IMetadata ITypedItem.Metadata => DynHelper.Metadata;


        ITypedItem ITypedItem.Parent(string noParamOrder, bool? current, string type, string field)
        {
            Protect(noParamOrder, $"{nameof(current)} {nameof(type)}, {nameof(field)}");
            if (current != true)
                return (this as ITypedItem).Parents(type: type, field: field).FirstOrDefault();
            
            return (DynHelper.Parent as DynamicEntity)?.TypedItem
                   ?? throw new Exception(
                       $"You tried to access {nameof(ITypedItem.Parent)}({nameof(current)}: true). This should get the original Item which was used to find this one, but this item doesn't seem to have one. " +
                       $"It's only set if this Item was created from another Item using {nameof(ITypedItem.Child)}(...) or {nameof(ITypedItem.Children)}(...). " +
                       $"Were you trying to use {nameof(ITypedItem.Parents)}(...)?");
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string noParamOrder, string type, string field)
        {
            Protect(noParamOrder, nameof(field), message:
                $" ***IMPORTANT***: The typed '.Parents(...)' method was changed to also make the parameter '{nameof(type)}' required. " +
                "So if you had '.Parents(something)' then change it to '.Parents(type: something)'. See https://r.2sxc.org/brc-1603");
            return Cdf.AsItems(GetHelper.Parents(entity: Entity, type: type, field: field), noParamOrder);
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type, bool? required)
        {
            Protect(noParamOrder, $"{nameof(type)}, {nameof(required)}");

            if (IsErrStrict(this, field, required, GetHelper.PropsRequired))
                throw ErrStrictForTyped(this, field);

            // new 16.08 ability to get child/children using path
            var dot = PropertyStack.PathSeparator.ToString();
            if (field.Contains(dot))
            {
                var first = field.Before(dot);
                var rest = Text.After(field, dot);
                if (first.IsEmptyOrWs() || rest.IsEmptyOrWs())
                    throw new Exception($"Got path '{field}' but either first or rest are empty");
                // on the direct child, don't apply type filter, as the intermediate step could be anything
                var child = (this as ITypedItem).Child(first, required: required);
                if (child == null) return CreateEmptyChildList();
                // On the next step, do forward the type filter, as the lowest node should check for that
                return child.Children(rest, type: type, required: required);
            }

            // Standard case: just get the direct children
            var dynChildren = GetHelper.Children(entity: Entity, field: field, type: type);
            var list = dynChildren.Cast<DynamicEntity>().Select(d => d.TypedItem).ToList();
            if (list.Any()) return list;

            return CreateEmptyChildList();

            IEnumerable<ITypedItem> CreateEmptyChildList()
            {
                // Generate a marker/placeholder to remember what field this is etc.
                var fakeEntity = GetHelper.Cdf.PlaceHolderInBlock(Entity.AppId, Entity, field);
                return new ListTypedItems(new List<ITypedItem>(), fakeEntity);
            }
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, GetHelper.PropsRequired)
                ? throw ErrStrictForTyped(this, name)
                : (this as ITypedItem).Children(name).FirstOrDefault();
        }

        #endregion

        #region Fields, Html, Picture

        [PrivateApi]
        IField ITypedItem.Field(string name, string noParamOrder, bool? required) => Cdf.Field(this, name, _propsRequired, noParamOrder, required);

        IHtmlTag ITypedItem.Html(
            string name,
            string noParamOrder,
            object container,
            bool? toolbar,
            object imageSettings,
            bool? required,
            bool debug
        ) => TypedItemHelpers.Html(Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
            toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug);

        /// <inheritdoc/>
        IResponsivePicture ITypedItem.Picture(
            string name,
            string noParamOrder,
            object settings,
            object factor,
            object width,
            string imgAlt,
            string imgAltFallback,
            string imgClass,
            object imgAttributes,
            string pictureClass,
            object pictureAttributes,
            object toolbar,
            object recipe
        ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, settings: settings,
            factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
            imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes, 
            toolbar: toolbar, recipe: recipe);

        #endregion


        IMetadataOf IHasMetadata.Metadata => (DynHelper.Metadata as IHasMetadata)?.Metadata;
    }
}
