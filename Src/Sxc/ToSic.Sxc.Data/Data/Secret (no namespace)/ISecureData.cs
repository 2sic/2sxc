namespace ToSic.Sxc.Data;

/// <summary>
/// Objects which contain secure/encrypted data and can be decrypted / verified.
/// </summary>
/// <remarks>
/// This object contains decrypted data (if it was encrypted originally) and tells you if the data was encrypted, signed etc.
/// It's still very basic, and will grow in functionality to assist in handling secure / encrypted / signed data.
/// 
/// History: Introduced in 2sxc 12.05
/// </remarks>
/// <typeparam name="TValue">
/// Type of the value in this secure data. As of now it's always a `string`.
/// </typeparam>
[PublicApi]
public interface ISecureData<out TValue>
{
    /// <summary>
    /// The value returned by the secure data - usually a string. 
    /// </summary>
    TValue Value { get; }

    //[PrivateApi("not final yet")]
    //bool IsEncrypted { get; }

    //[PrivateApi("not final yet")]
    //bool IsSigned { get; }

    //[PrivateApi("not final yet")]
    //SecureDataAuthorities Authority { get; }

    /// <summary>
    /// Determines if the data is secure data, so it's either encrypted or signed
    /// </summary>
    /// <remarks>made public in v17.01</remarks>
    bool IsSecured { get; }


    /// <summary>
    /// Determines what authority secured this secure data.
    /// This is to figure out what certificate or source verified the decryption / signing
    ///
    /// As of 2sxc 12.05, it can only be "preset", other keys are currently not handled yet
    /// </summary>
    /// <param name="authorityName"></param>
    /// <returns></returns>
    bool IsSecuredBy(string authorityName);

    /// <summary>
    /// This object explicitly has a ToString, so you can use the result in string concatenation like `"key:" + secureResult`
    /// </summary>
    /// <returns></returns>
    string? ToString();

}