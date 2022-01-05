using System.Collections.Specialized;

namespace ToSic.Sxc.Web.Images
{
    internal class ResizeParams
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int? Quality { get; set; }

        public string Mode { get; set; }
        public string Scale { get; set; }

        public string Format { get; set; }

        public NameValueCollection Parameters { get; set; }


        public ResizeParams() {}

        public ResizeParams(ResizeParams original)
        {
            Width=original.Width;
            Height=original.Height;
            Quality=original.Quality;
            Mode=original.Mode;
            Scale=original.Scale;
            Format=original.Format;
            Parameters=original.Parameters;
        }
    }
}
