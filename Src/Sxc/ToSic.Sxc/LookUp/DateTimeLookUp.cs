#if NETCOREAPP
using System;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.LookUp
{
    public class DateTimeLookUp : LookUpBase
    {
        public DateTimeLookUp()
        {
            Name = "DateTime";
        }

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
}
#endif