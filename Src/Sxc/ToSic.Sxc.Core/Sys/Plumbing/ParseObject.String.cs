namespace ToSic.Sxc.Internal.Plumbing;

internal static partial class ParseObject
{
    internal static string RealStringOrNull(object value)
    {
        if (value == null) return null;
        if (value is string strValue) return strValue;
        if (!value.GetType().IsValueType) return null;

        // Only do this for value types
        strValue = value.ToString();
        return string.IsNullOrEmpty(strValue) ? null : strValue;
    }


    internal static string KeepBestString(object given, object setting)
    {
        if (given == null && setting == null) return null;
        var strGiven = RealStringOrNull(given);
        if (strGiven != null) return strGiven;
        var strSetting = RealStringOrNull(setting);
        return strSetting;
    }

}