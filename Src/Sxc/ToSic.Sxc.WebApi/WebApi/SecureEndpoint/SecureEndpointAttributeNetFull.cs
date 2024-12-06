#if NETFRAMEWORK
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using ToSic.Eav.Security.Encryption;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.WebApi;

/// <summary>
/// An ActionFilter attribute that automatically decrypts encrypted POST payloads for Web API endpoints.
/// Apply this attribute to a controller or action method to seamlessly handle encrypted incoming POST requests,
/// ensuring the decrypted data is available for processing within the action method.
/// </summary>
/// <remarks>
/// This attribute intercepts POST requests with JSON content, checks for encrypted data,
/// and if present, decrypts the payload using the <see cref="AesHybridCryptographyService"/>.
/// It then deserializes the decrypted data into the expected parameter type and replaces the action arguments.
/// If the payload is not encrypted, the request content remains unchanged.
/// * Introduced in version 18.05.
/// </remarks>
[PublicApi]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SecureEndpointAttribute : ActionFilterAttribute
{
    private const string MediaType = "application/json";

    public override void OnActionExecuting(HttpActionContext filterContext)
    {
        var request = filterContext.Request;

        if (request.Method == HttpMethod.Post && request.Content.Headers.ContentType.MediaType == MediaType)
        {
            var jsonString = GetRequestContentStream(request);
            if (string.IsNullOrEmpty(jsonString))
            {
                filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request content is empty.");
                return;
            }

            // Deserializes the JSON string to an EncryptedData object and performs a "EncryptedData" check.
            // If encrypted data is missing, sets the request content stream to the original JSON string and returns.
            var encryptedData = JsonSerializer.Deserialize<EncryptedData>(jsonString, options: JsonOptions.SafeJsonForHtmlAttributes);
            if (encryptedData?.Data is null && encryptedData?.Key is null && encryptedData?.Iv is null && encryptedData?.Version == 1)
            {
                SetRequestContentStream(request, jsonString);
                return;
            }

            // Currently, only POST payloads can be encrypted. 
            // Try to determine the POST parameter dynamically by finding a parameter 
            // that has the FromBody attribute on it or a parameter that is not a value type.
            var parameter = filterContext.ActionDescriptor.GetParameters()
                .FirstOrDefault(p => p.GetCustomAttributes<FromBodyAttribute>().Any() || !p.ParameterType.IsValueType);
            if (parameter == null)
            {
                filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "No parameter found for action.");
                return;
            }

            try
            {
                // Decrypt data
                var decryptedData = Decrypt(filterContext.Request.GetDependencyScope(), encryptedData);

                // Validate that decrypted data is valid JSON
                var formData = JsonSerializer.Deserialize(decryptedData, parameter.ParameterType, options: JsonOptions.SafeJsonForHtmlAttributes);

                // Replace the request content with the deserialized object
                filterContext.ActionArguments[parameter.ParameterName] = formData;

                SetRequestContentStream(request, decryptedData);

                return;
            }
            catch (JsonException)
            {
                // Handle invalid JSON format
                filterContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Exception in decryption.");
                return;
            }
        }

        base.OnActionExecuting(filterContext);
    }

    /// <summary>
    /// Reads the request content stream as a string.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static string GetRequestContentStream(HttpRequestMessage request)
    {
        var stream = request.Content.ReadAsStreamAsync().Result;
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Replaces the request content with a new stream containing the specified JSON string.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="jsonString">The JSON string to set as the request content.</param>
    private static void SetRequestContentStream(HttpRequestMessage request, string jsonString)
    {

        request.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //filterContext.Request.Content = new ObjectContent(parameter.ParameterType, formData, parameter.Configuration.Formatters.JsonFormatter);
    }

    /// <summary>
    /// Decrypts the specified encrypted data.
    /// </summary>
    /// <param name="dependencyScope"></param>
    /// <param name="encryptedData"></param>
    /// <returns></returns>
    private static string Decrypt(IDependencyScope dependencyScope, EncryptedData encryptedData)
    {
        var cryptoService = (AesHybridCryptographyService)dependencyScope.GetService(typeof(AesHybridCryptographyService));
        return cryptoService.Decrypt(encryptedData);
    }
}
#endif
