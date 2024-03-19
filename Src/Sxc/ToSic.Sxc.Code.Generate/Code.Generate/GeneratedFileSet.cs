namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// WIP - should contain a list of code-files and additional specs
/// </summary>
[WorkInProgressApi("still being standardized")]
public class GeneratedFileSet : IGeneratedFileSet
{
    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public string Description { get; init; }

    /// <inheritdoc />
    public string Generator { get; init; }

    /// <inheritdoc />
    public string Path { get; init; }

    /// <inheritdoc />
    public IReadOnlyCollection<IGeneratedFile> Files { get; init; }
}