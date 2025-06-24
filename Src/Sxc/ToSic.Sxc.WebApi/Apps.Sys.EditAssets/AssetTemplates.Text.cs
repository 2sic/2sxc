namespace ToSic.Sxc.Apps.Internal.Assets;

public partial class AssetTemplates
{
    private static readonly TemplateInfo EmptyTextFile = new("txt", "text file", ".txt", "Notes", ForDocs, TypeNone)
    {
        Body = @"Simple text file.
",
        Description = "Simple text file",
    };

    private static readonly TemplateInfo EmptyFile = new("empty", "Empty file", "", "some-file.txt", ForDocs, TypeNone)
    {
        Body = "",
        Description = "Simple empty file",
        PlatformTypes = null,
    };
}