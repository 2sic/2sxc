using ToSic.Sxc.Apps.Internal.Assets;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

public class TemplatesDto
{
    public string Default { get; set; }
    public List<TemplateInfo> Templates;
}