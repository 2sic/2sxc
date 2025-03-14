using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam;

public class MockSxcFile : Eav.Apps.Assets.MockFile, IFile
{
    private IMetadataOf _metadata;
    private IMetadata _metadata1;
    public bool HasMetadata { get; init; } = false;

    IMetadata IAsset.Metadata => _metadata1;

    public string Url { get; init; }
    public string Type { get; init; }

    IMetadataOf IHasMetadata.Metadata => _metadata;

    public IField Field { get; set; }
}
