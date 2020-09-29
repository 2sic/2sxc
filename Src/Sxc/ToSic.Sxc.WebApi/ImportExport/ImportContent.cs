using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.Validation;

namespace ToSic.Sxc.WebApi.ImportExport
{
    internal class ImportContent: HasLog
    {

        #region Constructor / DI

        public ImportContent(IZoneMapper zoneMapper, IHttp http, IEnvironmentLogger envLogger) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _http = http;
            _envLogger = envLogger;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly IHttp _http;
        private readonly IEnvironmentLogger _envLogger;
        private IUser _user;

        public ImportContent Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _user = user;
            return this;
        }

        #endregion

        public ImportResultDto Import(int zoneId, int appId, string fileName, Stream stream, string defaultLanguage)
        {
            Log.Add("import content start");
            var result = new ImportResultDto();

            var allowSystemChanges = _user.IsSuperUser;
            if (fileName.EndsWith(".zip"))
            {   // ZIP
                try
                {
                    var zipImport = Factory.Resolve<ZipImport>();

                    zipImport.Init(zoneId, appId, _user.IsSuperUser, Log);
                    var temporaryDirectory = _http.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory,
                        Guid.NewGuid().ToString()));
                    result.Success = zipImport.ImportZip(stream, temporaryDirectory);
                    result.Messages.AddRange(zipImport.Messages);
                }
                catch (Exception ex)
                {
                    _envLogger.LogException(ex);
                }
            }
            else
            {   // XML
                using (var fileStreamReader = new StreamReader(stream))
                {
                    var xmlImport = new XmlImportWithFiles(Log, defaultLanguage, allowSystemChanges);
                    var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                    result.Success = xmlImport.ImportXml(zoneId, appId, xmlDocument);
                    result.Messages.AddRange(xmlImport.Messages);
                }
            }
            return result;
        }


        public ImportResultDto ImportContentType(int zoneId, int appId, List<FileUploadDto> files, string defaultLanguage)
        {
            var callLog = Log.Call<ImportResultDto>($"{zoneId}, {appId}, {defaultLanguage}");

            try
            {
                // 0. Verify it's json etc.
                if (files.Any(file => !Json.IsValidJson(file.Contents)))
                    throw new ArgumentException("a file is not json");

                // 1. create the content type
                var serializer = new JsonSerializer(State.Get(new AppIdentity(zoneId, appId)), Log);

                var types = files.Select(f => serializer.DeserializeContentType(f.Contents) as ContentType).ToList();

                if (types.Any(t => t == null))
                    throw new NullReferenceException("One ContentType is null, something is wrong");

                // 2. Import the type
                var import = new Import(zoneId, appId, true, parentLog: Log);
                import.ImportIntoDb(types, null);

                Log.Add($"Purging {zoneId}/{appId}");
                SystemManager.Purge(zoneId, appId, log: Log);

                // 3. possibly show messages / issues
                return callLog("ok", new ImportResultDto(true));
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                return callLog("error", new ImportResultDto(false, ex.Message, Message.MessageTypes.Error));
            }
        }
    }
}
