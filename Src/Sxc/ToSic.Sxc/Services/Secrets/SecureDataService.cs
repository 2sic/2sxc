using ToSic.Eav.Security.Encryption;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using static System.StringComparison;

namespace ToSic.Sxc.Services;

/// <summary>
/// Note: this is still a very temporary implementation, WIP
///
/// For now we have a super-trivial encryption for keys which are not really critical,
/// but only 2sxc distributions actually encrypt stuff. So it's not for any other use yet.
///
/// To encrypt other values, use the SecureDataTest.DumpEncryptedValue() code and get the encrypted value from the Trace
/// </summary>
[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class SecureDataService(AesCryptographyService aes)
    : ServiceBase($"{SxcLogName}.SecDtS"), ISecureDataService
{
    public readonly AesCryptographyService Aes = aes;

    public const string PrefixSecure = "secure:";
    public const string PrefixIv = "iv:";
    public const char ValueSeparator = ';';

    public ISecureData<string> Parse(string value)
    {
        var l = Log.Fn<ISecureData<string>>(enabled: Debug);
        if (string.IsNullOrWhiteSpace(value))
            return l.Return(new SecureData<string>(value, false), $"{nameof(value)} null/empty");

        // remove prefix which should be required, but ATM not enforced
        if (!value.StartsWith(PrefixSecure, InvariantCultureIgnoreCase))
            return l.Return(new SecureData<string>(value, false), $"not secured, missing prefix {PrefixSecure}");
            
        var probablySecure = value.Substring(PrefixSecure.Length);
        var parts = probablySecure.Split(ValueSeparator);
        var toDecrypt = parts[0];
        var iv = parts.Length <= 1 
            ? null 
            : parts[1].StartsWith(PrefixIv, InvariantCultureIgnoreCase)
                ? parts[1].Substring(PrefixIv.Length)
                : null;

        try
        {
            // will return null if it fails
            var decrypted = Aes.DecryptFromBase64(toDecrypt, new(true) { InitializationVector64 = iv });
            return decrypted == null 
                ? l.Return(new SecureData<string>(value, false), $"{nameof(decrypted)} null/empty")
                : l.Return(new SecureData<string>(decrypted, true), "decrypted");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            // if all fails, return the original
            return l.Return(new SecureData<string>(value, false), "error decrypting");
        }
    }

    public string Create(string value)
    {
        var l = Log.Fn<string>(enabled: Debug);
        if (string.IsNullOrWhiteSpace(value))
            return l.Return(null, "null/empty");

        try
        {
            // will return null if it fails
            var encrypt64 = Aes.EncryptToBase64(value);
            if (encrypt64.Value == null)
                return l.Return("", "empty encryption");
            var final = PrefixSecure + encrypt64.Value + ValueSeparator + PrefixIv + encrypt64.Iv;
            return l.Return(final, "encrypted");

        }
        catch (Exception ex)
        {
            l.Done(ex);
            throw;
        }
    }

    public string HashSha256(string value)
    {
        var l = Log.Fn<string>(enabled: Debug);
        return l.Return(Sha256.Hash(value ?? ""));
    }

    public string HashSha512(string value)
    {
        var l = Log.Fn<string>(enabled: Debug);
        return l.Return(Sha512.Hash(value ?? ""));
    }

    public bool Debug { get; set; }
}