using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ImportFromRemote: HasLog
    {
        private readonly IEnvironmentLogger _envLogger;
        private readonly ZipFromUrlImport _zipImportFromUrl;

        #region Constructor / DI

        public ImportFromRemote(IEnvironmentLogger envLogger, ZipFromUrlImport zipImportFromUrl) : base("Bck.Export")
        {
            _envLogger = envLogger;
            _zipImportFromUrl = zipImportFromUrl;
        }

        private IUser _user;

        public ImportFromRemote Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _user = user;
            return this;
        }

        #endregion

        public Tuple<bool, List<Message>> InstallPackage(int zoneId, int appId, bool isApp, string packageUrl)
        {
            var callLog = Log.Call($"{nameof(zoneId)}:{zoneId}, {nameof(appId)}:{appId}, {nameof(isApp)}:{isApp}, url:{packageUrl}");
            Log.Add("install package:" + packageUrl);
            if(!_user.IsAdmin) throw new Exception("must be admin");
            bool success;

            var importer = _zipImportFromUrl; // Factory.Resolve<ZipFromUrlImport>();
            try
            {
                success = importer.Init(zoneId, appId, _user.IsSuperUser, Log)
                    .ImportUrl(packageUrl, isApp);
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                throw new Exception("An error occurred while installing the app: " + ex.Message, ex);
            }

            callLog(success.ToString());
            return new Tuple<bool, List<Message>>(success, importer.Messages);
        }


    }
}
