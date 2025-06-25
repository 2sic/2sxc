using ToSic.Eav.Data.EntityDecorators.Sys;
using ToSic.Eav.Data.Sys.EntityDecorators;

namespace ToSic.Sxc.Data.Sys.Decorators;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EntityInBlockDecorator: EntityInListDecorator
{
    private EntityInBlockDecorator(string? fieldName, 
        int index = DefIndex,
        IEntity? presentation = DefPresentation, 
        bool isDemoItem = DefDemo,
        IEntity? parent = default)
        :base(fieldName, index, parent: parent)
    {
        Presentation = presentation;
        IsDemoItem = isDemoItem;
    }

    public static EntityWithDecorator<EntityInBlockDecorator> Wrap(
        IEntity entity,
        string? fieldName,
        int index = DefIndex,
        IEntity? presentation = DefPresentation,
        bool isDemoItem = DefDemo, 
        IEntity? parent = default)
        => new(entity, new(fieldName, index, presentation, isDemoItem, parent: parent));

    protected const IEntity? DefPresentation = null;
    protected const bool DefDemo = false;


    /// <summary>
    /// Presentation entity of this content-item.
    /// Important to keep content and presentation linked together
    /// </summary>
    public IEntity? Presentation { get; set; }

    /// <summary>
    /// Info if the item is a plain demo/fake item, or if it was added on purpose.
    /// new 2019-09-18 trying to mark demo-items for better detection in output #1792
    /// </summary>
    internal bool IsDemoItem { get; }
}