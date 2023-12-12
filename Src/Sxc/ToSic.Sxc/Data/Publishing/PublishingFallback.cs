namespace ToSic.Sxc.Data;

internal class PublishingFallback: IPublishing
{
    private readonly ITypedItem _item;

    public PublishingFallback(ITypedItem item)
    {
        _item = item;
    }

    public bool IsSupported => false;
    //public bool IsPublished => true;

    public bool HasPublished => true;
    public bool HasUnpublished => false;
    public bool HasDraft => false;

    public ITypedItem GetPublished() => _item;

    public ITypedItem GetUnpublished() => null;

    public ITypedItem GetOpposite() => null;
}