﻿using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    public ITypedMetadata Metadata(IMetadata mdOf)
        => new Metadata.Metadata(mdOf, this);
}