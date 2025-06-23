using ToSic.Sxc.Apps.Internal.Assets;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

public class TemplatesDto
{
    public required string Default { get; set; }
    public required List<TemplateInfo> Templates;
}