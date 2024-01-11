using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services;

/// <summary>
/// WIP - Adam Service for additional ADAM operations such as retrieving a single file
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("WIP 14.04")]
public interface IAdamService
{
    /// <summary>
    /// Retrieve a file by int-id (usually the ID managed by the platform)
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>The file object or null if not found or something else went wrong.</returns>
    IFile File(int id);

    /// <summary>
    /// Retrieve a file using the string-key such as "file:72"
    /// </summary>
    /// <param name="id">File ID String</param>
    /// <returns>The file object or null if not found or something else went wrong.</returns>
    IFile File(string id);

    /// <summary>
    /// Retrieve a file referenced in the field
    /// </summary>
    /// <param name="field"></param>
    /// <returns>The file object or null if not found or something else went wrong.</returns>
    IFile File(IField field);


    /// <summary>
    /// Provides an Adam Folder for the ID
    /// </summary>
    /// <param name="id">Folder ID</param>
    /// <returns>An Adam object for navigating the assets</returns>
    IFolder Folder(int id);

    /// <summary>
    /// Provides an Adam Folder for this item and field
    /// </summary>
    /// <param name="field">The Field information object for which to get the folder</param>
    /// <returns>An Adam object for navigating the assets</returns>
    IFolder Folder(IField field);
}