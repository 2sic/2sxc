using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using ToSic.Eav.Security.Encryption;

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

                var encryptedData = JsonConvert.DeserializeObject<EncryptedData>(encryptedDataJson);
                if (encryptedData?.Data == null)
                {
                    filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid encrypted data.");
                    return;
                }

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
                    var formData = JsonConvert.DeserializeObject(decryptedData, parameter.ParameterType);

                    // Replace the request content with the deserialized object
                    filterContext.ActionArguments[parameter.ParameterName] = formData;
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
