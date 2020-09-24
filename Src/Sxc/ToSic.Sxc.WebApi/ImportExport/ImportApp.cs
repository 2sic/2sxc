using System;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.WebApi.ImportExport
{
    internal class ImportApp: HasLog
    {

        #region Constructor / DI

        public ImportApp(IZoneMapper zoneMapper, IHttp http, IEnvironmentLogger envLogger) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _http = http;
            _envLogger = envLogger;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly IHttp _http;
        private readonly IEnvironmentLogger _envLogger;
        private IUser _user;

        public ImportApp Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _user = user;
            return this;
        }

        #endregion

        public ImportResultDto Import(int zoneId, string name, Stream stream)
        {
            Log.Add("import app start");
            var result = new ImportResultDto();

            if (!string.IsNullOrEmpty(name)) Log.Add($"new app name: {name}");

            var zipImport = Factory.Resolve<ZipImport>();
            try
            {
                zipImport.Init(zoneId, null, _user.IsSuperUser, Log);
                var temporaryDirectory = _http.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory, Guid.NewGuid().ToString().Substring(0, 8)));

                // Increase script timeout to prevent timeouts
                result.Succeeded = zipImport.ImportZip(stream, temporaryDirectory, name);
                result.Messages.AddRange(zipImport.Messages);
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                result.Succeeded = false;
                result.Messages.AddRange(zipImport.Messages);
            }
            return result;
        }

    }
}
