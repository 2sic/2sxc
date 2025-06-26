namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Represents a generated file.
/// It's usually provided in a <see cref="IGeneratedFileSet"/>, which contains additional specs.
/// </summary>
[WorkInProgressApi("still being standardized")]
public class GeneratedFile: IGeneratedFile
{
    /// <inheritdoc />
    public required string FileName { get; init; }

    /// <inheritdoc />
    public required string Path { get; init; }

    /// <inheritdoc />
    public required string Body { get; init; }

    /// <inheritdoc />
    public IReadOnlyCollection<IGeneratedFileInfo> Dependencies { get; init; } = [];
}