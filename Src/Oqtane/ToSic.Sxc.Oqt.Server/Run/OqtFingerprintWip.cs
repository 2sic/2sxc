﻿using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Oqtane.Shared;
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

        public OqtFingerprintWip(IDbConfiguration dbConfiguration)
        {
            _dbConfiguration = dbConfiguration;
        }

        public string GetSystemFingerprint()
        {
            var systemGuid = Guid.Empty;

            var mainVersion = Assembly.GetAssembly(typeof(SiteState))?.GetName().Version?.Major.ToString();

            var mainVersion2Sxc = Settings.Version.Major.ToString();

            var dbName = _dbConfiguration.ConnectionString;

            var fingerprint = $"guid={systemGuid}&sys=oqt&vsys={mainVersion}&v2sxc={mainVersion2Sxc}&db={dbName}";

            return Hash(fingerprint);
        }

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
