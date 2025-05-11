using System.Collections.Specialized;
using ToSic.Lib.Services;
using ToSic.Sxc.Web.Internal.Url;
using static ToSic.Sxc.Images.Internal.ImageConstants;
using static ToSic.Sxc.Internal.Plumbing.ParseObject;

namespace ToSic.Sxc.Images;

/// <summary>
/// Helper to process optional parameters and figure out if they should be used or not
/// </summary>
internal class ResizeParams(ILog parentLog) : HelperBase(parentLog, $"{SxcLogName}.ResPar")
{
    internal static double? AspectRatioOrNull(object aspectRatio) 
        => DoubleOrNullWithCalculation(aspectRatio);


    internal static double? FactorOrNull(object factor) 
        => DoubleOrNullWithCalculation(factor);

    public static string FormatOrNull(object format)
        => FindKnownFormatOrNull(RealStringOrNull(format));

    internal static int? QualityOrNull(object quality)
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

    internal static NameValueCollection ParametersOrNull(string parameters)
        => string.IsNullOrWhiteSpace(parameters)
            ? null
            : UrlHelpers.ParseQueryString(parameters);


    public static string ResizeModeOrNull(string resizeMode) => resizeMode; // this one doesn't do any conversion atm

    public static string ScaleModeOrNull(string scaleMode) => FindKnownScaleOrNull(scaleMode);

    internal static int? WidthOrNull(object width) => IntOrNull(width);
    internal static int? HeightOrNull(object height) => IntOrNull(height);

}