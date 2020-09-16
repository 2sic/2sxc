using System;
using System.IO;
using System.Xml.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.WebApi.ImportExport
{
    internal class ImportContent: HasLog
    {

        #region Constructor / DI

        public ImportContent(IZoneMapper zoneMapper, IHttp http) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _http = http;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly IHttp _http;
        private IUser _user;

        public ImportContent Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _user = user;
            return this;
        }

        #endregion

        public ImportResultDto Import(int zoneId, int appId, string fileName, Stream stream, string defaultLanguage,
            Action<Exception> logException)
        {
            Log.Add("import content start");
            var result = new ImportResultDto();

            var allowSystemChanges = _user.IsSuperUser;
            if (fileName.EndsWith(".zip"))
            {   // ZIP
                try
                {
                    var env = Factory.Resolve<IImportExportEnvironment>().Init(Log);
                    var zipImport = new ZipImport(env, zoneId, appId, _user.IsSuperUser, Log);
                    // Increase script timeout to prevent timeouts
                    var temporaryDirectory = _http.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory,
                        Guid.NewGuid().ToString()));
                    result.Succeeded = zipImport.ImportZip(stream, temporaryDirectory);
                    result.Messages = env.Messages;
                }
                catch (Exception ex)
                {
                    logException(ex);
                }
            }
            else
            {   // XML
                using (var fileStreamReader = new StreamReader(stream))
                {
                    var xmlImport = new XmlImportWithFiles(Log, defaultLanguage, allowSystemChanges);
                    var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                    result.Succeeded = xmlImport.ImportXml(zoneId, appId, xmlDocument);
                    result.Messages = xmlImport.Messages;
                }
            }
            return result;
        }

    }
}
