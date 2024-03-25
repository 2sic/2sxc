namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Set of code files to generate, including some information about the generator.
/// </summary>
[WorkInProgressApi("still being standardized")]
public interface IGeneratedFileSet
{
    /// <summary>
    /// Name for easy identification.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description to show in the UI
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Name of the generator, possibly with version
    /// </summary>
    string Generator { get; }

    /// <summary>
    /// The path, but not sure yet how to do, especially if it will be relative or contain editions.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// List of files to generate
    /// </summary>
    IReadOnlyCollection<IGeneratedFile> Files { get; }
}