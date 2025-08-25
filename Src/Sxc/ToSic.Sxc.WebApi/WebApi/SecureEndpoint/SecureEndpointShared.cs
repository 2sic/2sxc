using System.Text.Json;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sys.Security.Encryption;

namespace ToSic.Sxc.WebApi;

internal class SecureEndpointShared
{
    public const string MediaJson = "application/json";

    public const string ErrorIfBodyIsEmpty = "Request content is empty. Endpoint uses the SecureEndpoint, which expects data in the POST body.";

    public static EncryptedData? TestAndGetEncryptedData(string jsonString)
    {
        // Deserializes the JSON string to an EncryptedData object and performs a "EncryptedData" check.
        // If encrypted data is missing, sets the request content stream to the original JSON string and returns.
        var maybeEncrypted = JsonSerializer.Deserialize<EncryptedDataRaw>(jsonString, options: JsonOptions.SafeJsonForHtmlAttributes);
        if (maybeEncrypted == null || (maybeEncrypted.Version == 1 && (maybeEncrypted.Data is null || maybeEncrypted.Key is null || maybeEncrypted.Iv is null)))
            return null;

        var encryptedData = new EncryptedData
        {
            Data = maybeEncrypted.Data ?? throw new JsonException("Encrypted data is missing."),
            Key = maybeEncrypted.Key ?? throw new JsonException("Encryption key is missing."),
            Iv = maybeEncrypted.Iv ?? throw new JsonException("Initialization vector (IV) is missing."),
            Version = maybeEncrypted.Version
        };
        return encryptedData;
    }

}


/// <remarks>
/// If the data is encrypted, it would need to be required. But because we're test-deserializing this to do duck-checking, it cannot be required.
/// See also <see cref="EncryptedData"/>
/// </remarks>
public class EncryptedDataRaw
{
    public int Version { get; init; } = 1;

    public string? Data { get; init; }

    public string? Key { get; init; }

    public string? Iv { get; init; }
}
