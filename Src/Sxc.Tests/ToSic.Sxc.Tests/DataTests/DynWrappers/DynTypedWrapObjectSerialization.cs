using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Text.Json.JsonSerializer;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class DynTypedWrapObjectSerialization: DynAndTypedTestsBase
    {
        [TestMethod]
        public void JsonSerialization()
        {
            var data = DynFromObjectBasic.Data;
            var anon = new { data.Name, data.Description, data.Founded, data.Birthday, data.Truthy };
            var typed = Obj2Typed(anon);
            dynamic dynAnon = WrapObjFromObject(anon, false, false);

            var jsonTyped = Serialize(typed);
            var jsonAnon = Serialize(anon);
            var jsonDyn = Serialize(dynAnon);

            AreEqual(jsonTyped, jsonDyn);
            AreEqual(jsonTyped, jsonAnon);
        }
    }
}
