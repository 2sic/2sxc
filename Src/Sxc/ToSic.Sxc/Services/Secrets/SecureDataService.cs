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
        public SecureDataService() : base($"{Constants.SxcLogName}.SecDtS") { }

        public const string PrefixSecure = "Secure:";

        public ISecureData<string> Parse(string value) => Log.Func(value, () =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return (new SecureData<string>(value, false), "null/empty");

            var optimized = value;

            // remove prefix which should be required, but ATM not enforced
            // TODO: should probably enforce this...
            if (optimized.StartsWith(PrefixSecure, InvariantCultureIgnoreCase))
                optimized = optimized.Substring(PrefixSecure.Length);

            try
            {
                // will return null if it fails
                var decrypted = BasicAesCryptography.DecryptAesCrypto(optimized);
                if (decrypted != null)
                    return (new SecureData<string>(decrypted, true), "decrypted");
            }
            catch { /* ignore */ }

            // if all fails, return the original
            return (new SecureData<string>(value, false), "unchanged");
        });
    }
}
