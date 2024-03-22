namespace ToSic.Sxc.Code.Generate;

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
    string Edition { get; }

    DateTime DateTime { get; }
}