using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem: IHasMetadata
    {
        public IMetadataTyped Metadata => _typedMd.Get(() => new MetadataTyped(DynEntity.Metadata, _typedHelpers));
        private readonly GetOnce<IMetadataTyped> _typedMd = new GetOnce<IMetadataTyped>();

        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;

    }
}
