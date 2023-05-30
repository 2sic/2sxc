using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem: IHasMetadata
    {
        //public IDynamicMetadata Metadata => DynEntity.Metadata;

        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;

    }
}
