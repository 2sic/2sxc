namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        public static TemplateInfo Token =
            new TemplateInfo("html-token", "HTML Token Template", ".html", "DetailsTemplate", ForTemplate, TypeToken)
            {
                Body = @"
<p>
    You successfully created your own template.
    Start editing it by hovering the ""Manage"" button and opening the ""Edit Template"" dialog.
</p>",
                Description = "html token template",
            };

    }
}
