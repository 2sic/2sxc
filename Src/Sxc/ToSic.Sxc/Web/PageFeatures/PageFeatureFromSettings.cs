namespace ToSic.Sxc.Web.PageFeatures
{
    public class PageFeatureFromSettings: PageFeature
    {
        public PageFeatureFromSettings(string key, string name, string description = null, string[] needs = null, string html = null) : base(key, name, description, needs, html)
        {
        }

        public bool AlreadyProcessed { get; set; }

    }
}
