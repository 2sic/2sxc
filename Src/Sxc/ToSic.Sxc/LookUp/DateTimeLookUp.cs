using ToSic.Eav.LookUp;

namespace ToSic.Sxc.LookUp;

internal class DateTimeLookUp() : LookUpBase("DateTime", "LookUp in Date-Time")
{
    public override string Get(string key, string format)
    {
        return key.ToLowerInvariant() switch
        {
            "now" => DateTime.Now.ToString(format),
            "system" => DateTime.Now.ToString(format),
            "utc" => DateTime.Now.ToUniversalTime().ToString(format),
            _ => string.Empty
        };
    }
}