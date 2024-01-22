using System.Globalization;

namespace ToSic.Sxc.Internal.Plumbing;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static partial class ParseObject
{

    internal static double? DoubleOrNull(object value)
    {
        if (value is null) return null;
        if (value is float floatVal) return floatVal;
        if (value is double dVal) return dVal;

        var strValue = RealStringOrNull(value);
        if (strValue == null) return null;
        strValue = strValue.Replace(",", ".");
        if (!double.TryParse(strValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var doubleValue)) return null;
        return doubleValue;
    }

    internal static double? DoubleOrNullWithCalculation(object value)
    {
        // First, check if it's a string like "4:2" or "16/9"
        if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
        {
            // check for separator
            var separator = strValue.IndexOfAny([':', '/']);
            if (separator > 0) // if it starts with the separator, something is wrong
            {
                var leftPart = strValue.Substring(0, separator);
                var rightPart = strValue.Substring(separator + 1);

                if (double.TryParse(leftPart, out var top) && double.TryParse(rightPart, out var bottom) &&
                    top > 0 && bottom > 0)
                {
                    var result = top / bottom;
                    return result;
                }
            }
        }

        return DoubleOrNull(value);
    }

    /// <summary>
    /// Special helper to verify a double is near zero by at least 1%.
    /// To test if it's near another number, subtract that first and then check if near zero. 
    /// </summary>
    internal static bool DNearZero(double d) => Math.Abs(d) <= 0.01;
}