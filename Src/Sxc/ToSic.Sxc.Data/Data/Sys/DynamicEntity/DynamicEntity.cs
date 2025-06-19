﻿using ToSic.Eav.Metadata;
using ToSic.Razor.Blade;
using System.Dynamic;
using ToSic.Eav.Data.Entities.Sys.Wrappers;
using ToSic.Eav.Data.Sys;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Data.Internal.Dynamic;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Internal;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

// This is so we can have a lot of paranoid Entity? null checks
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace ToSic.Sxc.Data;

/// <summary>
/// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
/// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
/// </summary>
[PrivateApi("Changed to private in v16.01, previously was public/stable")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class DynamicEntity : DynamicObject, IDynamicEntity, IHasMetadata, IHasPropLookup, ISxcDynamicObject, ICanDebug, ICanBeItem, ICanGetByName
{
    #region Constructor / Setup

    /// <summary>
    /// Constructor with EntityModel and DimensionIds
    /// </summary>
    [PrivateApi]
    public DynamicEntity(IEntity entity, ICodeDataFactory cdf, bool propsRequired)
        : this(cdf, propsRequired, entity)
    {
        ListHelper = new(this, () => Debug, propsRequired: propsRequired, cdf);
    }

    internal DynamicEntity(IEnumerable<IEntity> list, IEntity? parent, string field, int? appIdOrNull, bool propsRequired, ICodeDataFactory cdf)
        : this(cdf, propsRequired,
            // Set the entity - if there was one, or if the list is empty, create a dummy Entity so toolbars will know what to do
            list.FirstOrDefault() ?? cdf.PlaceHolderInBlock(appIdOrNull ?? 0, parent, field))
    {
        ListHelper = new(list, parent, field, () => Debug, propsRequired: propsRequired, cdf);
    }
    /// <summary>
    /// Internal helper to make a entity behave as a list, new in 12.03
    /// </summary>
    [PrivateApi]
    internal readonly DynamicEntityListHelper ListHelper = null!;

    private DynamicEntity(ICodeDataFactory cdf, bool propsRequired, IEntity entity)
    {
        Cdf = cdf;
        _propsRequired = propsRequired;
        Entity = entity;
        var entAsWrapper = Entity as IEntityWrapper;
        RootContentsForEqualityCheck = entAsWrapper?.RootContentsForEqualityCheck ?? Entity;
        Decorators = entAsWrapper?.Decorators ?? [];
    }


    // ReSharper disable once InconsistentNaming
    [PrivateApi] public ICodeDataFactory Cdf { get; }
    [PrivateApi] public IEntity Entity { get; }
    private readonly bool _propsRequired;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    IPropertyLookup IHasPropLookup.PropertyLookup => field ??= new PropLookupWithPathEntity(Entity, canDebug: this);

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal GetAndConvertHelper GetHelper => field ??= new(this, Cdf, _propsRequired, childrenShouldBeDynamic: true, canDebug: this);

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal SubDataFactory SubDataFactory => field ??= new(Cdf, _propsRequired, canDebug: this);

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal CodeDynHelper DynHelper => field ??= new(Entity, SubDataFactory);

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal ITypedItem TypedItem => field ??= new TypedItemOfEntity(this, Entity, Cdf, _propsRequired);

    /// <inheritdoc />
    public bool Debug { get; set; }

    #endregion

    #region Basic Entity props: Id, Guid, Title, Type

    /// <inheritdoc />
    public int EntityId => Entity?.EntityId ?? 0;

    /// <inheritdoc />
    public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;

    /// <inheritdoc />
    public string? EntityTitle => Entity?.GetBestTitle(Cdf.Dimensions);

    /// <inheritdoc />
    public string? EntityType => Entity?.Type?.Name;

    #endregion

    #region Advanced: Fields, Html

    /// <inheritdoc />
    public IField? Field(string name) => Cdf.Field(TypedItem, name, _propsRequired);

    /// <inheritdoc/>
    [PrivateApi("Should not be documented here, as it should only be used on ITyped")]
    public IHtmlTag? Html(
        string name,
        NoParamOrder noParamOrder = default,
        object? container = default,
        bool? toolbar = default,
        object? imageSettings = default,
        bool debug = default
    ) => Cdf.CompatibilityLevel < CompatibilityLevels.CompatibilityLevel12
        // Only do compatibility check if used on DynamicEntity
        ? throw new NotSupportedException($"{nameof(Html)}(...) not supported in older Razor templates. Use Razor14, RazorTyped or newer.")
        : TypedItemHelpers.Html(Cdf, TypedItem, name: name, noParamOrder: noParamOrder, container: container,
            toolbar: toolbar, imageSettings: imageSettings, required: false, debug: debug);

    #endregion

    #region Special: IsDemoItem, IsFake

    // ReSharper disable once InheritdocInvalidUsage
    /// <inheritdoc />
    public virtual bool IsDemoItem => _isDemoItem ??= Entity.IsDemoItemSafe();
    private bool? _isDemoItem;

    [PrivateApi("Not in use yet, and I believe not communicated")]
    public bool IsFake => _isFake ??= (Entity?.EntityId ?? DataConstants.DataFactoryDefaultEntityId) == DataConstants.DataFactoryDefaultEntityId;
    private bool? _isFake;

    #endregion

    #region Metadata

    /// <inheritdoc />
    public ITypedMetadata Metadata => DynHelper.Metadata!;

    /// <summary>
    /// Explicit implementation, so it's not really available on DynamicEntity, only when cast to IHasMetadata
    /// This is important, because it uses the same name "Metadata"
    /// </summary>
    [PrivateApi]
    IMetadataOf IHasMetadata.Metadata => Entity?.Metadata!;


    #endregion

    #region Relationships: Presentation, Children, Parents

    /// <inheritdoc />
    public dynamic? Presentation => DynHelper.Presentation;

    /// <inheritdoc />
    public List<IDynamicEntity?> Parents(string? type = null, string? field = null)
        => GetHelper.ParentsDyn(entity: Entity!, type: type, field: field);

    /// <inheritdoc />
    public List<IDynamicEntity?> Children(string? field = null, string? type = null)
        => GetHelper.ChildrenDyn(Entity!, field: field, type: type);

    #endregion

    #region Publishing: IsPublished, GetDraft(), GetPublished()

    /// <inheritdoc />
    public bool IsPublished => Entity?.IsPublished ?? true;

    /// <inheritdoc />

    public dynamic? GetDraft() => SubDataFactory.SubDynEntityOrNull(Entity == null
        ? null
        : (Cdf as ICodeDataFactoryDeepWip)?.AppReaderOrNull?.GetDraft(Entity)
    );

    /// <inheritdoc />
    public dynamic? GetPublished() => SubDataFactory.SubDynEntityOrNull(Entity == null
        ? null
        : (Cdf as ICodeDataFactoryDeepWip)?.AppReaderOrNull?.GetPublished(Entity)
    );

    #endregion

    #region Get / Get<T>

    public dynamic? Get(string name) => GetHelper.Get(name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public dynamic? Get(string name, NoParamOrder noParamOrder = default, string? language = null, bool convertLinks = true, bool? debug = null)
        => GetHelper.Get(name, noParamOrder, language, convertLinks, debug);

    public TValue? Get<TValue>(string name)
        => GetHelper.Get<TValue>(name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TValue? Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue? fallback = default)
        => GetHelper.Get(name, noParamOrder, fallback);

    #endregion


    #region Any*** properties just for documentation

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public dynamic? AnyProperty => null;

    #endregion

    [PrivateApi] object? ICanBeItem.TryGetBlock() => Cdf?.BlockAsObjectOrNull;
    [PrivateApi] ITypedItem ICanBeItem.Item => TypedItem;


    #region Metadata - Enable Metadata.Methods()

    // Background: 2023-08-15 2dm
    // For reasons we don't fully understand, the razor dynamic binder can't find methods on inherited objects.
    // If we add their signatures here, and then override them in the implementation, it works
    // This is probably not the best way to do it, but for now it should work.

    [PrivateApi("This doesn't work until overriden by the Metadata object")]
    public virtual bool HasType(string type) => throw new NotSupportedException("This is just a stub for Metadata");

    [PrivateApi("This doesn't work until overriden by the Metadata object")]
    public virtual IEnumerable<IEntity> OfType(string type) => throw new NotSupportedException("This is just a stub for Metadata");

    #endregion
}