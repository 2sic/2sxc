using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ToSic.Sxc.Dnn.WebApi.HttpJson
{
    /// <summary>
    /// Utility methods for dealing with System.Text.Json.
    /// Based on https://github.com/RicoSuter/NJsonSchema/blob/master/src/NJsonSchema/Generation/SystemTextJsonUtilities.cs
    /// </summary>
    public static class SystemTextJsonUtilities
    {
        /// <summary>
        /// Convert System.Text.Json serializer options to Newtonsoft JSON settings.
        /// </summary>
        /// <param name="serializerOptions">The options.</param>
        /// <returns>The settings.</returns>
        public static JsonSerializerSettings ConvertJsonOptionsToNewtonsoftSettings(dynamic serializerOptions)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SystemTextJsonContractResolver(serializerOptions),
                
                //  2sxc specific settings
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                MaxDepth = 0
        };

            var jsonStringEnumConverter = ((IEnumerable)serializerOptions.Converters).OfType<object>()
                .FirstOrDefault(c => c is JsonStringEnumConverter);

            if (jsonStringEnumConverter == null)
                return settings;

            settings.Converters.Add(new StringEnumConverter { CamelCaseText = IsCamelCaseEnumNamingPolicy(jsonStringEnumConverter)});

            return settings;
        }

        private static bool IsCamelCaseEnumNamingPolicy(object jsonStringEnumConverter)
        {
            try
            {
                var enumNamingPolicy = jsonStringEnumConverter
                    .GetType().GetRuntimeFields()
                    .FirstOrDefault(x => x.FieldType.FullName == "System.Text.Json.JsonNamingPolicy")
                    ?.GetValue(jsonStringEnumConverter);

                return enumNamingPolicy != null &&
                    enumNamingPolicy.GetType().FullName == "System.Text.Json.JsonCamelCaseNamingPolicy";
            }
            catch
            {
                return false;
            }
        }
    }
}





