using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using DotNetNuke.Application;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// DNN implementation of the Fingerprinting system for extra security.
    /// </summary>
    [PrivateApi("probably not useful in the API docs.")]
    public class DnnFingerprint: IFingerprint
    {
        public string GetSystemFingerprint()
        {
            var sysGuid = DotNetNuke.Entities.Host.Host.GUID;

            var mainVersionSys = DotNetNukeContext.Current.Application.Version.ToString(1);

            var mainVersion2Sxc = Settings.Version.ToString(1);

            var dbName = getDbName();

            // todo: probably add &sys=dnn
            // todo: probably change &vdnn to be &vsys
            // Wait with these changes till we're sure we don't break stuff; ATM the fingerprint isn't critical yet
            var fingerprint = $"guid={sysGuid}&vdnn={mainVersionSys}&v2sxc={mainVersion2Sxc}&db={dbName}";

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

        private static string Hash(string randomString)
        {
            var hash = new StringBuilder();
            using (var crypt = new SHA256Managed())
            {
                var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
                foreach (var theByte in crypto) 
                    hash.Append(theByte.ToString("x2"));
            } 
            return hash.ToString();
        }
    }
}
