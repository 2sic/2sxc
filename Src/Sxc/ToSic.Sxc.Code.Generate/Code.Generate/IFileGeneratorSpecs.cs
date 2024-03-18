namespace ToSic.Sxc.Code.Generate;

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
}