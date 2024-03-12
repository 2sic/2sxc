namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Data which is meant to create a new file.
/// The file will usually be saved directly to the file system of the App.
///
/// This is WIP - goal is to make it more standard so others could create generators too.
/// </summary>
public interface ICodeFile
{
    /// <summary>
    /// The file name of the final code file, with extension.
    /// May not contain any slashes.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// The path to put the file in, relative to the root which is determined elsewhere.
    /// May not begin or end with a slash.
    ///
    /// Example: if path is "Data" and the root is "AppCode", the file will be saved to "AppCode\Data\FileName.cs"
    /// </summary>
    string Path { get; }

    /// <summary>
    /// The body of the file, which will be written to the file.
    /// </summary>
    string Body { get; }
}