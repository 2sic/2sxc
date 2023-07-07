using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {
        public IMetadata Metadata(IMetadataOf mdOf) => new Metadata(mdOf, null, DynamicEntityServices);
    }
}
