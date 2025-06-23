namespace ToSic.Sxc.Backend.Admin.AppFiles;

/// <summary>
/// helper class with all the info to identify a file in the app folder
/// </summary>
public record AppFileDto
{
    public int AppId { get; init; }

    public required string Path { get; init; }

    public bool Global { get; init; }

    public required string TemplateKey { get; init; }
}