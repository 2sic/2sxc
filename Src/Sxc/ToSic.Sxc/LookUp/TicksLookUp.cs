#if NETCOREAPP
using System;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.LookUp
{
    public class TicksLookUp : LookUpBase
    {
        public TicksLookUp()
        {
            Name = "Ticks";
        }

        public override string Get(string key, string format)
        {
            return key.ToLowerInvariant() switch
            {
                "now" => DateTime.Now.Ticks.ToString(format),
                "today" => DateTime.Today.Ticks.ToString(format),
                "ticksperday" => TimeSpan.TicksPerDay.ToString(format),
                _ => string.Empty
            };
        }
    }
}
#endif