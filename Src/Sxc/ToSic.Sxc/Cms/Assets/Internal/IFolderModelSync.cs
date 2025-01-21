namespace ToSic.Sxc.Cms.Assets.Internal;

internal interface IFolderModelSync
{
    /// <summary>
    /// The folder name - or blank when it's the root.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The full name with extension.
    /// If it's a folder or there is no extension, then it's identical to the `Name`
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Starting in the App-Root
    /// </summary>
    string Path { get; }

    /// <summary>
    /// The full url starting at the root of the site. Absolute but without protocol/domain.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// When the file/folder was created.
    /// </summary>
    public DateTime Created { get; }

    /// <summary>
    /// When the file/folder was modified.
    /// </summary>
    public DateTime Modified { get; }

}