using System.Collections.Generic;
using ToSic.Sxc.Apps.Internal.Assets;

namespace ToSic.Sxc.WebApi.Admin.AppFiles;

public class TemplatesDto
{
    public string Default { get; set; }
    public List<TemplateInfo> Templates;
}