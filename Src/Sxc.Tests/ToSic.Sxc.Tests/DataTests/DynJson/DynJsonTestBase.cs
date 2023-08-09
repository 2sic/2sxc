using ToSic.Eav.Serialization;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    public abstract class DynJsonTestBase: TestBaseSxcDb
    {
        protected DynJsonTestBase()
        {
            Factory = GetService<CodeDataWrapper>();
        }
        protected CodeDataWrapper Factory;

        public dynamic AsDynamic(string jsonString) => Factory.FromJson(jsonString);

        public dynamic AsDynamic(object obj) => AsDynamic(AsJson(obj));

        public string AsJson(object obj) => System.Text.Json.JsonSerializer.Serialize(obj, JsonOptions.UnsafeJsonWithoutEncodingHtml);

        public (dynamic Dyn, string Json, T Original) AnonToJsonToDyn<T>(T original)
        {
            var json = AsJson(original);
            return (AsDynamic(json), json, original);
        }
    }
}
