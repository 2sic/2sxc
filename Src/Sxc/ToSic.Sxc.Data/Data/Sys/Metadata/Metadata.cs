using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Sys.Dynamic;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Json;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Data.Sys.Metadata;

// NOTE
// Metadata is still a mix between dynamic and typed.
// Reason is that certain objects such as File.Metadata are the same for both scenarios
// and depending on the Razor version, it the code will either be working with the object
// as dynamic or as typed.
//
// This is not ideal, and probably causes some "you are using old code" warnings after trying to serialize things.

[PrivateApi("Hide implementation")]
internal partial class Metadata(IMetadata metadata, ICodeDataFactory cdf)
    : // DynamicEntity(metadata, null, "Metadata(virtual-field)", KnownAppsConstants.TransientAppId, propsRequired: false, cdf),
        ITypedMetadata, IHasPropLookup, IHasJsonSource
{
    #region Additions when going TypedOnly

    private readonly bool _propsRequired = false;

    [field: AllowNull, MaybeNull]
    public IEntity Entity => field ??= metadata.FirstOrDefault() ?? cdf.PlaceHolderInBlock(/*appIdOrNull ?? 0*/KnownAppsConstants.TransientAppId, /*parent*/ null, /*fieldName*/ null);

    /// <summary>
    /// TypedItem is only internal, for use in APIs which should only have one way to handle data.
    /// Since DynamicEntity is an old API, we don't want to surface TypedItem in the public API.
    /// </summary>
    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal ITypedItem TypedItem => field ??= new TypedItemOfEntity(Entity, Cdf, _propsRequired);

    [PrivateApi] ITypedItem ICanBeItem.Item => TypedItem;

    bool ITypedItem.IsPublished => Entity?.IsPublished ?? true;

    /// <inheritdoc />
    public bool Debug { get; set; }

    private ICodeDataFactory Cdf = cdf;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal GetAndConvertHelper GetHelper => field ??= new(this, cdf, _propsRequired, childrenShouldBeDynamic: true, canDebug: this);

    [PrivateApi]
    public IEntity? RootContentsForEqualityCheck => field
        ??= (Entity as IEntityWrapper)?.RootContentsForEqualityCheck ?? Entity;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    public IEnumerable<IDecorator<IEntity>> Decorators => field ??= (Entity as IEntityWrapper)?.Decorators ?? [];

    public object? Get(string name) => GetHelper.Get(name);

    #endregion

    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new(this, () => Debug);
    private PropLookupMetadata? _propLookup;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    private CodeItemHelper ItemHelper => field ??= new(GetHelper, this);

    [PrivateApi("Hide this")]
    private readonly IMetadata _metadata = metadata;


    IMetadata IHasMetadata.Metadata => _metadata;



    public /*override*/ bool HasType(string type) => _metadata.HasType(type);

    public /*override*/ IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

    #region Properties from the interfaces which are not really supported

    public /*override*/ bool IsDemoItem => false;

    //public new ITypedItem Presentation => throw new NotSupportedException();

    #endregion

    #region Other interfaces: JsonSource, IEquatable<ITypedItem>

    object IHasJsonSource.JsonSource() => throw new NotImplementedException();

    bool IEquatable<ITypedItem>.Equals(ITypedItem? other) => Equals(other);

    #endregion

}