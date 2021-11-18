namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        private static readonly TemplateInfo EmptyTextFile = new TemplateInfo("txt", "text file", ".txt", ForDocs)
        {
            Body = @"Simple text file.
",
            Description = "simple textual file",
        };
    }
}
