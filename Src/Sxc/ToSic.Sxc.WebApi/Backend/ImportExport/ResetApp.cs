using System.IO;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.ImportHelpers;
using ToSic.Eav.ImportExport.Internal.Zip;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Security;
using ToSic.Sxc.Apps.Internal.Work;

namespace ToSic.Sxc.Backend.ImportExport;

/// <summary>
/// This object will ensure that an app is reset to the state it was in when the app.xml was last exported
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ResetApp(
    LazySvc<XmlImportWithFiles> xmlImportWithFilesLazy,
    ImpExpHelpers impExpHelpers,
    WorkAppsRemove workAppsRemove,
    ISite site,
    IUser user,
    IImportExportEnvironment env,
    ZipImport zipImport,
    IEavFeaturesService features,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.Export",
        connect:
        [
            xmlImportWithFilesLazy, impExpHelpers, workAppsRemove, site, user, env, zipImport, features, appPathSvc
        ])
{

    internal ImportResultDto Reset(int zoneId, int appId, string defaultLanguage, bool withSiteFiles)
    {
        var l = Log.Fn<ImportResultDto>($"Reset App {zoneId}/{appId}");
        var result = new ImportResultDto();

        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log);

        // Ensure feature available...
        ExportApp.SyncWithSiteFilesVerifyFeaturesOrThrow(features, withSiteFiles);

        var contextZoneId = site.ZoneId;
        var appRead = impExpHelpers.GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, contextZoneId);
        var appPaths = appPathSvc.Get(appRead, site);

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
            result.Messages.Add(new($"Can't find the {Eav.Constants.AppDataFile} in the folder", Message.MessageTypes.Error));
            return result;
        }

        // 2. Now we can delete the app before we prepare the import
        workAppsRemove.RemoveAppInSiteAndEav(zoneId, appId, false);

        // 3. Optional reset SiteFiles
        if (withSiteFiles)
        {
            var sourcePath = Path.Combine(appPaths.PhysicalPath, Eav.Constants.AppDataProtectedFolder);

            // Copy app global template files persisted in /App_Data/2sexyGlobal/ back to app [globalTemplatesRoot]
            var globalTemplatesStateFolder = Path.Combine(appDataFolder, Eav.Constants.ZipFolderForGlobalAppStuff);
            if (Directory.Exists(globalTemplatesStateFolder))
            {
                zipImport.Init(zoneId, appId, allowCode: true);
                var discard = new List<Message>();
                zipImport.CopyAppGlobalFiles(discard, appId, sourcePath, deleteGlobalTemplates: true, overwriteFiles: true);
            }

            // Copy portal files persisted in /App_Data/SiteFiles/ back to site
            env.TransferFilesToSite(Path.Combine(sourcePath, Eav.Constants.ZipFolderForSiteFiles), string.Empty);
        }

        // 4. Now import the App.xml
        var allowSystemChanges = user.IsSystemAdmin;
        var xmlImport = xmlImportWithFilesLazy.Value.Init(defaultLanguage, allowSystemChanges);
        var imp = new ImportXmlReader(filePath, xmlImport, Log);
        result.Success = xmlImport.ImportXml(zoneId, appId, parentAppId: null /* not sure if we never have a parent here */, imp.XmlDoc);
        result.Messages.AddRange(xmlImport.Messages);
        return l.Return(result);
    }
}