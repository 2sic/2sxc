using Newtonsoft.Json;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Services
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

        /// <inheritdoc />
        public string ToJsonIndented(object dynamicEntity) => JsonConvert.SerializeObject(dynamicEntity, Formatting.Indented);
    }
}
