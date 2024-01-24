using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    public IMetadata Metadata(IMetadataOf mdOf) => new Metadata.Metadata(mdOf, this);
}