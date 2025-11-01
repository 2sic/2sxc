using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Sxc.Data.Options;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sys.Performance;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Data.Sys.Dynamic;


internal class GetAndConvertConverter(ICodeDataFactory cdf, bool propsRequired, bool childrenShouldBeDynamic, ICanDebug canDebug, IValueOverrider? overrider = default)
{
    [field: AllowNull, MaybeNull]
    internal SubDataFactory SubDataFactory => field ??= new(cdf, propsRequired, canDebug);

    internal object? ValueAutoConverted(PropReqResult original, bool lookupLink, string field, ILog? logOrNull)
    {
        var l = logOrNull.Fn<object?>($"..., {nameof(lookupLink)}: {lookupLink}, {nameof(field)}: {field}");
        var value = original.Result;
        var parent = original.Source as IEntity;

        // If it's a reference like "file:72", try to convert it
        if (lookupLink && value is string strMaybeLink && original.ValueType == ValueTypesWithState.Hyperlink)
        {
            var strMaybeReference = overrider != null
                ? overrider.ProcessString(field, strMaybeLink)
                : strMaybeLink;
            if (ValueConverterBase.CouldBeReference(strMaybeReference))
            {
                l.A("Try to convert value");
                // ReSharper disable once ConstantNullCoalescingCondition - paranoid
                value = cdf.Services.ValueConverter.ToValue(strMaybeReference, parent?.EntityGuid ?? Guid.Empty) ?? value;
                return l.Return(value, "link-conversion");
            }
        }


        // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
        // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
        if (value is IEnumerable<IEntity> children)
        {
            if (childrenShouldBeDynamic)
            {
                l.A("Convert entity list as DynamicEntity");
                var dynEnt = cdf.AsDynamicFromEntities(children.ToArray(), new() { ItemIsStrict = propsRequired }, parent: parent, field: field);
                if (canDebug.Debug)
                    dynEnt.Debug = true;
                return l.Return(dynEnt, "ent-list, now dyn");
            }
            l.A($"Convert entity list as {nameof(ITypedItem)}");
            var converted = AsChildrenItems(entities: children, field: field, parentEntity: parent, new());

            // if (Debug) converted.ForEach(c => c.Debug = true);
            return l.Return(converted, "ent-list, now dyn");
        }

        // special debug of path if possible
        if (canDebug.Debug)
            try
            {
                var finalPath = string.Join(" > ", original.Path?.Parts?.ToArray() ?? []);
                l.A($"Debug path: {finalPath}");
            }
            catch {/* ignore */}

        if (value is string strResult && overrider != null)
        {
            var maybeOverride = overrider.ProcessString(field, strResult);
            return l.Return(maybeOverride, "parsed");
        }

        return l.Return(value, "unmodified");
    }
    #region Parents / Children - ATM still dynamic

    public List<IDynamicEntity?> ParentsDyn(IEntity entity, string? type, string? field)
        => entity.Parents(type, field)
            .Select(SubDataFactory.SubDynEntityOrNull)
            .ToList();

    public List<ITypedItem> ParentsItems(IEntity entity, string? type, string? field, GetRelatedOptions options)
    {
        var list = entity.Parents(type, field);
        var processed = ProcessOptions(list, options, cdf.Services.User);

        var preserveNull = options.ProcessNull == ProcessNull.Preserve;

        return processed
            .Select(ITypedItem (e) => e == null && preserveNull
                ? null!
                : new TypedItemOfEntity(e!, cdf, propsRequired)
            )
            .ToList();
    }


    public List<IDynamicEntity?> ChildrenDyn(IEntity entity, string? field, string? type)
        => AsChildrenDyn(entity.Children(field, type), field, parentEntity: entity);

    public List<ITypedItem> ChildrenItems(IEntity entity, string field, string? type, GetRelatedOptions options)
        => AsChildrenItems(entity.Children(field, type), field, parentEntity: entity, options);

    private List<IDynamicEntity?> AsChildrenDyn(IEnumerable<IEntity?> entities, string? field, IEntity? parentEntity)
        => AsChildrenOf(
            ProcessOptions(entities, new(), cdf.Services.User),
            field,
            parentEntity,
            SubDataFactory.SubDynEntityOrNull,
            new()
        );

    private List<ITypedItem> AsChildrenItems(IEnumerable<IEntity?> entities, string field, IEntity? parentEntity, GetRelatedOptions options)
        => AsChildrenOf(
            ProcessOptions(entities, options, cdf.Services.User),
            field,
            parentEntity,
            ITypedItem (e) => new TypedItemOfEntity(e, cdf, propsRequired),
            options
        );

    private static List<T> AsChildrenOf<T>(
        IEnumerable<IEntity?> entities,
        string? fieldNameForWrapperInfo,
        IEntity? parentEntity,
        Func<IEntity, T> convert,
        GetRelatedOptions options)
        where T : class?, ICanBeEntity?
    {
        var preserveNull = options.ProcessNull == ProcessNull.Preserve;

        var list = entities
            .Select((e, i) =>
            {
                if (e == null && preserveNull)
                    return null!;
                var wrapped = EntityInBlockDecorator.Wrap(entity: e!, fieldName: fieldNameForWrapperInfo, index: i, parent: parentEntity);
                var converted = convert(wrapped);
                return converted;
            })
            .ToList();
        return new ListTypedItems<T>(list, null);
    }

    private static ICollection<IEntity?> ProcessOptions(IEnumerable<IEntity?> entities, GetRelatedOptions options, IUser user)
    {
        // 1. Check if we should remove drafts - default for non-admins
        if (options.ProcessDraft == ProcessDraft.NoDraft ||
            (options.ProcessDraft == ProcessDraft.Auto && !user.IsContentEditor))
            entities = entities
                .Where(e => e?.IsPublished == true);

        // 2. filter out nulls, as the razor code will usually not be able to handle them
        // in future, we should maybe add a trigger to optionally allow nulls,
        // in which case the result should then also be null
        var preserveNull = options.ProcessNull == ProcessNull.Preserve;
        if (!preserveNull)
            entities = entities
                .Where(e => e != null);

        return entities.ToListOpt();
    }

    #endregion
}
