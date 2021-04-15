// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.ApiExplorer
{
    public class ApiActionDto
    {
        public string name { get; set; }
        public IEnumerable<string> verbs { get; set; }
        public IEnumerable<ApiActionParamDto> parameters { get; set; }
        public ApiSecurityDto security { get; set; }
        public ApiSecurityDto mergedSecurity { get; set; }
        public string returns { get; set; }
    }
}
