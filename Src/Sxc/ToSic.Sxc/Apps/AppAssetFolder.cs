using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class AppAssetFolder: IFolder
{
    protected const string NotImplemented = "not implemented";

    public virtual string Name => NotImplemented;

    public virtual string Path => NotImplemented;

    public virtual string PhysicalPath => NotImplemented;

    public virtual string Url => NotImplemented;

    public bool HasChildren => false;


    #region Metadata - won't do anything useful

    bool IAsset.HasMetadata => false;

    IMetadataOf IHasMetadata.Metadata => null;

    IMetadata IAsset.Metadata => null;

    #endregion

    #region Properties which are simply not implemented and won't do anything useful

    public int Id => -1;
    public int ParentId => -1;
    public DateTime Created => DateTime.MinValue;
    public DateTime Modified => DateTime.MinValue;

    public string Type => Classification.Folder;

    public IField Field { get; set; }

    private const string FileFoldersNotSupported = "As of now you can't use the App folder to navigate files/folders";

    public IEnumerable<IFile> Files => throw new NotSupportedException(FileFoldersNotSupported);

    public IEnumerable<IFolder> Folders => throw new NotSupportedException(FileFoldersNotSupported);

    #endregion

}