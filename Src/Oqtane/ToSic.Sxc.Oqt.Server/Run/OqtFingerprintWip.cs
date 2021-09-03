using Oqtane.Infrastructure;
using System;
using System.Security.Cryptography;
using System.Text;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

// #WIP low priority: Oqtane doesn't seem to have a unique system GUID
// That would be ideal for a real fingerprint

namespace ToSic.Sxc.Oqt.Server.Run
{
    [PrivateApi]
    public class OqtFingerprintWip: IFingerprint
    {
        private readonly IDbConfiguration _dbConfiguration;
        private readonly IConfigManager _configManager;

        public OqtFingerprintWip(IDbConfiguration dbConfiguration, IConfigManager configManager)
        {
            _dbConfiguration = dbConfiguration;
            _configManager = configManager;
        }

        public string GetSystemFingerprint()
        {
            if (_fingerprintCache != null) return _fingerprintCache;

            var systemGuid = _configManager.GetInstallationId();

            // getting Oqtane version is changed in Oqtane 2.2
            // Oqtane.Shared.Constants.Version is static readonly string
            // var mainVersion = Assembly.GetAssembly(typeof(SiteState))?.GetName().Version?.Major.ToString();
            var mainVersion = Version.Parse(Oqtane.Shared.Constants.Version)?.Major.ToString();

            var mainVersion2Sxc = Settings.Version.Major.ToString();

            var dbName = _dbConfiguration.ConnectionString;

            var fingerprint = $"guid={systemGuid}&sys=oqt&vsys={mainVersion}&v2sxc={mainVersion2Sxc}&db={dbName}";

            return _fingerprintCache = Hash(fingerprint);
        }

        private static string _fingerprintCache;

        private static string Hash(string randomString)
        {
            var hash = new StringBuilder();
            using var crypt = new SHA256Managed();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (var theByte in crypto)
                hash.Append(theByte.ToString("x2"));
            return hash.ToString();
        }

    }
}
