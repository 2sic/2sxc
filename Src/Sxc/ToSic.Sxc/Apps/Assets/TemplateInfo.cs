using System.Collections.Generic;
using Newtonsoft.Json;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps.Assets
{
    public class TemplateInfo
    {
        public TemplateInfo(string key, string name, string extension, string purpose)
        {
            Key = key;
            Name = name;
            Extension = extension;
            Purpose = purpose;
        }


        public string Key { get; }

        public string Name { get; }

        public string Extension { get; set; }

        public string Purpose { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Suffix { get; set; }

        /// <summary>
        /// Returns an array of platforms this template supports so the UI can pick
        /// </summary>
        public IEnumerable<string> Platforms => PlatformTypes.ToString().Split(',');

        [JsonIgnore]
        public PlatformType PlatformTypes { get; set; } = PlatformType.Hybrid | PlatformType.Dnn | PlatformType.Oqtane;
    }
}
