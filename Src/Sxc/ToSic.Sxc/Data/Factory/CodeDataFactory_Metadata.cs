using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data;

partial class CodeDataFactory
{
    public IMetadata Metadata(IMetadataOf mdOf) => new Metadata(mdOf, this);
}