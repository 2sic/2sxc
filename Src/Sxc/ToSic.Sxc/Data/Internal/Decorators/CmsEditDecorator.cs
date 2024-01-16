namespace ToSic.Sxc.Data.Internal.Decorators;

internal class CmsEditDecorator : IDecorator<IEntity>
{
    public bool DisableEdit { get; }

    private CmsEditDecorator(bool enableEdit)
    {
        DisableEdit = !enableEdit;
    }

    public static EntityDecorator12<CmsEditDecorator> Wrap(IEntity entity, bool enableEdit) =>
        new(entity, new(enableEdit));
}