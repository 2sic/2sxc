using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using IAsset = ToSic.Sxc.Adam.IAsset;
using IFile = ToSic.Sxc.Adam.IFile;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class AppAssetFile: IFile
{
    protected const string NotImplemented = "not implemented";
    public virtual string Url => NotImplemented;
    public virtual string Type => NotImplemented;

    public virtual string Name => NotImplemented;
    public virtual string Path => NotImplemented;
    public virtual string PhysicalPath => NotImplemented;
    public virtual string Extension => NotImplemented;
    public virtual string Folder => NotImplemented;
    public virtual string FullName => NotImplemented;

    #region Metadata - won't do anything useful

    bool IAsset.HasMetadata => false;

    IMetadataOf IHasMetadata.Metadata => null;

    IMetadata IAsset.Metadata => null;

    #endregion

    #region Properties which are simply not implemented ATM

    public int Size => 0;
    public ISizeInfo SizeInfo => new SizeInfo(0);
    public int FolderId => -1;

    public IField Field { get; set; }
    public DateTime Created => DateTime.Now;
    public int Id => -1;
    public int ParentId => -1;
    public DateTime Modified => DateTime.MinValue;

    #endregion
}