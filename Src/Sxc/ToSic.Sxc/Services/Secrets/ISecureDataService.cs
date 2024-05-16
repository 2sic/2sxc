using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services;

/// <summary>
/// Helper to work with secure / encrypted data. 
/// </summary>
/// <remarks>
/// History
/// * Added in 2sxc 12.05
/// </remarks>
[PublicApi]
public interface ISecureDataService: IHasLog, ICanDebug
{
    /// <summary>
    /// Read an input value and return a secure data object.
    /// This will contain the readable value and additional information if it was encrypted or not, etc.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ISecureData<string> Parse(string value);

    //[PrivateApi("WIP v15.01")]
    //string Create(string value);

    /// <summary>
    /// Hash a value using SHA256, using a FIPS compliant provider.
    /// </summary>
    /// <param name="value">value to hash, `null` will be treated as empty string</param>
    /// <returns>the hash as a ???</returns>
    /// <remarks>Added v17.08</remarks>
    string HashSha256(string value); //, NoParamOrder protector = default, string salt = default);

    /// <summary>
    /// Hash a value using SHA512, using a FIPS compliant provider.
    /// </summary>
    /// <param name="value">value to hash, `null` will be treated as empty string</param>
    /// <returns>the hash as a ???</returns>
    /// <remarks>Added v17.08</remarks>
    string HashSha512(string value); //, NoParamOrder protector = default, string salt = default);

    // Note to my future self: HashSha3_256 and HashSha3_512 are not implemented yet
    // reason is that they are not FIPS compliant until .net 8 according to 
    // https://docs.microsoft.com/en-us/dotnet/standard/security/cryptographic-services#hash-algorithms
    // https://stackoverflow.com/questions/47679476/how-to-generate-sha3-256-in-net-core
}