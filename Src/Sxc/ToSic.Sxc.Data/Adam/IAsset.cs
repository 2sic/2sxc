using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Field;

namespace ToSic.Sxc.Adam;

/// <summary>
/// Describes an ADAM (Automatic Digital Asset Management) asset. <br/>
/// This contains properties which both <see cref="IFolder"/> and <see cref="IFile"/> have in common
/// in addition to what they inherit from the EAV
/// </summary>
[PublicApi]
public interface IAsset: IHasMetadata, IFromField
{
    #region Metadata
    /// <summary>
    /// Tells you if this asset has real metadata attached or not. 
    /// </summary>
    /// <returns>
    /// `true` if this asset has metadata, `false` if it doesn't (in which case the Metadata property still works, but won't deliver any real values)
    /// </returns>
    bool HasMetadata { get; }

    /// <summary>
    /// List of metadata items - 
    /// will automatically contain a fake item, even if no metadata exits
    /// to help in razor template etc.
    /// </summary>
    /// <returns>
    /// An `IMetadata` which contains the metadata, or an empty IMetadata which still works if no metadata exists.
    /// </returns>
    new ITypedMetadata Metadata { get; }


    #endregion


    /// <summary>
    /// The path to this asset as used from external access.
    /// Must be a full url beginning with a "/" like "/Portals/0/adam/..."
    /// </summary>
    /// <returns>
    /// The url to this asset.
    /// `/Portals/0/2sxc/content/assets/docs/terms/file.pdf` for `C:\Inetpub\wwwroot\www.2sic.com\Portals\0\2sxc\content\assets\docs\terms\file.pdf`
    /// </returns>
    string Url { get; }

    /// <summary>
    /// The type of this asset (folder, file, etc.) - typically for adding icons or similar when listing assets.
    /// </summary>
    /// <returns>
    /// `folder`, `image`, `document`, `file` depending on what it is.
    /// `document` for `C:\Inetpub\wwwroot\www.2sic.com\Portals\0\2sxc\content\assets\docs\terms\file.pdf`
    /// </returns>
    string Type { get; }
}