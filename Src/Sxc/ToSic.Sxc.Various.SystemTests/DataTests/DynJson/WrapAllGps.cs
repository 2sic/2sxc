using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynJson;


public class WrapAllGps(DynAndTypedTestHelper helper)
{
    public static double GpsDataLat = 43.508075;
    public static double GpsDataLong = 16.4665157;
    public static object GpsDataAnon = new { Lat = GpsDataLat, Long = GpsDataLong };
    public dynamic GpsDataDyn => helper.Obj2Json2Dyn(GpsDataAnon);
    public ITyped GpsDataJsonTyped => helper.Obj2Json2TypedStrict(GpsDataAnon);
    public ITyped GpsDataObjTyped => helper.Obj2Typed(GpsDataAnon);

    [Fact]
    public void GpsDynProperties()
    {
        Equal<double>(GpsDataLat, GpsDataDyn.Lat);
        Equal<double>(GpsDataLong, GpsDataDyn.Long);
    }

    [Fact] public void Gps_TypedJson() => GpsTypedProperties(GpsDataJsonTyped);
    [Fact] public void Gps_TypedFromObject() => GpsTypedProperties(GpsDataObjTyped);

    public void GpsTypedProperties(ITyped typed)
    {
        Equal<double>(GpsDataLat, typed.Double("Lat"));
        Equal<double>(GpsDataLong, typed.Double("Long"));
    }
}