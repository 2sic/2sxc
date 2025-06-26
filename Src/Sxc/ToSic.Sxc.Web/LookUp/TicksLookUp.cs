using ToSic.Eav.LookUp.Sources;

#if NETCOREAPP
namespace ToSic.Sxc.LookUp;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class TicksLookUp() : LookUpBase("Ticks", "LookUp ticks of now, today, TicksPerDay")
{
    public override string Get(string key, string format)
        => key.ToLowerInvariant() switch
        {
            "now" => DateTime.Now.Ticks.ToString(format),
            "today" => DateTime.Today.Ticks.ToString(format),
            "ticksperday" => TimeSpan.TicksPerDay.ToString(format),
            _ => string.Empty
        };
}

#endif