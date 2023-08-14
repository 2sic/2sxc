using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data
{
    public partial class CodeDataFactory
    {
        public IMetadata Metadata(IMetadataOf mdOf) => new Metadata(mdOf, /*null,*/ this);
    }
}
