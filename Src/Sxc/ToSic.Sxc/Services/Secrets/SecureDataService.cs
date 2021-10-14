using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Security.Encryption;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Note: this is still a very temporary implementation, WIP
    /// </summary>
    [PrivateApi("Hide implementation")]
    public class SecureDataService: ISecureDataService
    {
        public const string PrefixSecure = "Secure:";

        public ISecureData<string> Parse(string value)
        {
            if (value == null) return new SecureData<string>(null, false);
            if (string.IsNullOrWhiteSpace(value)) return new SecureData<string>(value, false);

            var optimized = value;
            if (optimized.StartsWith(PrefixSecure, StringComparison.InvariantCultureIgnoreCase))
                optimized = optimized.Substring(PrefixSecure.Length);

            try
            {
                // will return null if it fails
                var decrypted = BasicAesCryptography.Decrypt(optimized);
                if (decrypted != null)
                    return new SecureData<string>(decrypted, true);
            }
            catch { /* ignore */ }

            // if all fails, return the original
            return new SecureData<string>(value, false);
        }
    }
}
