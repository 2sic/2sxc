using System.Collections.Specialized;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Hide implementation")]
    public class ResizeSettings : IResizeSettings
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int Quality { get; set; }

        public string ResizeMode { get; set; }

        public string ScaleMode { get; set; }

        public string Format { get; set; }

        public string SrcSet { get; set; }

        public NameValueCollection Parameters { get; set; }

        public ResizeSettings() {}

        public ResizeSettings(IResizeSettings original, bool keepSourceSet)
        {
            Width=original.Width;
            Height=original.Height;
            Quality=original.Quality;
            ResizeMode=original.ResizeMode;
            ScaleMode=original.ScaleMode;
            Format=original.Format;
            Parameters=original.Parameters;
            if (keepSourceSet)
                SrcSet = original.SrcSet;
        }
    }
}
