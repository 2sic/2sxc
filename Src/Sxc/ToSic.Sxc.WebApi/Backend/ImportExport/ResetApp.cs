using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.ImportHelpers;
using ToSic.Eav.ImportExport.Internal.Zip;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;

namespace ToSic.Sxc.Backend.ImportExport;

/// <summary>
/// This object will ensure that an app is reset to the state it was in when the app.xml was last exported
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ResetApp: ServiceBase
{

    #region Constructor / DI

    public ResetApp(
        LazySvc<XmlImportWithFiles> xmlImportWithFilesLazy,
        ImpExpHelpers impExpHelpers,
        WorkAppsRemove workAppsRemove,
        ISite site,
        IUser user,
        IImportExportEnvironment env,
        ZipImport zipImport,
        IEavFeaturesService features,
        IAppPathsMicroSvc appPathSvc
    ) : base("Bck.Export")
    {
        ConnectServices(
            _xmlImportWithFilesLazy = xmlImportWithFilesLazy,
            _impExpHelpers = impExpHelpers,
            _workAppsRemove = workAppsRemove,
            _site = site,
            _user = user,
            _env = env,
            _zipImport = zipImport,
            _features = features,
            _appPathSvc = appPathSvc
        );
    }

    private readonly IAppPathsMicroSvc _appPathSvc;
    private readonly WorkAppsRemove _workAppsRemove;

    private readonly LazySvc<XmlImportWithFiles> _xmlImportWithFilesLazy;
    private readonly ImpExpHelpers _impExpHelpers;
    private readonly ISite _site;
    private readonly IUser _user;
    private readonly IImportExportEnvironment _env;
    private readonly ZipImport _zipImport;
    private readonly IEavFeaturesService _features;

    #endregion

    internal ImportResultDto Reset(int zoneId, int appId, string defaultLanguage, bool withSiteFiles)
    {
        var l = Log.Fn<ImportResultDto>($"Reset App {zoneId}/{appId}");
        var result = new ImportResultDto();

        SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log);

        // Ensure feature available...
        ExportApp.SyncWithSiteFilesVerifyFeaturesOrThrow(_features, withSiteFiles);

        var contextZoneId = _site.ZoneId;
        var appRead = _impExpHelpers.GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
        var appPaths = _appPathSvc.Init(_site, appRead);

        // migrate old .data/app.xml to App_Data
        ZipImport.MigrateOldAppDataFile(appPaths.PhysicalPath);

        //// 1. Verify the file exists before we flush
        //var path = Path.Combine(currentApp.PhysicalPath, Eav.Constants.AppDataProtectedFolder);
        //if (!Directory.Exists(path))
        //{
        //    result.Success = false;
        //    result.Messages.Add(new Message($"Error: Path to {Eav.Constants.AppDataFile} not found on hard disk", Message.MessageTypes.Error));
        //    return result;
        //}

        var appDataFolder = Path.Combine(appPaths.PhysicalPath, Eav.Constants.AppDataProtectedFolder);
        var filePath = Path.Combine(appDataFolder, Eav.Constants.AppDataFile);
        if (!File.Exists(filePath))
        {
            result.Success = false;
            result.Messages.Add(new Message($"Can't find the {Eav.Constants.AppDataFile} in the folder", Message.MessageTypes.Error));
            return result;
        }

        // 2. Now we can delete the app before we prepare the import
        _workAppsRemove.RemoveAppInSiteAndEav(zoneId, appId, false);

        // 3. Optional reset SiteFiles
        if (withSiteFiles)
        {
            var sourcePath = Path.Combine(appPaths.PhysicalPath, Eav.Constants.AppDataProtectedFolder);

            // Copy app global template files persisted in /App_Data/2sexyGlobal/ back to app [globalTemplatesRoot]
            var globalTemplatesStateFolder = Path.Combine(appDataFolder, Eav.Constants.ZipFolderForGlobalAppStuff);
            if (Directory.Exists(globalTemplatesStateFolder))
            {
                _zipImport.Init(zoneId, appId, allowCode: true);
                var discard = new List<Message>();
                _zipImport.CopyAppGlobalFiles(discard, appId, sourcePath, deleteGlobalTemplates: true, overwriteFiles: true);
            }

            // Copy portal files persisted in /App_Data/SiteFiles/ back to site
            _env.TransferFilesToSite(Path.Combine(sourcePath, Eav.Constants.ZipFolderForSiteFiles), string.Empty);
        }

        // 4. Now import the App.xml
        var allowSystemChanges = _user.IsSystemAdmin;
        var xmlImport = _xmlImportWithFilesLazy.Value.Init(defaultLanguage, allowSystemChanges);
        var imp = new ImportXmlReader(filePath, xmlImport, Log);
        result.Success = xmlImport.ImportXml(zoneId, appId, imp.XmlDoc);
        result.Messages.AddRange(xmlImport.Messages);
        return l.Return(result);
    }
}