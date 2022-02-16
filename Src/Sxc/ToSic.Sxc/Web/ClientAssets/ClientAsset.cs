namespace ToSic.Sxc.Web
{
    public class ClientAsset : IClientAsset
    {
        /// <summary>
        /// Asset ID for use in HTML - ideally should ensure that this asset is only loaded once
        /// </summary>
        public string Id { get; set; }
        
        public bool IsJs { get; set; }= true;
        public string Url { get; set; }
        public int Priority {get; set; }
        public string PosInPage { get; set; } = "body";
        public bool AutoOpt { get; set; } = false;

        public bool IsExternal { get; set; } = true;
        public string Content { get; set; } = null;
    }
}
