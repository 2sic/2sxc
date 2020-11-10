using System;
using System.IO;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.ImportExport.ImportHelpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi.ImportExport
{
    /// <summary>
    /// This object will ensure that an app is reset to the state it was in when the app.xml was last exported
    /// </summary>
    public class ResetApp: HasLog
    {

        #region Constructor / DI

        public ResetApp(IZoneMapper zoneMapper, 
            Lazy<XmlImportWithFiles> xmlImportWithFilesLazy,
            ImpExpHelpers impExpHelpers,
            CmsZones cmsZones) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _xmlImportWithFilesLazy = xmlImportWithFilesLazy;
            _impExpHelpers = impExpHelpers;
            _cmsZones = cmsZones;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly Lazy<XmlImportWithFiles> _xmlImportWithFilesLazy;
        private readonly ImpExpHelpers _impExpHelpers;
        private readonly CmsZones _cmsZones;
        private IUser _user;
        private int _siteId;
        public ResetApp Init(int siteId, IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _user = user;
            _siteId = siteId;
            return this;
        }

        #endregion

        public ImportResultDto Reset(int zoneId, int appId, string defaultLanguage)
        {
            Log.Add($"Reset App {zoneId}/{appId}");
            var result = new ImportResultDto();

            var allowSystemChanges = _user.IsSuperUser;

            SecurityHelpers.ThrowIfNotAdmin(_user);

            var contextZoneId = _zoneMapper.GetZoneId(_siteId);
            var currentApp = _impExpHelpers.Init(Log).GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            // 1. Verify the file exists before we flush
            var path = currentApp.PhysicalPath + "\\" + Eav.Constants.FolderData;
            if (!Directory.Exists(path))
            {
                result.Success = false;
                result.Messages.Add(new Message("Error: Path to app.xml not found on hard disk", Message.MessageTypes.Error));
                return result;
            }

            var filePath = Path.Combine(path, Eav.Constants.AppDataFile);
            if (!File.Exists(filePath))
            {
                result.Success = false;
                result.Messages.Add(new Message($"Can't find the {Eav.Constants.AppDataFile} in the folder", Message.MessageTypes.Error));
                return result;
            }

            // 2. Now we can delete the app before we prepare the import
            _cmsZones.Init(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId, false);

            var xmlImport = _xmlImportWithFilesLazy.Value.Init(defaultLanguage, allowSystemChanges, Log);
            var imp = new ImportXmlReader(filePath, xmlImport, Log);
            result.Success = xmlImport.ImportXml(zoneId, appId, imp.XmlDoc);
            result.Messages.AddRange(xmlImport.Messages);
            return result;
        }
    }
}
