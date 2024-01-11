namespace ToSic.Sxc.Internal.Plumbing;

internal static partial class ParseObject
{
    /// <summary>
    /// Check if an object
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static int? IntOrNull(object value)
    {
        if (value is null) return null;
        if (value is int intVal) return intVal;

        var floatVal = DoubleOrNull(value);
        if (floatVal is null) return null;

        var rounded = (int)Math.Round(floatVal.Value);
        if (rounded < 1) return 0;
        return rounded;
    }


    internal static int? IntOrZeroAsNull(object value)
    {
        var val = IntOrNull(value);
        if (val == 0) return null;
        return val;
    }


}