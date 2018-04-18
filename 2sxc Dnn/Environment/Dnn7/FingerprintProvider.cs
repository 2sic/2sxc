using DotNetNuke.Application;
using ToSic.Eav.Interfaces;
using System.Data.SqlClient;

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

            var fingerprint = $"guid={hostGuid}&dnnv={mainVersionDnn}&2sxcv={mainVersion2Sxc}&db={dbName}";

            return fingerprint;
        }

        private string getDbName()
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
            {
                ConnectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString()
            };
            return connBuilder.InitialCatalog;
        }
    }
}
