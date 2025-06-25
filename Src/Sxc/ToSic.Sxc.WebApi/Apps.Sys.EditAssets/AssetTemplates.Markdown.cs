namespace ToSic.Sxc.Apps.Sys.EditAssets;

public partial class AssetTemplates
{
    private static readonly TemplateInfo Markdown = new("markdown-readme", "Markdown Readme", ".md", "readme", ForDocs, TypeNone)
    {
        Body = @"# Readme

A standard README file.
",
        Description = "Markdown text file",
    };
}