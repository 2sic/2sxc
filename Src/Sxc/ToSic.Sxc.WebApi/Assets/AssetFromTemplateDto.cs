namespace ToSic.Sxc.WebApi.Assets
{
    /// <summary>
    /// helper class, because it's really hard to get a post-body in a web-api call if it's not in a json-object format
    /// </summary>
    public class AssetFromTemplateDto
    {
        public int AppId;

        public string Path;

        public bool Global;

        public string TemplateKey;
    }

}
