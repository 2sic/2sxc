using ToSic.Eav.Serialization;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.Data.DynamicJacket
{
    public abstract class DynamicJacketTestBase: TestBaseSxcDb
    {
        protected DynamicJacketTestBase()
        {
            Factory = GetService<DynamicWrapperFactory>();
        }
        protected DynamicWrapperFactory Factory;

        public dynamic AsDynamic(string jsonString) => Factory.AsDynamicJacket(jsonString);

        public dynamic AsDynamic(object obj) => AsDynamic(AsJson(obj));

        public string AsJson(object obj) => System.Text.Json.JsonSerializer.Serialize(obj, JsonOptions.UnsafeJsonWithoutEncodingHtml);

        public (dynamic Dyn, string Json, T Original) PrepareTest<T>(T original)
        {
            var json = AsJson(original);
            return (AsDynamic(json), json, original);
        }
    }
}
