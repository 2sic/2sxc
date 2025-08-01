using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Cms.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Field;
using ToSic.Sxc.Images.Sys;

namespace ToSic.Sxc.Adam.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class File<TFolderId, TFileId>(AdamManager adamManager)
    : Eav.Apps.Assets.Sys.File<TFolderId, TFileId>,
        IFile,
        IHasLink
{
    protected AdamManager AdamManager { get; } = adamManager;

    #region Metadata

    /// <inheritdoc />
    [JsonIgnore]
    [field: AllowNull, MaybeNull]
    public ITypedMetadata Metadata => field
        ??= AdamManager.CreateMetadataTyped($"{CmsMetadata.FilePrefix}{SysId}", FullName, AttachMdRecommendations);

    /// <summary>
    /// Attach metadata recommendations
    /// </summary>
    /// <param name="mdOf"></param>
    protected void AttachMdRecommendations(IMetadata mdOf)
    {
        if (mdOf?.Target == null || Type != Classification.Image)
            return;
        mdOf.Target.Recommendations = AdamManager.Cdf
            .GetService<IImageMetadataRecommendationsService>()
            .GetImageRecommendations();
    }

    IMetadata IHasMetadata.Metadata => (Metadata as IHasMetadata).Metadata;

    /// <inheritdoc />
    [JsonIgnore]
    public bool HasMetadata => (Metadata as IHasMetadata)?.Metadata.Any() ?? false;


    #endregion

    public string? Url { get; set; }

    public string Type => Classification.TypeName(Extension);


    [PrivateApi]
    public IField? Field { get; set; }
}