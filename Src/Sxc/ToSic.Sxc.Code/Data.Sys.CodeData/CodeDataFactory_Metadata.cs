using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Sys.Metadata;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    public ITypedMetadata Metadata(IMetadata mdOf)
        => new Metadata(mdOf, this);
}