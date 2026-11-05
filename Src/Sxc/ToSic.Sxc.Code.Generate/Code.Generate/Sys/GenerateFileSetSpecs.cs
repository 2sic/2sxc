namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Specs for a file generator.
/// </summary>
[WorkInProgressApi("still being standardized")]
public record GenerateFileSetSpecs
{
    /// <summary>
    /// ID and name of the configuration used.
    /// If null, configuration is not used.
    /// </summary>
    public string? Configuration { get; init; }

    /// <summary>
    /// The AppId of the app for which the file is generated.
    /// </summary>
    public int AppId { get; init; }

    /// <summary>
    /// The Edition for which we're generating the file.
    /// </summary>
    public string? Edition { get; init; }

    /// <summary>
    /// The date and time when the file set is generated.
    /// </summary>
    public DateTime DateTime { get; init; } = DateTime.Now;

    /// <summary>
    /// Alternate namespace to use for the generated code files.
    /// If null, just use automatic / default namespace.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// The target path for the generated code files.
    /// If null, use default path.
    /// </summary>
    public string? TargetPath { get; init; }

    /// <summary>
    /// The content types to generate files for.
    /// If null, use all available content types in the "Default" scope + the app settings/resources.
    /// </summary>
    public ICollection<string>? ContentTypes { get; init; }

    /// <summary>
    /// Prefix for generated class and file names.
    /// If null, don't change the default naming.
    /// </summary>
    public string? Prefix { get; init; }

    /// <summary>
    /// Suffix for generated class and file names.
    /// If null, don't change the default naming.
    /// </summary>
    public string? Suffix { get; init; }
}
