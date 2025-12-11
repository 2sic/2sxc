namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// WIP Parameters to give the code generator
/// </summary>
internal record FileGeneratorSpecs : IFileGeneratorSpecs
{
    /// <inheritdoc />
    public int AppId { get; init; }

    /// <inheritdoc />
    public string? Edition { get; init; }

    /// <inheritdoc />
    public DateTime DateTime { get; init; } = DateTime.Now;

    /// <inheritdoc />
    public string? Namespace { get; init; }

    /// <inheritdoc />
    public string? TargetPath { get; init; }

    /// <inheritdoc />
    public string? Scope { get; init; }

    /// <inheritdoc />
    public ICollection<string>? ContentTypes { get; init; }
}