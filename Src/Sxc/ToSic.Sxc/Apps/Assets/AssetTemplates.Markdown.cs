namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        private static readonly TemplateInfo Markdown = new TemplateInfo("markdown-readme", "Markdown Readme", ".md", ForDocs)
        {
            Body = @"# Readme

A standard README file.
",
            Description = ".md file",
        };
    }
}
