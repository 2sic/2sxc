namespace ToSic.Sxc.Web
{
    public class ClientAssetInfo
    {
        /// <summary>
        /// Asset ID for use in HTML - ideally should ensure that this asset is only loaded once
        /// </summary>
        public string Id;
        
        public bool IsJs = true;
        public string Url;
        public int Priority;
        public string PosInPage = "body";
        public bool AutoOpt = false;

        public bool IsExternal = true;
        public string Content = null;
    }
}
