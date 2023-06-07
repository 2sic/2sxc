using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem: IHasMetadata
    {
        public IDynamicMetadata Metadata => _typedMd.Get(() => DynEntity.Metadata);
        private readonly GetOnce<IDynamicMetadata> _typedMd = new GetOnce<IDynamicMetadata>();

        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;

    }
}
