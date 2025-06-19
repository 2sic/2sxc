using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam;

public class MockSxcFolder: MockFolder, IFolder
{
    private IMetadataOf _metadata;
    private ITypedMetadata _metadata1;
    public bool HasMetadata { get; }

    ITypedMetadata IAsset.Metadata => _metadata1;

    public string Url { get; init; }
    public string Type { get; init; }

    IMetadataOf IHasMetadata.Metadata => _metadata;

    public IField Field { get; set; }
    public IEnumerable<IFile> Files { get; init; }
    public IEnumerable<IFolder> Folders { get; init; }
}