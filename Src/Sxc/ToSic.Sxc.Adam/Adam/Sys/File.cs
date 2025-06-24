﻿using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Field;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Adam.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
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
    [field: AllowNull, MaybeNull]
    public ITypedMetadata Metadata => field
        ??= AdamManager.CreateMetadata(CmsMetadata.FilePrefix + SysId, FileName, AttachMdRecommendations);

    /// <summary>
    /// Attach metadata recommendations
    /// </summary>
    /// <param name="mdOf"></param>
    private void AttachMdRecommendations(IMetadata mdOf)
    {
        if (mdOf?.Target == null || Type != Classification.Image)
            return;
        mdOf.Target.Recommendations = AdamManager
                                          ?.Cdf
                                          ?.GetService<IImageMetadataRecommendationsService>()
                                          .GetImageRecommendations()
                                      ?? [];
    }

    IMetadata IHasMetadata.Metadata => (Metadata as IHasMetadata).Metadata;

    /// <inheritdoc />
    [JsonIgnore]
    public bool HasMetadata => (Metadata as IHasMetadata)?.Metadata.Any() ?? false;


    #endregion

    public string? Url { get; set; }

    public string Type => Classification.TypeName(Extension);



    public string FileName => FullName;

    public DateTime CreatedOnDate => Created;

    public int FileId => SysId as int? ?? 0;


    [PrivateApi]
    public IField? Field { get; set; }
}