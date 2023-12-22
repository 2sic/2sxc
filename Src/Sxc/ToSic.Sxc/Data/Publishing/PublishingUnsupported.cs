namespace ToSic.Sxc.Data;

internal class PublishingUnsupported(ITypedItem item) : IPublishing
{
    public bool IsSupported => false;

    public bool HasPublished => true;

    public bool HasUnpublished => false;

    public bool HasBoth => false;

    public ITypedItem GetPublished() => item;

    public ITypedItem GetUnpublished() => null;

    public ITypedItem GetOpposite() => null;
}