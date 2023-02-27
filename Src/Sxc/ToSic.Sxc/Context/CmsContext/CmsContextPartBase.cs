using ToSic.Eav.Metadata;
using ToSic.Lib.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context
{
    public abstract class CmsContextPartBase<T> : Wrapper<T>, IHasMetadata where T : class
    {
        protected CmsContextPartBase(CmsContext parent, T contents) : base(contents)
        {
            _parent = parent;
        }
        private CmsContext _parent;

        protected CmsContextPartBase() : base(null) { }

        protected void Init(CmsContext parent, T contents)
        {
            Wrap(contents);
            _parent = parent;
        }

        public IDynamicMetadata Metadata => _dynMeta.Get(() => new DynamicMetadata((this as IHasMetadata).Metadata, null, _parent.DEDeps));
        private readonly GetOnce<IDynamicMetadata> _dynMeta = new GetOnce<IDynamicMetadata>();

        IMetadataOf IHasMetadata.Metadata => _md.Get(GetMetadataOf);
        private readonly GetOnce<IMetadataOf> _md = new GetOnce<IMetadataOf>();

        protected abstract IMetadataOf GetMetadataOf();

        protected IMetadataOf ExtendWithRecommendations(IMetadataOf md)
        {
            if (md == null) return null;
            md.Target.Recommendations = new[] { Decorators.NoteDecoratorName };
            return md;
        }
    }
}
