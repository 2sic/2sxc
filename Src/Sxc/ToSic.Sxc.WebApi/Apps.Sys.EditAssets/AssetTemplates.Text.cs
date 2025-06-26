namespace ToSic.Sxc.Apps.Sys.EditAssets;

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