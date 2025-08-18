#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using ToSic.Sys.Security.Encryption;
using JsonOptions = ToSic.Eav.Serialization.Sys.Json.JsonOptions;

namespace ToSic.Sxc.WebApi;

[PrivateApi]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SecureEndpointAttribute : ActionFilterAttribute
{
    [PrivateApi]
    public SecureEndpointAttribute()
    {
        Order = -3002;
    }

    [PrivateApi]
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        if (request.Method != HttpMethods.Post || request.ContentType != SecureEndpointShared.MediaJson)
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        var jsonString = await GetRequestContentStream(request);
        if (string.IsNullOrEmpty(jsonString))
        {
            context.Result = new BadRequestObjectResult(SecureEndpointShared.ErrorIfBodyIsEmpty);
            return;
        }

        var encryptedData = SecureEndpointShared.TestAndGetEncryptedData(jsonString);
        if (encryptedData == null)
            return;

        // Currently, only POST payloads can be encrypted. 
        // Try to determine the POST parameter dynamically by finding a parameter 
        // that has the FromBody attribute on it or a parameter that is not a value type.
        // We need the final type because we will deserialize the decrypted data into it.
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
            var formData = JsonSerializer.Deserialize(decryptedData, parameter.ParameterType,
                JsonOptions.SafeJsonForHtmlAttributes);

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
        request.ContentType = SecureEndpointShared.MediaJson;
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
