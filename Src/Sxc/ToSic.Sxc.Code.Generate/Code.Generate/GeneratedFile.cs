namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Represents a generated file.
/// It's usually provided in a <see cref="IGeneratedFileSet"/>, which contains additional specs.
/// </summary>
[WorkInProgressApi("still being standardized")]
public class GeneratedFile: IGeneratedFile
{
    /// <inheritdoc />
    public string FileName { get; init; }

    /// <inheritdoc />
    public string Path { get; init; }

    /// <inheritdoc />
    public string Body { get; init; }

    /// <inheritdoc />
    public IReadOnlyCollection<IGeneratedFileInfo> Dependencies { get; init; } = [];
}