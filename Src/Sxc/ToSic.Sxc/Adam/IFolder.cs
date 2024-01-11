namespace ToSic.Sxc.Adam;

/// <summary>
/// An ADAM (Automatic Digital Asset Management) folder.
/// This simple interface assumes that it uses int-IDs.
/// </summary>
[PublicApi]
public interface IFolder: Eav.Apps.Assets.IFolder, IAsset
{
    /// <summary>
    /// Get the files in this folder
    /// </summary>
    /// <returns>A list of files in this folder as <see cref="IFile"/></returns>
    IEnumerable<IFile> Files { get; }

    /// <summary>
    /// Get the sub-folders in this folder
    /// </summary>
    /// <returns>A list of folders inside this folder - as <see cref="IFolder"/>.</returns>
    IEnumerable<IFolder> Folders { get; }
}