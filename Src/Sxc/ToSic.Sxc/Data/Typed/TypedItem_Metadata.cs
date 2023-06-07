using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem: IHasMetadata
    {
        public ITypedMetadata Metadata => _typedMd.Get(() => new TypedMetadata(DynEntity.Metadata));
        private readonly GetOnce<ITypedMetadata> _typedMd = new GetOnce<ITypedMetadata>();

        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;

    }
}
