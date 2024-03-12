namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Bundle of code files to generate, including some information about the generator.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICodeFileBundle
{
    /// <summary>
    /// Name for easy identification.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Description to show in the UI
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Name of the generator, possibly with version
    /// </summary>
    string Generator { get; set; }

    /// <summary>
    /// The path, but not sure yet how to do, especially if it will be relative or contain editions.
    /// </summary>
    string Path { get; set; }

    /// <summary>
    /// List of files to generate
    /// </summary>
    List<ICodeFile> Files { get; set; }
}