using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    public ITypedMetadata Metadata(IMetadataOf mdOf)
        => new Metadata.Metadata(mdOf, this);
}