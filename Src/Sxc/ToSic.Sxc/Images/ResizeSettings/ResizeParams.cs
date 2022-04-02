using System.Collections.Specialized;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Images.ImageConstants;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// Helper to process optional parameters and figure out if they should be used or not
    /// </summary>
    internal class ResizeParams
    {
        public double? AspectRationOrNull(object aspectRatio) 
            => DoubleOrNullWithCalculation(aspectRatio);


        public double? FactorOrNull(object factor) 
            => DoubleOrNullWithCalculation(factor);

        public string FormatOrNull(object format)
            => FindKnownFormatOrNull(RealStringOrNull(format));

        public int? QualityOrNull(object quality)
        {
            var qParamDouble = DoubleOrNull(quality);
            if (qParamDouble.HasValue)
                qParamDouble = DNearZero(qParamDouble.Value)  // ignore if basically 0
                    ? null
                    : qParamDouble.Value > 1
                        ? qParamDouble
                        : qParamDouble * 100;
            var qParamInt = (int?)qParamDouble;
            return qParamInt;
        }

        public NameValueCollection ParametersOrNull(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters)) return null;
            return UrlHelpers.ParseQueryString(parameters);
        }

        public string ScaleModeOrNull(string scaleMode) => FindKnownScaleOrNull(scaleMode);

        public int? WidthOrNull(object width) => IntOrNull(width);
        public int? HeightOrNull(object height) => IntOrNull(height);
    }
}
