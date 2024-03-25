namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Data which describes a code file which will be created or may already exist.
///
/// It's used as the foundation for <see cref="IGeneratedFile"/>
/// but also to reference dependencies to other files created or which should be created in tandem.
/// </summary>
[WorkInProgressApi("still being standardized")]
public interface IGeneratedFileInfo
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
}