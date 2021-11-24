using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        private static readonly TemplateInfo EmptyTextFile = new TemplateInfo("txt", "text file", ".txt", ForDocs, "Notes")
        {
            Body = @"Simple text file.
",
            Description = "Simple text file",
        };

        private static readonly TemplateInfo EmptyFile = new TemplateInfo("empty", "Empty file", "", ForDocs, "some-file.txt")
        {
            Body = "",
            Description = "Simple empty file",
            PlatformTypes = null,
        };
    }
}
