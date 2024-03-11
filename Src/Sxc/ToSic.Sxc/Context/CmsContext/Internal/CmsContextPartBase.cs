using ToSic.Eav.Metadata;
using ToSic.Lib.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class CmsContextPartBase<T> : Wrapper<T>, IHasMetadata where T : class
{
    protected CmsContextPartBase(CmsContext parent, T contents) : base(contents)
    {
        Parent = parent;
    }
    protected CmsContext Parent;

    protected CmsContextPartBase() : base(null) { }

    protected void Init(CmsContext parent, T contents)
    {
        Wrap(contents);
        Parent = parent;
    }

    public IMetadata Metadata => _dynMeta.Get(() => Parent._CodeApiSvc.Cdf.Metadata((this as IHasMetadata).Metadata));
    private readonly GetOnce<IMetadata> _dynMeta = new();

    IMetadataOf IHasMetadata.Metadata => _md.Get(GetMetadataOf);
    private readonly GetOnce<IMetadataOf> _md = new();

    protected abstract IMetadataOf GetMetadataOf();

    protected IMetadataOf ExtendWithRecommendations(IMetadataOf md)
    {
        if (md == null) return null;
        md.Target.Recommendations = [Decorators.NoteDecoratorName];
        return md;
    }
}