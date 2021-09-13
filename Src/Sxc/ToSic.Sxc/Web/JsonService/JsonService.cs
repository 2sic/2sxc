using Newtonsoft.Json;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    [PrivateApi("Hide implementation")]
    public class JsonService: IJsonService
    {
        /// <inheritdoc />
        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);

        /// <inheritdoc />
        public object Deserialize(string json) => JsonConvert.DeserializeObject(json);

        /// <inheritdoc />
        public string Serialize(object dynamicEntity) => JsonConvert.SerializeObject(dynamicEntity);
    }
}
