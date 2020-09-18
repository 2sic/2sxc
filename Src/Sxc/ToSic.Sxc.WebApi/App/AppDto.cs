namespace ToSic.Sxc.WebApi.App
{
    public class AppDto
    {
        public int Id { get; set; }
        public bool IsApp { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public string AppRoot { get; set; }
        public bool IsHidden { get; set; }
        public int? ConfigurationId { get; set; }
        public int Items { get; set; }
        public string Thumbnail { get; set; }
        public string Version { get; set; }
    }
}
