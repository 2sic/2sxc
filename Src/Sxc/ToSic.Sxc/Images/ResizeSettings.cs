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

        public string Mode { get; set; }

        public string Scale { get; set; }

        public string Format { get; set; }

        public string SrcSet { get; set; }

        public NameValueCollection Parameters { get; set; }

        public ResizeSettings() {}

        public ResizeSettings(IResizeSettings original, bool keepSourceSet)
        {
            Width=original.Width;
            Height=original.Height;
            Quality=original.Quality;
            Mode=original.Mode;
            Scale=original.Scale;
            Format=original.Format;
            Parameters=original.Parameters;
            if (keepSourceSet)
                SrcSet = original.SrcSet;
        }
    }
}
