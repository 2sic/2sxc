namespace ToSic.Sxc.Images
{
    /// <summary>
    /// The final sizes to be used when resizing
    /// </summary>
    public class OneResize
    {
        public int Width;
        public int Height;
        public string Url;
        public string Suffix;

        public string UrlWithSuffix => Url + Suffix;

        public IMultiResizeRule TagEnhancements;
    }
}
