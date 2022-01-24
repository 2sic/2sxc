using System.Collections.Generic;
using Newtonsoft.Json;

namespace ToSic.Sxc.WebApi.Assets
{
    public class AllFilesDto
    {
        public IEnumerable<AllFileDto> Files = new List<AllFileDto>();
    }

    public class AllFileDto
    {
        public string Path;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Shared;
    }
}
