using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Data.Internal.Dynamic;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;
using static ToSic.Eav.Data.Shared.WrapperEquality;
using System.Text.Json.Serialization;
using ToSic.Sxc.Cms.Data;

namespace ToSic.Sxc.Data.Internal.Typed;

[JsonConverter(typeof(DynamicJsonConverter))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TypedItemOfEntity(DynamicEntity dynOrNull, IEntity entity, CodeDataFactory cdf, bool propsRequired)
    : ITypedItem, IHasPropLookup, ICanDebug, ICanBeItem, ICanGetByName, IWrapper<IEntity>,
        IEntityWrapper, IHasMetadata, IHasJsonSource
{
    #region Setup

    // internal TypedItemOfEntity

    public IEntity Entity { get; } = entity;
    private CodeDataFactory Cdf { get; } = cdf;

    #endregion

    public bool Debug { get; set; }

    public override string ToString() => $"{GetType().Name}: '{((ITypedItem)this).Title}' ({Entity})";

    object IHasJsonSource.JsonSource() => Entity;

    IEntity IWrapper<IEntity>.GetContents() => Entity;


    #region Helpers / Services

    [PrivateApi]
    private GetAndConvertHelper GetHelper => _getHelper ??= new(this, Cdf, propsRequired, childrenShouldBeDynamic: false, canDebug: this);
    private GetAndConvertHelper _getHelper;

    [PrivateApi]
    private SubDataFactory SubDataFactory => _subData ??= new(Cdf, propsRequired, canDebug: this);
    private SubDataFactory _subData;

    [PrivateApi]
    private CodeDynHelper DynHelper => _dynHelper ??= new(Entity, SubDataFactory);
    private CodeDynHelper _dynHelper;

    [PrivateApi]
    private CodeItemHelper ItemHelper => _itemHelper ??= new(GetHelper, this);
    private CodeItemHelper _itemHelper;

    #endregion

    #region Special Interface Implementations: IHasPropLookup, IJsonSource, ICanBeItem

    [PrivateApi]
    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new(Entity, canDebug: this);
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

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    public override bool Equals(object b) => MultiWrapperEquality.EqualsObj(this, b);

    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    IEnumerable<IDecorator<IEntity>> IHasDecorators<IEntity>.Decorators => (Entity as IEntityWrapper)?.Decorators ?? [];

    IEntity IMultiWrapper<IEntity>.RootContentsForEqualityCheck => (Entity as IEntityWrapper)?.RootContentsForEqualityCheck ?? Entity;

    #endregion

    #region Keys

    [PrivateApi]
    public bool ContainsKey(string name) =>
        TypedHelpers.ContainsKey(name, Entity,
            (e, k) => e.Attributes.ContainsKey(k),
            (e, k) => e.Children(k)?.FirstOrDefault()
        );

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => ItemHelper.IsEmpty(name, noParamOrder, isBlank: default, language: language);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => ItemHelper.IsNotEmpty(name, noParamOrder, isBlank: default, language: language);

    [PrivateApi]
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default)
        => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

    #endregion

    #region ITyped

    [PrivateApi]
    object ITyped.Get(string name, NoParamOrder noParamOrder, bool? required, string language)
        => ItemHelper.Get(name, noParamOrder, required, language: language);

    [PrivateApi]
    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, string language)
        => ItemHelper.GetT(name, noParamOrder, fallback: fallback, required: required, language: language);

    [PrivateApi]
    IRawHtmlString ITyped.Attribute(string name, NoParamOrder noParamOrder, string fallback, bool? required)
        => ItemHelper.Attribute(name, noParamOrder, fallback, required);

    [PrivateApi]
    [JsonIgnore]
    dynamic ITypedItem.Dyn => _dyn ??= dynOrNull ?? new DynamicEntity(Entity, cdf, propsRequired: propsRequired);
    private object _dyn;

    [PrivateApi]
    DateTime ITyped.DateTime(string name, NoParamOrder noParamOrder, DateTime fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    string ITyped.String(string name, NoParamOrder noParamOrder, string fallback, bool? required, object scrubHtml)
        => ItemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

    [PrivateApi]
    int ITyped.Int(string name, NoParamOrder noParamOrder, int fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    bool ITyped.Bool(string name, NoParamOrder noParamOrder, bool fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    long ITyped.Long(string name, NoParamOrder noParamOrder, long fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    float ITyped.Float(string name, NoParamOrder noParamOrder, float fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    decimal ITyped.Decimal(string name, NoParamOrder noParamOrder, decimal fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    double ITyped.Double(string name, NoParamOrder noParamOrder, double fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    [PrivateApi]
    string ITyped.Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
        => ItemHelper.Url(name, noParamOrder, fallback, required);




    [PrivateApi]
    string ITyped.ToString() => "test / debug: " + ToString();

    /// <summary>
    /// Get by name should never throw an error, as it's used to get null if not found.
    /// </summary>
    object ICanGetByName.Get(string name) => (this as ITyped).Get(name, required: false);

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

    public bool IsDemoItem => _isDemoItem ??= Entity.IsDemoItemSafe();
    private bool? _isDemoItem;

    #endregion
        
    #region ADAM

    /// <inheritdoc />
    [PrivateApi]
    IFolder ITypedItem.Folder(string name, NoParamOrder noParamOrder, bool? required)
    {
        return IsErrStrict(this, name, required, GetHelper.PropsRequired)
            ? throw ErrStrictForTyped(this, name)
            : _adamCache.Get(name, () => Cdf.Folder(Entity, name, ((ITypedItem)this).Field(name, required: false)));
    }
    private readonly GetOnceNamed<IFolder> _adamCache = new();

    IFile ITypedItem.File(string name, NoParamOrder noParamOrder, bool? required)
    {
        var typedThis = (ITypedItem)this;
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
    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedItem ITypedItem.Presentation => (DynHelper.Presentation as ICanBeItem)?.Item;

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    IMetadata ITypedItem.Metadata => DynHelper.Metadata;


    ITypedItem ITypedItem.Parent(NoParamOrder noParamOrder, bool? current, string type, string field)
    {
        if (current != true)
            return ((ITypedItem)this).Parents(type: type, field: field).FirstOrDefault();
            
        return (DynHelper.Parent as ICanBeItem)?.Item
               ?? throw new(
                   $"You tried to access {nameof(ITypedItem.Parent)}({nameof(current)}: true). This should get the original Item which was used to find this one, but this item doesn't seem to have one. " +
                   $"It's only set if this Item was created from another Item using {nameof(ITypedItem.Child)}(...) or {nameof(ITypedItem.Children)}(...). " +
                   $"Were you trying to use {nameof(ITypedItem.Parents)}(...)?");
    }

    /// <inheritdoc />
    [PrivateApi]
    IEnumerable<ITypedItem> ITypedItem.Parents(NoParamOrder noParamOrder, string type, string field)
        => GetHelper.ParentsItems(entity: Entity, type: type, field: field);

    bool ITypedItem.IsPublished => Entity.IsPublished;

    IPublishing ITypedItem.Publishing => _publishing.Get(() => new Publishing(this, Cdf));
    private readonly GetOnce<IPublishing> _publishing = new();

    /// <inheritdoc />
    [PrivateApi]
    IEnumerable<ITypedItem> ITypedItem.Children(string field, NoParamOrder noParamOrder, string type, bool? required)
    {
        if (IsErrStrict(this, field, required, GetHelper.PropsRequired))
            throw ErrStrictForTyped(this, field);

        // Ability to get child/children using path
        var dot = PropertyStack.PathSeparator.ToString();
        if (field.Contains(dot))
        {
            var first = field.Before(dot);
            var rest = Text.After(field, dot);
            if (first.IsEmptyOrWs() || rest.IsEmptyOrWs())
                throw new($"Got path '{field}' but either first or rest are empty");
            // on the direct child, don't apply type filter, as the intermediate step could be anything
            var child = ((ITypedItem)this).Child(first, required: required);
            
            // if the child is null, we must return a fake list which knows about this parent
            return child == null 
                ? cdf.CreateEmptyChildList<ITypedItem>(Entity, field)
                // On the next step, do forward the type filter, as the lowest node should check for that
                : child.Children(rest, type: type, required: required);
        }

        // Standard case: just get the direct children
        var list = GetHelper.ChildrenItems(entity: Entity, field: field, type: type);
        
        // Return list or special list if it's empty, as we need a special list which knows about this object being the parent
        return list.Any() ? list : cdf.CreateEmptyChildList<ITypedItem>(Entity, field);

    }

    /// <inheritdoc />
    [PrivateApi]
    ITypedItem ITypedItem.Child(string name, NoParamOrder noParamOrder, bool? required)
        => IsErrStrict(this, name, required, GetHelper.PropsRequired)
            ? throw ErrStrictForTyped(this, name)
            : ((ITypedItem)this).Children(name).FirstOrDefault();

    #endregion

    #region Fields, Html, Picture

    [PrivateApi]
    IField ITypedItem.Field(string name, NoParamOrder noParamOrder, bool? required) => Cdf.Field(this, name, propsRequired, noParamOrder, required);

    IHtmlTag ITypedItem.Html(
        string name,
        NoParamOrder noParamOrder,
        object container,
        bool? toolbar,
        object imageSettings,
        bool? required,
        bool debug,
        Func<ITweakInput<string>, ITweakInput<string>> tweak
    ) => TypedItemHelpers.Html(Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
        toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug, tweak: tweak);

    /// <inheritdoc/>
    IResponsivePicture ITypedItem.Picture(
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
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
    ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
        imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes, 
        toolbar: toolbar, recipe: recipe);

    IResponsiveImage ITypedItem.Img(
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
        object settings,
        object factor,
        object width,
        string imgAlt,
        string imgAltFallback,
        string imgClass,
        object imgAttributes,
        object toolbar,
        object recipe
    ) => TypedItemHelpers.Img(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback,
        imgClass: imgClass, imgAttributes: imgAttributes,
        toolbar: toolbar, recipe: recipe);


    #endregion


    IMetadataOf IHasMetadata.Metadata => (DynHelper.Metadata as IHasMetadata)?.Metadata;

    #region New Child<T> / Children<T> - disabled as ATM Kit is missing

    /// <inheritdoc />
    T ITypedItem.Child<T>(string name, NoParamOrder protector, bool? required)
        => Cdf.AsCustom<T>(
            source: ((ITypedItem)this).Child(name, required: required), protector: protector, mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Children<T>(string field, NoParamOrder protector, string type, bool? required)
        => Cdf.AsCustomList<T>(
            source: ((ITypedItem)this).Children(field: field, noParamOrder: protector, type: type, required: required),
            protector: protector,
            nullIfNull: false
        );

    /// <inheritdoc />
    T ITypedItem.Parent<T>(NoParamOrder protector, bool? current, string type, string field)
        => Cdf.AsCustom<T>(
            source: ((ITypedItem)this).Parent(noParamOrder: protector, current: current, type: type ?? typeof(T).Name, field: field), protector: protector, mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Parents<T>(NoParamOrder protector, string type, string field)
        => Cdf.AsCustomList<T>(
            source: ((ITypedItem)this).Parents(noParamOrder: protector, field: field, type: type ?? typeof(T).Name), protector: protector, nullIfNull: false
        );


    #endregion

    #region GPS

    GpsCoordinates ITypedItem.Gps(string name, NoParamOrder protector, bool? required) 
        => GpsCoordinates.FromJson(((ITypedItem)this).String(name, required: required));

    #endregion
}