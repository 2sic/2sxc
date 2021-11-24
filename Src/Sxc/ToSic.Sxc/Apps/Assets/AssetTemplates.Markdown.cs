namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        private static readonly TemplateInfo Markdown = new TemplateInfo("markdown-readme", "Markdown Readme", ".md", ForDocs, "readme")
        {
            Body = @"# Readme

A standard README file.
",
            Description = "Markdown text file",
        };
    }
}
