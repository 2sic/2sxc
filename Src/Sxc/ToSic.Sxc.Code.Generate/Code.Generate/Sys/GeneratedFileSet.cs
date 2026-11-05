namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// WIP - should contain a list of code-files and additional specs
/// </summary>
[WorkInProgressApi("still being standardized")]
public class GeneratedFileSet : IGeneratedFileSet
{
    /// <inheritdoc />
    public required string Name { get; init; }

    /// <inheritdoc />
    public required string Description { get; init; }

    /// <inheritdoc />
    public required string Generator { get; init; }

    /// <inheritdoc />
    public required string Path { get; init; }

    /// <inheritdoc />
    public required IReadOnlyCollection<IGeneratedFile> Files { get; init; }

    public static GeneratedFileSet FromSpecs(FileGeneratorSpecs specs, string name, IReadOnlyCollection<IGeneratedFile> files) =>
        new()
        {
            Name = name,
            Description = specs.Description,
            Generator = $"{specs.Name} v{specs.Version}",
            Path = GenerateConstants.PathToAppCode,
            Files = files
        };
}