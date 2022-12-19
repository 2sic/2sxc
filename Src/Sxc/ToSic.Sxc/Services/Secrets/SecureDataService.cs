using System;
using ToSic.Eav.Security.Encryption;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;

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
    public class SecureDataService: HasLog, ISecureDataService
    {
        public SecureDataService() : base(Constants.SxcLogName + ".SecDtS") { }

        public const string PrefixSecure = "Secure:";

        public ISecureData<string> Parse(string value)
        {
            var l = Log.Fn<ISecureData<string>>(value);
            if (value == null)
                return l.Return(new SecureData<string>(null, false), "null");
            if (string.IsNullOrWhiteSpace(value))
                return l.Return(new SecureData<string>(value, false), "empty");

            var optimized = value;
            if (optimized.StartsWith(PrefixSecure, StringComparison.InvariantCultureIgnoreCase))
                optimized = optimized.Substring(PrefixSecure.Length);

            try
            {
                // will return null if it fails
                var decrypted = BasicAesCryptography.Decrypt(optimized);
                if (decrypted != null)
                    return l.Return(new SecureData<string>(decrypted, true), "decrypted");
            }
            catch { /* ignore */ }

            // if all fails, return the original
            return l.Return(new SecureData<string>(value, false), "unchanged");
        }
    }
}
