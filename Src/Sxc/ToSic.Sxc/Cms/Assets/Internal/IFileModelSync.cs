namespace ToSic.Sxc.Cms.Assets.Internal;

internal interface IFileModelSync
{
    /// <summary>
    /// The file name without extension.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The file name extension, without any dot.
    /// Purpose is to do switching between extensions.
    /// If you want to have a safe, merged file name, just take the `FullName`.
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// The size in bytes.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// The full name with extension.
    /// If it's a folder or there is no extension, then it's identical to the <see cref="Name"/>
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
    DateTime Created { get; }

    /// <summary>
    /// When the file/folder was modified.
    /// </summary>
    DateTime Modified { get; }

}