using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps.Assets
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class TemplateInfo
    {
        public TemplateInfo(string key, string name, string extension, string suggestedFileName, string purpose, string type)
        {
            Key = key;
            Name = name;
            Extension = extension;
            Purpose = purpose;
            Type = type;
            SuggestedFileName = suggestedFileName;
        }


        public string Key { get; }

        public string Name { get; }

        public string SuggestedFileName { get; }

        public string Extension { get; set; }

        public string Purpose { get; set; }

        public string Type { get; set; }

        public string Folder { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Body { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Prefix { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Suffix { get; set; }

        /// <summary>
        /// Returns an array of platforms this template supports so the UI can pick
        /// </summary>
        public IEnumerable<string> Platforms => PlatformTypes?.ToString().Split(',').Select(p => p.Trim());

        [JsonIgnore]
        public PlatformType? PlatformTypes { get; set; } = PlatformType.Hybrid | PlatformType.Dnn | PlatformType.Oqtane;
    }
}
