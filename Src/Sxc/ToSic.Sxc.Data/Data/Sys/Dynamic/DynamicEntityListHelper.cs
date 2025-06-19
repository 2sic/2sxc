using ToSic.Sxc.Data.Internal.Decorators;

namespace ToSic.Sxc.Data.Internal.Dynamic;

/// <summary>
/// This is a helper in charge of the list-behavior of a DynamicEntity
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class DynamicEntityListHelper
{
    protected bool PropsRequired { get; }
    public readonly IEntity? ParentOrNull;
    public readonly string? FieldNameOrNull;
    private readonly ICodeDataFactory _cdf;

    private readonly Func<bool> _getDebug;

    public DynamicEntityListHelper(IDynamicEntity singleItem, Func<bool> getDebug, bool propsRequired, ICodeDataFactory cdf)
        : this(cdf, propsRequired, getDebug)
    {
        DynEntities = [singleItem ?? throw new ArgumentException(nameof(singleItem))];
    }
        
    public DynamicEntityListHelper(IEnumerable<IEntity>? entities, IEntity? parentOrNull, string? fieldNameOrNull, Func<bool> getDebug, bool propsRequired, ICodeDataFactory cdf)
        : this(cdf, propsRequired, getDebug)
    {
        ParentOrNull = parentOrNull;
        FieldNameOrNull = fieldNameOrNull;
        _entities = entities?.ToArray() ?? throw new ArgumentNullException(nameof(entities));
    }

    private DynamicEntityListHelper(ICodeDataFactory cdf, bool propsRequired, Func<bool> getDebug)
    {
        _cdf = cdf ?? throw new ArgumentNullException(nameof(cdf));
        PropsRequired = propsRequired;
        _getDebug = getDebug;
    }

    /// <summary>
    /// Alternate source list if DynEntities was not set.
    /// </summary>
    private readonly IEntity[]? _entities;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    public List<IDynamicEntity?> DynEntities
    {
        get
        {
            // Case #1 & Case #2- already created before or because of Single-Item
            if (field != null)
                return field;

            // Case #3 - Real sub-list
            // If it has a parent, it should apply numbering to the things inside
            // If not, it's coming from a stream or something and shouldn't do that
            var reWrapWithListNumbering = ParentOrNull != null;

            var debug = _getDebug.Invoke();
            return field = _entities!
                .Select(IDynamicEntity? (e, i) =>
                {
                    // If we should re-wrap, we create an Entity with some metadata-decoration, so that toolbars know it's part of a list
                    var blockEntity = reWrapWithListNumbering
                        ? EntityInBlockDecorator.Wrap(entity: e, fieldName: FieldNameOrNull, index: i, parent: ParentOrNull)
                        : e;
                    return SubDataFactory.SubDynEntityOrNull(blockEntity, _cdf, debug, propsRequired: PropsRequired);
                })
                .ToList();
        }
    }
}