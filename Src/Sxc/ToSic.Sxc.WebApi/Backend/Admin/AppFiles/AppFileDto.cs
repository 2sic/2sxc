namespace ToSic.Sxc.Backend.Admin.AppFiles;

/// <summary>
/// helper class with all the info to identify a file in the app folder
/// </summary>
public class AppFileDto
{
    public int AppId { get; set; }

    public string Path { get; set; }

    public bool Global { get; set; }

    public string TemplateKey { get; set; }
}