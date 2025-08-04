using System.Text.Json.Serialization;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Data.Sys.PropertyStack;
using ToSic.Eav.Metadata;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data.Options;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Dynamic;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Json;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;
using static ToSic.Sxc.Data.Sys.Typed.TypedHelpers;
using static ToSic.Sys.Wrappers.WrapperEquality;

// This is for paranoid Entity? checking
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace ToSic.Sxc.Data.Sys.Typed;

[JsonConverter(typeof(DynamicJsonConverter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class TypedItemOfEntity(IEntity entity, ICodeDataFactory cdf, bool propsRequired)
    // ReSharper disable RedundantExtendsListEntry
    : ITypedItem, IHasPropLookup, ICanDebug, ICanBeItem, ICanGetByName, IWrapper<IEntity>,
        IEntityWrapper, IHasMetadata, IHasJsonSource
    // ReSharper restore RedundantExtendsListEntry
{
    #region Setup

    // internal TypedItemOfEntity

    public IEntity Entity { get; } = entity;
    private ICodeDataFactory Cdf { get; } = cdf;

    #endregion

    public bool Debug { get; set; }

    public override string ToString() => $"{GetType().Name}: '{((ITypedItem)this).Title}' ({Entity})";

    object IHasJsonSource.JsonSource() => Entity;

    IEntity IWrapper<IEntity>.GetContents() => Entity;


    #region Private Helpers / Services

    [field: AllowNull, MaybeNull]
    private GetAndConvertHelper GetHelper => field ??= new(this, Cdf, propsRequired, childrenShouldBeDynamic: false, canDebug: this);

    [field: AllowNull, MaybeNull]
    private CodeDynHelper DynHelper => field ??= new(Entity, new(Cdf, propsRequired, canDebug: this));

    [field: AllowNull, MaybeNull]
    private CodeItemHelper ItemHelper => field ??= new(GetHelper, this);

    #endregion

    #region Special Interface Implementations: IHasPropLookup, IJsonSource, ICanBeItem

    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new(Entity, canDebug: this);
    private PropLookupWithPathEntity? _propLookup;

    // #RemoveBlocksIRenderService
    //[PrivateApi] object? ICanBeItem.TryGetBlock() => Cdf?.BlockAsObjectOrNull;
    ITypedItem ICanBeItem.Item => this;

    #endregion

    #region Equality

    /// <summary>
    /// This is used by various equality comparison. 
    /// Since we define two object to be equal when they host the same contents, this determines the hash based on the contents
    /// </summary>
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => GetWrappedHashCode(this);

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    public override bool Equals(object? b) => MultiWrapperEquality.EqualsObj(this, b);

    bool IEquatable<ITypedItem>.Equals(ITypedItem? other) => Equals(other);

    IEnumerable<IDecorator<IEntity>> IHasDecorators<IEntity>.Decorators => (Entity as IEntityWrapper)?.Decorators ?? [];

    IEntity IMultiWrapper<IEntity>.RootContentsForEqualityCheck => (Entity as IEntityWrapper)?.RootContentsForEqualityCheck ?? Entity;

    #endregion

    #region Keys


    public bool ContainsKey(string name) =>
        TypedHelpers.ContainsKey(name, Entity,
            (e, k) => e.Attributes.ContainsKey(k),
            (e, k) => e.Children(k)?.FirstOrDefault()
        );

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string? language = default)
        => ItemHelper.IsEmpty(name, noParamOrder, isBlank: default, language: language);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string? language = default)
        => ItemHelper.IsNotEmpty(name, noParamOrder, isBlank: default, language: language);


    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string>? only = default)
        => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

    #endregion

    #region ITyped

    object? ITyped.Get(string name, NoParamOrder noParamOrder, bool? required, string? language)
        => ItemHelper.Get(name, noParamOrder, required, language: language);

    TValue? ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue? fallback, bool? required, string? language)
        where TValue : default
        => ItemHelper.GetT(name, noParamOrder, fallback: fallback, required: required, language: language);

    IRawHtmlString? ITyped.Attribute(string name, NoParamOrder noParamOrder, string? fallback, bool? required)
        => ItemHelper.Attribute(name, noParamOrder, fallback, required);

    DateTime ITyped.DateTime(string name, NoParamOrder noParamOrder, DateTime fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    string? ITyped.String(string name, NoParamOrder noParamOrder, string? fallback, bool? required, object? scrubHtml)
        => ItemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

    int ITyped.Int(string name, NoParamOrder noParamOrder, int fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    bool ITyped.Bool(string name, NoParamOrder noParamOrder, bool fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    long ITyped.Long(string name, NoParamOrder noParamOrder, long fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    float ITyped.Float(string name, NoParamOrder noParamOrder, float fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    decimal ITyped.Decimal(string name, NoParamOrder noParamOrder, decimal fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    double ITyped.Double(string name, NoParamOrder noParamOrder, double fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    string? ITyped.Url(string name, NoParamOrder noParamOrder, string? fallback, bool? required)
        => ItemHelper.Url(name, noParamOrder, fallback, required);



    string ITyped.ToString() => "test / debug: " + ToString();

    /// <summary>
    /// Get by name should never throw an error, as it's used to get null if not found.
    /// </summary>
    object? ICanGetByName.Get(string name) => (this as ITyped).Get(name, required: false);

    #endregion

    #region Basic Props like Id, Guid, Title, Type, IsDemoItem

    int ITypedItem.Id => Entity?.EntityId ?? 0;

    Guid ITypedItem.Guid => Entity?.EntityGuid ?? Guid.Empty;

    string? ITypedItem.Title => Entity?.GetBestTitle(Cdf.Dimensions);

    IContentType ITypedItem.Type => Entity?.Type!;
    public bool IsDemoItem => _isDemoItem ??= Entity.IsDemoItemSafe();
    private bool? _isDemoItem;

    #endregion
        
    #region ADAM

    IFolder ITypedItem.Folder(string name, NoParamOrder noParamOrder, bool? required)
        => IsErrStrictNameRequired(this, name, required, GetHelper.PropsRequired)
            ? throw ErrStrictForTyped(this, name)
            : _adamCache.Get(name, () => Cdf.Folder(Entity, name, ((ITypedItem)this).Field(name, required: false)));

    private readonly GetOnceNamed<IFolder> _adamCache = new();

    IFile? ITypedItem.File(string name, NoParamOrder noParamOrder, bool? required)
    {
        ITypedItem typedThis = this;
        // Case 1: The field contains a direct reference to a file
        var field = typedThis.Field(name, required: required);
        if (field == null)
            return null; // if the field is not found, return null
        var file = Cdf.File(field);
        // Case 2: No direct reference, just get the first file in the folder of this field
        return file ?? typedThis.Folder(name)?.Files.FirstOrDefault();
    }

    #endregion

    #region Relationship properties Presentation, Metadata, Child, Children, Parents

    /// <inheritdoc />

    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedItem? ITypedItem.Presentation => (DynHelper.Presentation as ICanBeItem)?.Item;

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedMetadata ITypedItem.Metadata => _md.Get(() => ItemHelper.Helper.Cdf.MetadataTyped(Entity.Metadata))!;
    private readonly GetOnce<ITypedMetadata?> _md = new();


    ITypedItem? ITypedItem.Parent(NoParamOrder noParamOrder, bool? current, string? type, string? field, GetRelatedOptions? options)
    {
        if (current != true)
            return ((ITypedItem)this).Parents(type: type, field: field, options: options).FirstOrDefault();
            
        return (DynHelper.Parent as ICanBeItem)?.Item
               ?? throw new(
                   $"You tried to access {nameof(ITypedItem.Parent)}({nameof(current)}: true). This should get the original Item which was used to find this one, but this item doesn't seem to have one. " +
                   $"It's only set if this Item was created from another Item using {nameof(ITypedItem.Child)}(...) or {nameof(ITypedItem.Children)}(...). " +
                   $"Were you trying to use {nameof(ITypedItem.Parents)}(...)?");
    }


    IEnumerable<ITypedItem> ITypedItem.Parents(NoParamOrder noParamOrder, string? type, string? field, GetRelatedOptions? options)
        => GetHelper.ParentsItems(entity: Entity, type: type, field: field, options: options ?? new());

    bool ITypedItem.IsPublished => Entity.IsPublished;

    IPublishing ITypedItem.Publishing => _publishing.Get(() => new Publishing.Publishing(this, Cdf))!;
    private readonly GetOnce<IPublishing> _publishing = new();

    
    IEnumerable<ITypedItem> ITypedItem.Children(string? field, NoParamOrder noParamOrder, string? type, bool? required, GetRelatedOptions? options)
    {
        if (IsErrStrictNameOptional(this, field, required, GetHelper.PropsRequired))
            throw ErrStrictForTyped(this, field);

        // Ability to get child/children using path such as Child.Grandchild.GreatGrandChild
        var dot = PropertyStack.PathSeparator.ToString();
        if (field!.Contains(dot))
        {
            var first = field.Before(dot);
            var rest = Text.After(field, dot);

            // Verify it never starts / ends with a dot
            if (first.IsEmptyOrWs() || rest.IsEmptyOrWs())
                throw new($"Got path '{field}' but either first or rest are empty");

            // Get first part of path without applying type filter, since that should only apply to the last returned level
            var child = ((ITypedItem)this).Child(first, required: required);
            
            // if the child is null, we must return a fake list which knows about this parent
            // so any UI interactions trying to add children know about it
            if (child == null)
                return Cdf.CreateEmptyChildList<ITypedItem>(Entity, field);

            // On the next step, do forward the type filter, as the lowest node should check for that
            return child.Children(rest, type: type, required: required, options: options);
        }

        // Standard case: just get the direct children
        var list = GetHelper.ChildrenItems(entity: Entity, field: field, type: type, options ?? new());
        
        // Return list or special list if it's empty, as we need a special list which knows about this object being the parent
        return list.Any()
            ? list
            : Cdf.CreateEmptyChildList<ITypedItem>(Entity, field);

    }

    /// <inheritdoc />

    ITypedItem? ITypedItem.Child(string name, NoParamOrder noParamOrder, bool? required, GetRelatedOptions? options)
        => IsErrStrictNameRequired(this, name, required, GetHelper.PropsRequired)
            ? throw ErrStrictForTyped(this, name)
            : ((ITypedItem)this).Children(name, options: options).FirstOrDefault();

    #endregion

    #region Fields, Html, Picture


    IField? ITypedItem.Field(string name, NoParamOrder noParamOrder, bool? required)
        => Cdf.Field(this, supportOldMetadata: false, name, new() { FirstIsRequired = required ?? true, ItemIsStrict = propsRequired });

    IHtmlTag? ITypedItem.Html(
        string name,
        NoParamOrder noParamOrder,
        object? container,
        bool? toolbar,
        object? imageSettings,
        bool? required,
        bool debug,
        Func<ITweakInput<string>, ITweakInput<string>>? tweak
    ) => TypedItemHelpers.Html(Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
        toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug, tweak: tweak);

    /// <inheritdoc/>
    IResponsivePicture? ITypedItem.Picture(
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia>? tweak,
        object? settings,
        object? factor,
        object? width,
        string? imgAlt,
        string? imgAltFallback,
        string? imgClass,
        object? imgAttributes,
        string? pictureClass,
        object? pictureAttributes,
        object? toolbar,
        object? recipe
    ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
        imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes, 
        toolbar: toolbar, recipe: recipe);

    IResponsiveImage? ITypedItem.Img(
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia>? tweak,
        object? settings,
        object? factor,
        object? width,
        string? imgAlt,
        string? imgAltFallback,
        string? imgClass,
        object? imgAttributes,
        object? toolbar,
        object? recipe
    ) => TypedItemHelpers.Img(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback,
        imgClass: imgClass, imgAttributes: imgAttributes,
        toolbar: toolbar, recipe: recipe);


    #endregion


    IMetadata IHasMetadata.Metadata => (((ITypedItem)this).Metadata as IHasMetadata)?.Metadata!;

    #region New Child<T> / Children<T> - disabled as ATM Kit is missing

    /// <inheritdoc />
    T? ITypedItem.Child<T>(string name, NoParamOrder protector, bool? required, GetRelatedOptions? options)
        where T : class
        => Cdf.AsCustom<T>(
            source: ((ITypedItem)this).Child(name, required: required, options: options),
            protector: protector,
            mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Children<T>(string? field, NoParamOrder protector, string? type, bool? required, GetRelatedOptions? options)
        => Cdf.AsCustomList<T>(
            source: ((ITypedItem)this).Children(field: field, noParamOrder: protector, type: type, required: required, options: options),
            protector: protector,
            nullIfNull: false
        );

    /// <inheritdoc />
    T? ITypedItem.Parent<T>(NoParamOrder protector, bool? current, string? type, string? field, GetRelatedOptions? options)
        where T : class
        => Cdf.AsCustom<T>(
            source: ((ITypedItem)this).Parent(noParamOrder: protector, current: current, type: type ?? typeof(T).Name, field: field, options: options),
            protector: protector,
            mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Parents<T>(NoParamOrder protector, string? type, string? field, GetRelatedOptions? options)
        => Cdf.AsCustomList<T>(
            source: ((ITypedItem)this).Parents(noParamOrder: protector, field: field, type: type ?? typeof(T).Name, options: options),
            protector: protector,
            nullIfNull: false
        );


    #endregion

    #region GPS

    GpsCoordinates ITypedItem.Gps(string name, NoParamOrder protector, bool? required) 
        => GpsCoordinates.FromJson(((ITypedItem)this).String(name, required: required));

    #endregion
}