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
        public double Factor { get; set; } = 1;


        public string SrcSet { get; set; }
        public NameValueCollection Parameters { get; set; }

        // New properties
        public double AspectRatio { get; set; }
        public FactorMap[] FactorMap { get; set; }
        public bool UseFactorMap { get; set; } = true;
        public bool UseAspectRatio { get; set; } = true;

        public ResizeSettings() {}

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
            if (keepSourceSet)
                SrcSet = original.SrcSet;
            
        }

        //internal ResizeSettings ApplyFactor(double factor)
        //{
        //    Factor = factor;
        //    return ApplyFactor();
        //}

        //internal ResizeSettings ApplyFactor()
        //{
        //    Width = (UseFactorMap ? FactorMapHelper.Find(FactorMap, Factor)?.Width : null) ?? (int)(Factor * Width);
        //    Height = (int)(Factor * Height);
        //    Factor = 1;
        //    return this;
        //}
    }
}
