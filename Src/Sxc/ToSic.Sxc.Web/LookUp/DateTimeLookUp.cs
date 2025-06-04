using ToSic.Lib.LookUp.Sources;

namespace ToSic.Sxc.LookUp;

internal class DateTimeLookUp() : LookUpBase("DateTime", "LookUp in Date-Time")
{
    public override string Get(string key, string format)
        => key.ToLowerInvariant() switch
        {
            "now" => DateTime.Now.ToString(format),
            "system" => DateTime.Now.ToString(format),
            "utc" => DateTime.Now.ToUniversalTime().ToString(format),
            _ => string.Empty
        };
}