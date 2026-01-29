using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;
using ToSic.Eav.Models.Factory;
using ToSic.Eav.Models.Sys;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory: IModelFactory
{
    public TModel? Create<TSource, TModel>(TSource? source)
        where TModel : IModelSetup<TSource>
    {
        var wrapper = serviceProvider.Build<TModel>();
        var ok = wrapper.SetupModel(source);
        return ok ? wrapper : default;
    }

    /// <summary>
    /// Convert an object to a custom type, if possible.
    /// If the object is an entity-like thing, that will be converted.
    /// If it's a list of entity-like things, the first one will be converted.
    /// </summary>
    [return: NotNullIfNotNull(nameof(source))]
    public TCustom? AsCustom<TCustom>(object? source, NoParamOrder npo = default, bool mock = false)
        where TCustom : class, IDataWrapper
    {
        var settings = new WrapDataSettings { ItemIsStrict = true, UseMock = mock };
        return source switch
        {
            null when !mock => null,
            TCustom alreadyT => alreadyT,
            IEntity entity => AsCustomFrom<TCustom, ITypedItem>(AsItem(entity, settings), settings),
            _ => AsCustomFrom<TCustom, ITypedItem>(source as ITypedItem ?? AsItem(source, settings), settings)
        };
    }

    [return: NotNullIfNotNull("item")]
    public TCustom? AsCustomFrom<TCustom, TData>(TData? item, WrapDataSettings? settings)
        where TCustom : class, IDataWrapper
    {
        if (item == null)
            return null;
        if (item is TCustom t)
            return t;

        var bestType = ModelAnalyseUse.GetTargetType<TCustom>();
        var newT = ActivatorUtilities.CreateInstance(serviceProvider, bestType) as TCustom;

        switch (newT)
        {
            // 1. Direct setup from the data type specified, no further conversions
            // Should be an ITypedItemWrapper, but not enforced in the signature
            case IDataWrapperNeedingFactoryWip<TData> withMatchingSetup:
                withMatchingSetup.Setup(item, this);
                return newT;

            // 2. Setup from Item, a more complex object which already has more features
            // In some cases the type of the data is already a model, so we need to unwrap it
            case IDataWrapperNeedingFactoryWip<ITypedItem> forItem when item is ICanBeItem canBeItem:
                forItem.Setup(canBeItem.Item, this);
                return newT;

            // 3. Setup from Entity, the most basic object
            // DataModelOfEntity can also be filled from Typed (but ATM not the other way around)
            case IDataWrapperNeedingFactoryWip<IEntity> forEntity when item is ICanBeEntity canBeEntity:
                forEntity.Setup(canBeEntity.Entity, this);
                return newT;

            // 4. Setup from item, when starting with an entity
            // In some cases we can only wrap an item, but the data is an entity-based model
            case IDataWrapperNeedingFactoryWip<ITypedItem> forTypedItem when item is ICanBeEntity canBeEntity:
                settings ??= new() { ItemIsStrict = true };
                var asItem = AsItem(canBeEntity.Entity, settings);
                // TODO: #ConvertItemSettings
                forTypedItem.Setup(asItem, this);
                return newT;
            default:
                throw new($"The custom type {typeof(TCustom).Name} does not implement 'ICanWrap<TData>'. This is probably a mistake.");
        }
    }

    public TCustom? GetOne<TCustom>(Func<IEntity?> getItem, object id, bool skipTypeCheck)
        where TCustom : class, IDataWrapper
    {
        var item = getItem();
        if (item == null)
            return null;

        // Skip Type-Name check
        if (skipTypeCheck)
            return AsCustom<TCustom>(item);

        // Do Type-Name check
        //var typeNames = DataModelAnalyzer.GetContentTypeNamesList<TCustom>();
        
        // Check all type names if they are `*` or match the data ContentType
        DataModelAnalyzer.IsTypeNameAllowedOrThrow<TCustom>(item, id, skipTypeCheck);
        //if (typeNames.Any(t => t == ModelSourceAttribute.ForAnyContentType || item.Type.Is(t)))
            return AsCustom<TCustom>(item);

        //throw new(
        //    $"Item with ID {id} is not a '{string.Join(",", typeNames)}'. " +
        //    $"This is probably a mistake, otherwise use '{nameof(skipTypeCheck)}: true' " +
        //    $"or apply an attribute [{nameof(ModelSourceAttribute)}({nameof(ModelSourceAttribute.ContentType)} = \"expected-type-name\")] to your model class. "
        //);
    }

    /// <summary>
    /// Create list of custom-typed ITypedItems
    /// </summary>
    public IEnumerable<TCustom> AsCustomList<TCustom>(object? source, NoParamOrder npo, bool nullIfNull)
        where TCustom : class, IDataWrapper
    {
        return source switch
        {
            // Note: for simplicity, the interface always claims to be non-null, since it will only ever be null if clearly demanded
            null => nullIfNull
                ? null!
                : new List<TCustom>(),
            IEnumerable<TCustom> alreadyListT => alreadyListT,
            // special case: empty list, with hidden info about where it's from so the toolbar can adjust and provide new-buttons
            ListTypedItems<ITypedItem> { Count: 0, Entity: not null } list => new ListTypedItems<TCustom>(new List<TCustom>(), list.Entity),
            _ => new ListTypedItems<TCustom>(SafeItems()
                    .Select(item => AsCustomFrom<TCustom, ITypedItem>(item, new() { ItemIsStrict = true })),
                null
            )
        };

        // Helper function to be called from above to ensure that
        // the source is a list of ITypedItems
        IEnumerable<ITypedItem> SafeItems()
            => source switch
            {
                null => [],
                IEnumerable enumerable when !enumerable.Cast<object>().Any() => [],
                IEnumerable<ITypedItem> alreadyOk => alreadyOk,
                // TODO: #ConvertItemSettings - NOT SURE IF THIS DEFAULT IS CORRECT
                _ => AsItems(source, new() { ItemIsStrict = false })
            };
    }

}