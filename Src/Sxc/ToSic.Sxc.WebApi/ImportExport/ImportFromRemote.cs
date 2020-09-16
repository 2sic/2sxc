using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi.ImportExport
{
    internal class ImportFromRemote: HasLog
    {

        #region Constructor / DI

        public ImportFromRemote() : base("Bck.Export")
        {
        }

        private IUser _user;

        public ImportFromRemote Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _user = user;
            return this;
        }

        #endregion

        public Tuple<bool, List<Message>> InstallPackage(int zoneId, int appId, bool isApp, string packageUrl, Action<Exception> logException)
        {
            var callLog = Log.Call($"{nameof(zoneId)}:{zoneId}, {nameof(appId)}:{appId}, {nameof(isApp)}:{isApp}, url:{packageUrl}");
            Log.Add("install package:" + packageUrl);
            if(!_user.IsAdmin) throw new Exception("must be admin");
            bool success;

            var importer = Factory.Resolve<ZipFromUrlImport>();
            try
            {
                success = importer.Init(zoneId, appId, _user.IsSuperUser, Log)
                    .ImportUrl(packageUrl, isApp);
            }
            catch (Exception ex)
            {
                logException(ex);
                throw new Exception("An error occurred while installing the app: " + ex.Message, ex);
            }

            callLog(success.ToString());
            return new Tuple<bool, List<Message>>(success, importer.Messages);//  Request.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, new { success, importer.Messages });
        }


    }
}
