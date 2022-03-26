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

        public Recipe TagEnhancements;

        /// <summary>
        /// Will be set based on image metadata, to determine that the image should be shown completely (like a logo) and not cropped.
        /// This means the image could be a different size than expected
        /// </summary>
        public bool ShowAll { get; set; } = false;
    }
}
