using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Adam;

/// <summary>
/// Describes an ADAM (Automatic Digital Asset Management) asset. <br/>
/// This contains properties which both <see cref="IFolder"/> and <see cref="IFile"/> have in common.
/// </summary>
[PublicApi]
public interface IAsset: IHasMetadata, IFromField
{
    #region Metadata
    /// <summary>
    /// Informs the code if this asset has real metadata attached or not. 
    /// </summary>
    /// <returns>True if this asset has metadata, false if it doesn't (in which case the Metadata property still works, but won't deliver any real values)</returns>
    bool HasMetadata { get; }

    /// <summary>
    /// List of metadata items - 
    /// will automatically contain a fake item, even if no metadata exits
    /// to help in razor template etc.
    /// </summary>
    /// <returns>An IDynamicEntity which contains the metadata, or an empty IDynamicEntity which still works if no metadata exists.</returns>
    new IMetadata Metadata { get; }


    #endregion


    /// <summary>
    /// The path to this asset as used from external access.
    /// Must be a full url beginning with a "/" like "/Portals/0/adam/..."
    /// </summary>
    /// <returns>The url to this asset</returns>
    string Url { get; }

    /// <summary>
    /// The type of this asset (folder, file, etc.)
    /// </summary>
    /// <returns>"folder", "image", "document", "file" depending on what it is</returns>
    string Type { get; }
}