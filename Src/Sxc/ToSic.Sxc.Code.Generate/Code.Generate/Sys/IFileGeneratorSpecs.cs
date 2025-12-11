namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Specs for a file generator.
///
/// An object containing these specs - and sometimes more - is passed to the file generator.
/// </summary>
/// <remarks>
/// WIP v17.04
/// </remarks>
[WorkInProgressApi("still being standardized")]
public interface IFileGeneratorSpecs
{
    /// <summary>
    /// The AppId of the app for which the file is generated.
    /// </summary>
    int AppId { get; }

    /// <summary>
    /// The Edition for which we're generating the file.
    /// </summary>
    string? Edition { get; }

    /// <summary>
    /// The moment the generation was performed.
    /// </summary>
    DateTime DateTime { get; }

    /// <summary>
    /// Alternate namespace to use for the generated code files.
    /// If null, just use automatic / default namespace.
    /// </summary>
    string? Namespace { get; init; }

    /// <summary>
    /// The target path for the generated code files.
    /// If null, use default path.
    /// </summary>
    string? TargetPath { get; init; }

    /// <summary>
    /// Get the scope of the Content Type (like sections in a DB)
    /// </summary>
    string? Scope { get; init; }

    /// <summary>
    /// The content types to generate files for.
    /// If null, use all available content types in the "Default" scope + the app settings/resources.
    /// </summary>
    ICollection<string>? ContentTypes { get; init; }
}