namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// WIP Parameters to give the code generator
/// </summary>
internal class FileGeneratorSpecs : IFileGeneratorSpecs
{
    /// <inheritdoc />
    public int AppId { get; init; }

    /// <inheritdoc />
    public string Edition { get; init; }

    public DateTime DateTime { get; init; } = DateTime.Now;
}