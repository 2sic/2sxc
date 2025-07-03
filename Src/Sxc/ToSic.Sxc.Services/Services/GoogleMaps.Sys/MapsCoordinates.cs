namespace ToSic.Sxc.Services.GoogleMaps.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class MapsCoordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>
    /// The default coordinates have a trailing 1.
    /// This way we can detect if it's using the default or the configured values
    /// </summary>
    internal static MapsCoordinates Defaults = new()
    {
        Latitude = 47.17471,
        Longitude = 9.46921,
    };
}