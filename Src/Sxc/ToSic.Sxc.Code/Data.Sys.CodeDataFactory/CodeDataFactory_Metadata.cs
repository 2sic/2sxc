using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    public ITypedMetadata MetadataDynamic(IMetadata mdOf)
        => new Metadata.Metadata(mdOf, this);

    public ITypedMetadata MetadataTyped(IMetadata mdOf)
        => new Metadata.Metadata(mdOf, this);
}