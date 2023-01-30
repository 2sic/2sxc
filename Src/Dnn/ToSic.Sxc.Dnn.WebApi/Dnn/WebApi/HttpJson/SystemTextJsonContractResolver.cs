using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Reflection;

namespace ToSic.Sxc.Dnn.WebApi.HttpJson
{
    /// <summary>
    /// Based on https://github.com/RicoSuter/NJsonSchema/blob/master/src/NJsonSchema/Generation/SystemTextJsonUtilities.cs
    /// </summary>
    public class SystemTextJsonContractResolver : DefaultContractResolver
    {
        private readonly dynamic _serializerOptions;

        public SystemTextJsonContractResolver(dynamic serializerOptions)
        {
            _serializerOptions = serializerOptions;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var propertyIgnored = false;
            var jsonIgnoreAttribute = member.GetCustomAttributes<System.Text.Json.Serialization.JsonIgnoreAttribute>(true).FirstOrDefault();
            if (jsonIgnoreAttribute != null) propertyIgnored = true;

            property.Ignored = propertyIgnored /*|| member.GetCustomAttributes<System.Text.Json.Serialization.JsonExtensionDataAttribute>(true).FirstOrDefault() != null*/;

            if (_serializerOptions.PropertyNamingPolicy != null)
                property.PropertyName = _serializerOptions.PropertyNamingPolicy.ConvertName(member.Name);

            dynamic jsonPropertyNameAttribute = member
                .GetCustomAttributes<System.Text.Json.Serialization.JsonPropertyNameAttribute>(true).FirstOrDefault();
            if (jsonPropertyNameAttribute != null && !string.IsNullOrEmpty(jsonPropertyNameAttribute.Name))
                property.PropertyName = jsonPropertyNameAttribute.Name;

            return property;
        }
    }
}
