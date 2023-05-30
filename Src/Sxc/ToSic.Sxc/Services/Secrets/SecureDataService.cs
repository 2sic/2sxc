using System;
using ToSic.Eav.Security.Encryption;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using static System.StringComparison;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Note: this is still a very temporary implementation, WIP
    ///
    /// For now we have a super-trivial encryption for keys which are not really critical,
    /// but only 2sxc distributions actually encrypt stuff. So it's not for any other use yet.
    ///
    /// To encrypt other values, use the SecureDataTest.DumpEncryptedValue() code and get the encrypted value from the Trace
    /// </summary>
    [PrivateApi("Hide implementation")]
    public class SecureDataService: ServiceBase, ISecureDataService
    {
        public readonly AesCryptographyService Aes;

        public SecureDataService(AesCryptographyService aes) : base($"{Constants.SxcLogName}.SecDtS")
        {
            Aes = aes;
        }

        public const string PrefixSecure = "secure:";
        public const string PrefixIv = "iv:";
        public const char ValueSeparator = ';';

        public ISecureData<string> Parse(string value) => Log.Func(value, enabled: Debug, func: l =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return (new SecureData<string>(value, false), $"{nameof(value)} null/empty");

            var optimized = value;

            // remove prefix which should be required, but ATM not enforced
            if (!optimized.StartsWith(PrefixSecure, InvariantCultureIgnoreCase))
                return (new SecureData<string>(value, false), $"not secured, missing prefix {PrefixSecure}");
            
            var probablySecure = optimized.Substring(PrefixSecure.Length);
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
                var decrypted = Aes.DecryptFromBase64(toDecrypt, new AesConfiguration(true) { InitializationVector64 = iv });
                return decrypted == null 
                    ? (new SecureData<string>(value, false), $"{nameof(decrypted)} null/empty")
                    : (new SecureData<string>(decrypted, true), "decrypted");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                // if all fails, return the original
                return (new SecureData<string>(value, false), "error decrypting");
            }
        });

        public string Create(string value) => Log.Func(enabled: Debug, func: l =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return (null, "null/empty");

            try
            {
                // will return null if it fails
                var encrypt64 = Aes.EncryptToBase64(value);
                if (encrypt64.Value != null)
                {
                    var final = PrefixSecure + encrypt64.Value + ValueSeparator + PrefixIv + encrypt64.Iv;
                    return (final, "encrypted");
                }

                return ("", "empty encryption");
            }
            catch (Exception ex)
            {
                throw l.Done(ex);
            }
        });

        public bool Debug { get; set; }
    }
}
