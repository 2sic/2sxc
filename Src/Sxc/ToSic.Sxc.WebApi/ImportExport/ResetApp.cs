using System;
using System.IO;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.ImportExport.ImportHelpers;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi.ImportExport
{
    /// <summary>
    /// This object will ensure that an app is reset to the state it was in when the app.xml was last exported
    /// </summary>
    public class ResetApp: HasLog<ResetApp>
    {
        #region Constructor / DI

        public ResetApp(Lazy<XmlImportWithFiles> xmlImportWithFilesLazy,
            ImpExpHelpers impExpHelpers,
            CmsZones cmsZones,
            ISite site,
            IUser user
            ) : base("Bck.Export")
        {
            _xmlImportWithFilesLazy = xmlImportWithFilesLazy;
            _impExpHelpers = impExpHelpers;
            _cmsZones = cmsZones;
            _site = site;
            _user = user;
        }

        private readonly Lazy<XmlImportWithFiles> _xmlImportWithFilesLazy;
        private readonly ImpExpHelpers _impExpHelpers;
        private readonly CmsZones _cmsZones;
        private readonly ISite _site;
        private readonly IUser _user;

        #endregion

        public ImportResultDto Reset(int zoneId, int appId, string defaultLanguage)
        {
            Log.Add($"Reset App {zoneId}/{appId}");
            var result = new ImportResultDto();

            SecurityHelpers.ThrowIfNotAdmin(_user);

            var contextZoneId = _site.ZoneId;
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

            // 3. Now import the App.xml
            var allowSystemChanges = _user.IsSuperUser;
            var xmlImport = _xmlImportWithFilesLazy.Value.Init(defaultLanguage, allowSystemChanges, Log);
            var imp = new ImportXmlReader(filePath, xmlImport, Log);
            result.Success = xmlImport.ImportXml(zoneId, appId, imp.XmlDoc);
            result.Messages.AddRange(xmlImport.Messages);
            return result;
        }
    }
}
