namespace ToSic.Sxc.Tests.Data.DynamicJacket
{
    public abstract class DynamicJacketTestBase
    {
        public dynamic AsDynamic(string jsonString) => Sxc.Data.DynamicJacket.AsDynamicJacket(jsonString);

        public dynamic AsDynamic(object obj) => AsDynamic(AsJson(obj));

        public string AsJson(object obj) => System.Text.Json.JsonSerializer.Serialize(obj);

        public (dynamic Dyn, string Json, T Original) PrepareTest<T>(T original)
        {
            var json = AsJson(original);
            return (AsDynamic(json), json, original);
        }
    }
}
