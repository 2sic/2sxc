using ToSic.Eav.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Net.Http.Formatting
{
    internal class JsonFormatters
    {
        public static SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatter =>
            _systemTextJsonMediaTypeFormatter ??= new SystemTextJsonMediaTypeFormatter
            {
                JsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtml
            };
        private static SystemTextJsonMediaTypeFormatter _systemTextJsonMediaTypeFormatter;
    }
}
