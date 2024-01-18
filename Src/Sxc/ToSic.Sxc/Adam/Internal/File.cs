using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class File<TFolderId, TFileId>(AdamManager adamManager) : Eav.Apps.Assets.Internal.File<TFolderId, TFileId>,
#if NETFRAMEWORK
#pragma warning disable 618
    ToSic.SexyContent.Adam.AdamFile,
#pragma warning restore 618
#endif
    IFile,
    IHasLink
{
    private AdamManager AdamManager { get; } = adamManager;

    #region Metadata

    /// <inheritdoc />
    [JsonIgnore]
    public IMetadata Metadata => _metadata ??= AdamManager.Create(CmsMetadata.FilePrefix + SysId, FileName, AttachMdRecommendations);
    private IMetadata _metadata;

    /// <summary>
    /// Attach metadata recommendations
    /// </summary>
    /// <param name="mdOf"></param>
    private void AttachMdRecommendations(IMetadataOf mdOf)
    {
        if (mdOf?.Target == null) return;
        if (Type == Classification.Image)
            mdOf.Target.Recommendations = ImageDecorator.GetImageRecommendations(AdamManager?.Cdf?._CodeApiSvc);
    }

    IMetadataOf IHasMetadata.Metadata => (Metadata as IHasMetadata)?.Metadata;

    /// <inheritdoc />
    [JsonIgnore]
    public bool HasMetadata => (Metadata as IHasMetadata)?.Metadata.Any() ?? false;


    #endregion

    public string Url { get; set; }

    public string Type => Classification.TypeName(Extension);



    public string FileName => FullName;

    public DateTime CreatedOnDate => Created;

    public int FileId => SysId as int? ?? 0;


    [PrivateApi]
    public IField Field { get; set; }
}