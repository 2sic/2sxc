namespace ToSic.Sxc.Apps.Assets
{
    public class TemplateInfo
    {
        public TemplateInfo(
            string key,
            string name ,
            string extension,
            string purpose,
            string description = "")
        {
            Key = key;
            Name = name;
            Extension = extension;
            Purpose = purpose;
            Description = description;
            Body = string.Empty;
        }

        public string Key { get; }
        public string Name { get; set; }
        public string Extension { get; }
        public string Description { get; }
        public string Body { get; set;  }
        // will be removed
        internal string Purpose { get; }
    }
}
