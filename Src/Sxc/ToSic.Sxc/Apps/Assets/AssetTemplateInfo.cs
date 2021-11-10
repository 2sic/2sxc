namespace ToSic.Sxc.Apps.Assets
{
    public class AssetTemplateInfo
    {
        public AssetTemplateType Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Extension { get; set; }
        public string Purpose { get; set; }
    }
}
