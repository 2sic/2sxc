using System;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.WebApi.ImportExport
{
    internal class ImportApp: HasLog
    {

        #region Constructor / DI

        public ImportApp(IZoneMapper zoneMapper, IHttp http) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _http = http;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly IHttp _http;
        private IUser _user;

        public ImportApp Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _user = user;
            return this;
        }

        #endregion

        public ImportResultDto Import(int zoneId, string name, Stream stream, Action<Exception> logException)
        {
            Log.Add("import app start");
            var result = new ImportResultDto();

            //var request = HttpContext.Current.Request;
            //var server = HttpContext.Current.Server;
            //server.ScriptTimeout = 300;

            //var zoneId = int.Parse(request["ZoneId"]);
            //if (request.Files.Count <= 0) return result;

            //var name = request["Name"];
            if (!string.IsNullOrEmpty(name))
            {
                Log.Add($"new app name: {name}");
            }

            var helper = Factory.Resolve<IImportExportEnvironment>().Init(Log);
            try
            {
                var zipImport = new ZipImport(helper, zoneId, null, _user.IsSuperUser, Log);
                var temporaryDirectory = _http.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory, Guid.NewGuid().ToString().Substring(0, 8)));

                // Increase script timeout to prevent timeouts
                result.Succeeded = zipImport.ImportZip(stream, temporaryDirectory, name);
                result.Messages = helper.Messages;
            }
            catch (Exception ex)
            {
                logException(ex);
                result.Succeeded = false;
                result.Messages = helper.Messages;
            }
            return result;
        }

    }
}
