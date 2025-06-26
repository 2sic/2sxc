using ToSic.Sxc.Apps.Sys.EditAssets;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

public class TemplatesDto
{
    public required string Default { get; set; }
    public required List<TemplateInfo> Templates;
}