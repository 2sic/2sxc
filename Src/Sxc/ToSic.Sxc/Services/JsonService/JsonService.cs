using System.IO;
using System.Text;
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
        public string ToJson(object item) => JsonConvert.SerializeObject(item);

        /// <inheritdoc />
        public string ToJson(object item, int indentation) => JsonConvert.SerializeObject(item, Formatting.Indented);
    }
}
