﻿using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Field;

namespace ToSic.Sxc.Adam;

public class MockSxcFile : Eav.Apps.Assets.MockFile, IFile, IHasLink
{
    private IMetadata _metadata;
    private ITypedMetadata _metadata1;
    public bool HasMetadata { get; init; } = false;

    ITypedMetadata IAsset.Metadata => _metadata1;

    public string Url { get; init; }
    public string Type => Classification.TypeName(Extension);

    IMetadata IHasMetadata.Metadata => _metadata;

    public IField Field { get; set; }
}
