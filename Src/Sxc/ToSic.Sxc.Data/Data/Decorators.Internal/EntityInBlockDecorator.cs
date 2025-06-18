using ToSic.Eav.Data.EntityDecorators.Sys;

namespace ToSic.Sxc.Data.Internal.Decorators;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EntityInBlockDecorator: EntityInListDecorator
{
    private EntityInBlockDecorator(string field, 
        int index = DefIndex,
        IEntity presentation = DefPresentation, 
        bool isDemoItem = DefDemo,
        IEntity? parent = default)
        :base(field, index, parent: parent)
    {
        Presentation = presentation;
        IsDemoItem = isDemoItem;
    }

    public static EntityWithDecorator<EntityInBlockDecorator> Wrap(
        IEntity entity,
        string field,
        int index = DefIndex,
        IEntity presentation = DefPresentation,
        bool isDemoItem = DefDemo, 
        IEntity? parent = default) =>
        new(entity, new(field, index, presentation, isDemoItem, parent: parent));

    protected const IEntity DefPresentation = null;
    protected const bool DefDemo = false;


    /// <summary>
    /// Presentation entity of this content-item.
    /// Important to keep content and presentation linked together
    /// </summary>
    public IEntity Presentation { get; set; }

    /// <summary>
    /// Info if the item is a plain demo/fake item, or if it was added on purpose.
    /// new 2019-09-18 trying to mark demo-items for better detection in output #1792
    /// </summary>
    internal bool IsDemoItem { get; }
}