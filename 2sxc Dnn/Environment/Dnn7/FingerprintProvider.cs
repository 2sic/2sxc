using ToSic.Eav.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class FingerprintProvider: IFingerprintProvider
    {
        public string GetSystemFingerprint()
        {
            // todo: find host-guid
            var hostGuid = "todo";

            // todo: find dnn version
            var mainVersionDnn = "9"; // todo

            // todo: find 2sxc version
            var mainVersion2Sxc = "9"; // todo

            // todo: find db-name
            var dbName = "db-todo"; // todo

            // todo: connect together
            var fingerprint = $"guid={hostGuid}&dnnv={mainVersionDnn}&2sxcv={mainVersion2Sxc}&db={dbName}";

            return fingerprint;
        }
    }
}
