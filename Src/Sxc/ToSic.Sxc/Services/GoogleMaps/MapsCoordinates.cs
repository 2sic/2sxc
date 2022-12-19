using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Services.GoogleMaps
{
    [PrivateApi]
    public class MapsCoordinates
    {
        //public static double DefaultLatitude = 47.1747; // 47.17465989999999
        //public static double DefaultLongitude = 9.4692; // 9.469142499999975

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        internal static MapsCoordinates Default = new MapsCoordinates
        {
            Latitude = 47.17471, // 47.17465989999999
            Longitude = 9.46921, // 9.469142499999975
        };
    }

}
