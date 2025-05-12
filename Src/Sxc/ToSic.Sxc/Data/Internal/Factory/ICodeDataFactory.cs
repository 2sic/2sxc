using ToSic.Eav.Context;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Code.InfoSystem;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data.Internal.Stack;

namespace ToSic.Sxc.Data.Internal;

public interface ICodeDataFactory
{
    IMetadata Metadata(IMetadataOf mdOf);

    /// <summary>
    /// Convert an object to a custom type, if possible.
    /// If the object is an entity-like thing, that will be converted.
    /// If it's a list of entity-like things, the first one will be converted.
    /// </summary>
    TCustom AsCustom<TCustom>(object source, NoParamOrder protector = default, bool mock = false)
        where TCustom : class, ICanWrapData;

    TCustom AsCustomFrom<TCustom, TData>(TData item)
        where TCustom : class, ICanWrapData;

    /// <summary>
    /// Create list of custom-typed ITypedItems
    /// </summary>
    IEnumerable<TCustom> AsCustomList<TCustom>(object source, NoParamOrder protector, bool nullIfNull)
        where TCustom : class, ICanWrapData;

    ITyped AsTyped(object data, bool required = false, bool? propsRequired = default, string detailsMessage = default);
    IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder, bool? required = false, bool? propsRequired = default);
    AdamManager AdamManager { get; }
    int CompatibilityLevel { get; }
    CodeDataServices Services { get; }

    /// <summary>
    /// WIP, we need this in the GetAndConvertHelper, and want to make sure it's not executed on every entity used,
    /// so for now we're doing this once only here.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: LOWER-CASE guaranteed.
    /// </remarks>
    List<string> SiteCultures { get; }

    /// <inheritdoc />
    ILog Log { get; }

    ICodeApiService _CodeApiSvc { get; }
    bool Debug { get; set; }
    IBlock BlockOrNull { get; }

    /// <summary>
    /// List of dimensions for value lookup, incl. priorities etc. and null-trailing.
    /// lower case safe guaranteed. 
    /// </summary>
    // If we don't have a DynCodeRoot, try to generate the language codes and compatibility
    // There are cases where these were supplied using SetFallbacks, but in some cases none of this is known
    string[] Dimensions { get; }

    CodeInfoService CodeInfo { get; }

    IFolder Folder(ICanBeEntity item, string name, IField field);

    /// <summary>
    /// Implement AsDynamic for DynamicCode - not to be used in internal APIs.
    /// Always assumes Strict is false
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    DynamicEntity CodeAsDyn(IEntity entity);

    DynamicEntity AsDynamic(IEntity entity, bool propsRequired);

    /// <summary>
    /// Convert a list of Entities into a DynamicEntity.
    /// Only used in DynamicCodeRoot.
    /// </summary>
    DynamicEntity AsDynamicFromEntities(IEnumerable<IEntity> list, bool propsRequired);

    /// <summary>
    /// Convert any object into a dynamic list.
    /// Only used in Dynamic Code for the public API.
    /// </summary>
    IEnumerable<dynamic> CodeAsDynList(object list, bool propsRequired = false);

    /// <summary>
    /// Convert any object into a dynamic object.
    /// Only used in Dynamic Code for the public API.
    /// </summary>
    object AsDynamicFromObject(object dynObject, bool propsRequired = false);

    dynamic MergeDynamic(object[] entities);
    ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? required = default, ITypedItem fallback = default, bool? propsRequired = default, bool? mock = default);

    /// <summary>
    /// Quick convert an entity to item - if not null, otherwise return null.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="propsRequired"></param>
    /// <returns></returns>
    ITypedItem AsItem(IEntity entity, bool propsRequired);

    IEnumerable<ITypedItem> EntitiesToItems(IEnumerable<IEntity> entities, bool propsRequired = false);
    IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? required = default, IEnumerable<ITypedItem> fallback = default, bool? propsRequired = default);
    void SetCompatibilityLevel(int compatibilityLevel);
    void SetFallbacks(ISite site, int? compatibility = default, AdamManager adamManagerPrepared = default);
    object Json2Jacket(string json, string fallback = default);
    ITypedStack AsStack(object[] parts);

    T AsStack<T>(object[] parts)
        where T : class, ICanWrapData, new();

    DynamicStack AsDynStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources);
    ITypedStack AsTypedStack(string name, List<KeyValuePair<string, IPropertyLookup>> sources);
    IField Field(ITypedItem parent, string name, bool propsRequired, NoParamOrder noParamOrder = default, bool? required = default);
    IEntity AsEntity(object thingToConvert);
    IEntity FakeEntity(int? appId);

    TCustom GetOne<TCustom>(Func<IEntity> getItem, object id, bool skipTypeCheck)
        where TCustom : class, ICanWrapData;

    IEntity PlaceHolderInBlock(int? appIdOrNull, IEntity parent, string field);

    /// <summary>
    /// Creates an empty list of a specific type, with hidden information to remember what field this is etc.
    /// </summary>
    /// <typeparam name="TTypedItem"></typeparam>
    /// <param name="parent"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    IEnumerable<TTypedItem> CreateEmptyChildList<TTypedItem>(IEntity parent, string field) where TTypedItem : class, ITypedItem;
}