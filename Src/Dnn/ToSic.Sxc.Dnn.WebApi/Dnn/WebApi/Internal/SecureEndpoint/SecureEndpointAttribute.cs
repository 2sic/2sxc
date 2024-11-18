using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using ToSic.Eav.Security.Encryption;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Dnn.WebApi.Internal.SecureEndpoint
{
    public class SecureEndpointAttribute(string mediaType = "application/json") : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var request = filterContext.Request;

            if (request.Method == HttpMethod.Post && request.Content.Headers.ContentType.MediaType == mediaType)
            {
                var encryptedDataJson = ReadEncryptedData(request);
                if (string.IsNullOrEmpty(encryptedDataJson))
                {
                    filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request content is empty.");
                    return;
                }

                var encryptedData = JsonSerializer.Deserialize<EncryptedData>(encryptedDataJson, options: JsonOptions.SafeJsonForHtmlAttributes);
                // "duck typing" check
                if (encryptedData.Version == 1 && encryptedData.Data is null && encryptedData.Key is null && encryptedData.Iv is null) // 
                    return;

                // Determine the parameter name and type dynamically.
                // Find parameter that has FromBody attribute on it,
                // or param that is not a value type.
                var parameter = filterContext.ActionDescriptor.GetParameters()
                    .FirstOrDefault(p => p.GetCustomAttributes<FromBodyAttribute>().Any() || !p.ParameterType.IsValueType);
                if (parameter == null)
                {
                    filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "No parameter found for action.");
                    return;
                }

                var decryptedData = Decrypt(filterContext.Request.GetDependencyScope(), encryptedData);

                try
                {
                    // Validate that decrypted data is valid JSON
                    var formData = JsonSerializer.Deserialize(decryptedData, parameter.ParameterType, options: JsonOptions.SafeJsonForHtmlAttributes);

                    // Replace the request content with the deserialized object
                    filterContext.ActionArguments[parameter.ParameterName] = formData;
                    request.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(decryptedData)));
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //filterContext.Request.Content = new ObjectContent(parameter.ParameterType, formData, parameter.Configuration.Formatters.JsonFormatter);
                }
                catch (JsonException)
                {
                    // Handle invalid JSON format
                    filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid JSON format after decryption.");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private static string ReadEncryptedData(HttpRequestMessage request)
        {
            var stream = request.Content.ReadAsStreamAsync().Result;
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static string Decrypt(IDependencyScope dependencyScope, EncryptedData encryptedData)
        {
            var cryptoService = (AesHybridCryptographyService)dependencyScope.GetService(typeof(AesHybridCryptographyService));
            return cryptoService.Decrypt(encryptedData);
        }
    }
}
