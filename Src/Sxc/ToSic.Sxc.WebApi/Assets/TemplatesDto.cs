using System.Collections.Generic;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.WebApi.Assets
{
    public class TemplatesDto
    {
        public string Default { get; set; }
        public List<TemplateInfo> Templates;
    }
}
