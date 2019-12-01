using DotNetNuke.Application;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using ToSic.Eav.Configuration;
using ToSic.Sxc;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class FingerprintProvider: IFingerprintProvider
    {
        public string GetSystemFingerprint()
        {
            var hostGuid = DotNetNuke.Entities.Host.Host.GUID;

            var mainVersionDnn = DotNetNukeContext.Current.Application.Version.ToString(1);

            var mainVersion2Sxc = Settings.Version.ToString(1);

            var dbName = getDbName();

            var fingerprint = $"guid={hostGuid}&vdnn={mainVersionDnn}&v2sxc={mainVersion2Sxc}&db={dbName}";

            return Hash(fingerprint);
        }

        private string getDbName()
        {
            var connBuilder = new SqlConnectionStringBuilder
            {
                ConnectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString()
            };
            return connBuilder.InitialCatalog;
        }

        //public static string Base64Encode(string plainText)
        //{
        //    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        //    return System.Convert.ToBase64String(plainTextBytes);
        //}

        public static string Hash(string randomString)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
