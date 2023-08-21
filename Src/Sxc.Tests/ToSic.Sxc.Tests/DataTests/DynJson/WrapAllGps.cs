using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class WrapAllGps : DynAndTypedTestsBase
    {
        public static double GpsDataLat = 43.508075;
        public static double GpsDataLong = 16.4665157;
        public static object GpsDataAnon = new { Lat = GpsDataLat, Long = GpsDataLong };
        public dynamic GpsDataDyn => Obj2Json2Dyn(GpsDataAnon);
        public ITyped GpsDataJsonTyped => Obj2Json2TypedStrict(GpsDataAnon);
        public ITyped GpsDataObjTyped => Obj2Typed(GpsDataAnon);

        [TestMethod]
        public void Gps_Dyn()
        {
            AreEqual<double>(GpsDataLat, GpsDataDyn.Lat);
            AreEqual<double>(GpsDataLong, GpsDataDyn.Long);
        }

        [TestMethod] public void Gps_TypedJson() => Gps_Typed(GpsDataJsonTyped);
        [TestMethod] public void Gps_TypedFromObject() => Gps_Typed(GpsDataObjTyped);

        public void Gps_Typed(ITyped typed)
        {
            AreEqual<double>(GpsDataLat, typed.Double("Lat"));
            AreEqual<double>(GpsDataLong, typed.Double("Long"));
        }
    }
}