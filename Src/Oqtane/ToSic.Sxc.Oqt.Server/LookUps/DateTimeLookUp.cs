using System;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Oqt.Server.LookUps
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