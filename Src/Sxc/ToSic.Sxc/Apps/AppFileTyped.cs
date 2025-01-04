using Custom.Data;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps;

/// <summary>
/// An App File Entity for typed use.
/// It defines the schema for a file as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also 
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still beta in v19 as the final name may change.")]
public class AppFileTyped: CustomItem, IAppFileEntity
{
    /// <inheritdoc />
    public string Name => _item.String(nameof(Name));
    /// <inheritdoc />
    public string Extension => _item.String(nameof(Extension));
    /// <inheritdoc />
    public string FullName => _item.String(nameof(FullName));
    /// <inheritdoc />
    public string Path => _item.String(nameof(Path));

    /// <summary>
    /// Reference to the folder this file is in.
    /// </summary>
    public new AppFolderTyped Folder => _item.Child<AppFolderTyped>(nameof(Folder));

    /// <inheritdoc />
    public int Size => _item.Int(nameof(Size));

    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    /// <inheritdoc cref="IAppFileEntity.Url" />
    /// <remarks>
    /// It hides the base method <see cref="ITypedItem.Url"/>.
    /// </remarks>
    public new string Url => _item.String(nameof(Url));

    /// <inheritdoc />
    public DateTime Created => _item.DateTime(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _item.DateTime(nameof(Modified));

}