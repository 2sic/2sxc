#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToSic.Eav.Security.Encryption;
using JsonOptions = ToSic.Eav.Serialization.JsonOptions;

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

    public SecureEndpointAttribute()
    {
        Order = -3002;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        if (request.Method == HttpMethods.Post && request.ContentType == MediaType)
        {
            var jsonString = await GetRequestContentStream(request);
            if (string.IsNullOrEmpty(jsonString))
            {
                context.Result = new BadRequestObjectResult("Request content is empty.");
                return;
            }

            // Deserializes the JSON string to an EncryptedData object and performs a "EncryptedData" check.
            // If encrypted data is missing, sets the request content stream to the original JSON string and returns.
            var encryptedData = JsonSerializer.Deserialize<EncryptedData>(jsonString, JsonOptions.SafeJsonForHtmlAttributes);
            if (encryptedData?.Data is null && encryptedData?.Key is null && encryptedData?.Iv is null && encryptedData?.Version == 1)
            {
                //SetRequestContentStream(request, jsonString);
                return;
            }

            // Currently, only POST payloads can be encrypted. 
            // Try to determine the POST parameter dynamically by finding a parameter 
            // that has the FromBody attribute on it or a parameter that is not a value type.
            var parameter = context.ActionDescriptor.Parameters
                .FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body || !p.ParameterType.IsValueType);
            if (parameter == null)
            {
                context.Result = new BadRequestObjectResult("No parameter found for action.");
                return;
            }

            try
            {
                // Decrypt data
                var decryptedData = Decrypt(context.HttpContext.RequestServices, encryptedData);

                // Validate that decrypted data is valid JSON
                var formData = JsonSerializer.Deserialize(decryptedData, parameter.ParameterType, JsonOptions.SafeJsonForHtmlAttributes);

                // Replace the request content with the deserialized object
                context.ActionArguments[parameter.Name] = formData;

                SetRequestContentStream(request, decryptedData);
            }
            catch (JsonException)
            {
                // Handle invalid JSON format
                context.Result = new BadRequestObjectResult("Exception in decryption.");
                return;
            }
        }

        await base.OnActionExecutionAsync(context, next);
    }

    /// <summary>
    /// Reads the request content stream as a string.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static async Task<string> GetRequestContentStream(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var content = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return content;
    }


    /// <summary>
    /// Replaces the request content with a new stream containing the specified JSON string.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="jsonString">The JSON string to set as the request content.</param>
    private void SetRequestContentStream(HttpRequest request, string jsonString)
    {
        var newStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        request.Body = newStream;
        request.Body.Position = 0;
        request.ContentType = MediaType;
    }

    /// <summary>
    /// Decrypts the specified encrypted data.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="encryptedData"></param>
    /// <returns></returns>
    private static string Decrypt(IServiceProvider serviceProvider, EncryptedData encryptedData)
    {
        var cryptoService = serviceProvider.GetRequiredService<AesHybridCryptographyService>();
        return cryptoService.Decrypt(encryptedData);
    }
}
#endif
