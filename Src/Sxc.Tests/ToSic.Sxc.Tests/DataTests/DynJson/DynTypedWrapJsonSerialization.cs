using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Tests.DataTests.DynWrappers;
using static System.Text.Json.JsonSerializer;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class DynTypedWrapJsonSerialization: DynAndTypedTestsBase
    {
        [TestMethod]
        public void JsonSerialization()
        {
            var data = DynFromObjectBasic.Data;
            var anon = new
            {
                data.Name,
                data.Description,
                data.Founded,
                // Birthday not used in this test, because the initial default serializer
                // will not use "Z" time, so it will be a bit different.
                //data.Birthday, 
                data.Truthy
            };
            var jsonAnon = Serialize(anon);


            //var typed = TypedFromObject(anon);
            var dynAnon = Json2Obj2Dyn(anon);

            //var jsonTyped = Serialize(typed);
            
            var jsonDyn = Serialize(dynAnon);

            //AreEqual(jsonTyped, jsonDyn);
            AreEqual(jsonAnon, jsonDyn);
        }
    }
}
