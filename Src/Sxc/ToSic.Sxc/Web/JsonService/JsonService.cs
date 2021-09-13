using Newtonsoft.Json;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    [PrivateApi("Hide implementation")]
    public class JsonService: IJsonService
    {
        /// <inheritdoc />
        public T To<T>(string json) => JsonConvert.DeserializeObject<T>(json);

        /// <inheritdoc />
        public object ToObject(string json) => JsonConvert.DeserializeObject(json);

        /// <inheritdoc />
        public string ToJson(object dynamicEntity) => JsonConvert.SerializeObject(dynamicEntity);
    }
}
