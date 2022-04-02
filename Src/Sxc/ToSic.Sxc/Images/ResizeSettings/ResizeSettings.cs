using System.Collections.Specialized;
using ToSic.Eav.Documentation;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

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
        public double AspectRatio { get; }
        public NameValueCollection Parameters { get; set; }


        public bool UseFactorMap { get; set; } = true;
        public bool UseAspectRatio { get; set; } = true;

        public AdvancedSettings Advanced { get; set; }

        /// <summary>
        /// Constructor to create new
        /// </summary>
        public ResizeSettings(int width, int height, double aspectRatio, double factor, string format)
        {
            Width = width;
            Height = height;
            AspectRatio = aspectRatio;
            Factor = factor;
            Format = format;
        }

        /// <summary>
        /// Constructor to copy
        /// </summary>
        public ResizeSettings(IResizeSettings original,
            string noParamOrder = Eav.Parameters.Protector,
            int? width = null,
            int? height = null,
            double? aspectRatio = null,
            double? factor = null,
            int? quality = null,
            string format = null,
            string resizeMode = null,
            string scaleMode = null,
            NameValueCollection parameters = null,
            AdvancedSettings advanced = null
        )
        {
            Width = width ?? original.Width;
            Height = height ?? original.Height;
            Quality = quality ?? original.Quality;
            ResizeMode = resizeMode ?? original.ResizeMode;
            ScaleMode = scaleMode ?? original.ScaleMode;
            Format = format ?? original.Format;
            Factor = factor ?? original.Factor;
            Parameters = parameters ?? original.Parameters;
            AspectRatio = aspectRatio ?? original.AspectRatio;
            UseAspectRatio = original.UseAspectRatio;
            UseFactorMap = original.UseFactorMap;
            Advanced = advanced ?? original.Advanced;
        }

        //public ResizeSettings(IResizeSettings original, string format): this(original)
        //{
        //    Format = format ?? Format;
        //}

        //public ResizeSettings(IResizeSettings original, double factor, AdvancedSettings advanced = null): this(original)
        //{
        //    Factor = factor;
        //    Advanced = advanced ?? Advanced;
        //}

    }
}
