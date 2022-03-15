using System.Collections.Specialized;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Hide implementation")]
    public class ResizeSettings : IResizeSettings
    {
        public int Width { get; }
        public int Height { get; }
        public int Quality { get; set; }
        public string ResizeMode { get; set; }
        public string ScaleMode { get; set; }
        public string Format { get; }
        public double Factor { get; } = 1;


        public string SrcSet { get; }
        public NameValueCollection Parameters { get; set; }

        // New properties
        public double AspectRatio { get; }
        public FactorMap[] FactorMap { get; set; }
        public bool UseFactorMap { get; set; } = true;
        public bool UseAspectRatio { get; set; } = true;

        /// <summary>
        /// Constructor to create new
        /// </summary>
        public ResizeSettings(int width, int height, double aspectRatio, double factor, string format, string srcSet)
        {
            Width = width;
            Height = height;
            AspectRatio = aspectRatio;
            Factor = factor;
            Format = format;
            SrcSet = srcSet;
        }

        /// <summary>
        /// Constructor to copy
        /// </summary>
        public ResizeSettings(IResizeSettings original, bool keepSourceSet = true)
        {
            Width = original.Width;
            Height = original.Height;
            Quality = original.Quality;
            ResizeMode = original.ResizeMode;
            ScaleMode = original.ScaleMode;
            Format = original.Format;
            Factor = original.Factor;
            Parameters = original.Parameters;
            FactorMap = original.FactorMap;
            AspectRatio = original.AspectRatio;
            UseAspectRatio = original.UseAspectRatio;
            UseFactorMap = original.UseFactorMap;
            if (keepSourceSet)
                SrcSet = original.SrcSet;
        }

        public ResizeSettings(IResizeSettings original, string format, string srcSet): this(original)
        {
            Format = format ?? Format;
            SrcSet = srcSet ?? SrcSet;
        }

        public ResizeSettings(IResizeSettings original, double factor): this(original)
        {
            Factor = factor;
        }
    }
}
