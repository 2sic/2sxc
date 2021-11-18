namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        public static TemplateInfo Token =
            new TemplateInfo(TemplateKey.Token, "HTML Token Template", Extension.Html, ForTemplate)
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
