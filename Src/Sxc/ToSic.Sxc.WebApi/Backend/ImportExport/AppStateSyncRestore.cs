using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.ImportExport.Integration;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.ImportHelpers;
using ToSic.Eav.ImportExport.Sys.XmlImport;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Persistence.Sys.Logging;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.HookUp;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Backend.ImportExport;

/// <summary>
/// This object will ensure that an app is reset to the state it was in when the app.xml was last exported
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppStateSyncRestore(
    LazySvc<XmlImportWithFiles> xmlImportWithFilesLazy,
    ImpExpHelpers impExpHelpers,
    WorkAppsRemove workAppsRemove,
    ISite site,
    IUser user,
    IImportExportEnvironment env,
    ZipImport zipImport,
    ISysFeaturesService features)
    : ServiceBase("Bck.Export",
        connect: [xmlImportWithFilesLazy, impExpHelpers, workAppsRemove, site, user, env, zipImport, features]),
        IWork<AppStateSyncRestore.Parameters, ImportResultDto>
{
    public record Parameters(int ZoneId, int AppId, string DefaultLanguage, bool WithSiteFiles): IAppIdentity;

    public async Task<Package<ImportResultDto>> Handle(WorkContext context, Package<Parameters> package)
    {
        var parameters = package.Data;
        var l = Log.Fn<ImportResultDto>($"Reset App {parameters.Show()}");
        var result = new ImportResultDto();


        if (features.IsEnabled(BuiltInFeatures.AppStateSyncRestoreDisabled))
            throw new FeaturesRefusingException(BuiltInFeatures.AppStateSyncRestoreDisabled.NameId,
                "App Sync Restore Disabled is active, probably as a protective measure.");

        // Ensure feature available...
        ExportHelper.SyncWithSiteFilesVerifyFeaturesOrThrow(features, parameters.WithSiteFiles);

        var (_, appPaths) = impExpHelpers.GetReaderAndPathsAfterZoneSwitchPermissionCheck(parameters);

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

        var appDataFolder = Path.Combine(appPaths.PhysicalPath, FolderConstants.DataFolderProtected);
        var filePath = Path.Combine(appDataFolder, FolderConstants.AppDataFile);
        if (!File.Exists(filePath))
        {
            result.Success = false;
            result.Messages.Add(new($"Can't find the {FolderConstants.AppDataFile} in the folder", Message.MessageTypes.Error));
            return new(result);
        }

        var allowSystemChanges = user.IsSystemAdmin;
        var xmlImport = xmlImportWithFilesLazy.Value.Init(parameters.DefaultLanguage, allowSystemChanges);
        var imp = new ImportXmlReader(filePath, xmlImport, l);

        // Informational only: a failed audit must never prevent resetting from the source-control export.
        try
        {
            var validator = new PathCasePreflightValidator(l);
            // Reset uses the pending-app layout: app files at the root and saved shared/site files under App_Data.
            var preflight = validator.ValidateImportPackage(appPaths.PhysicalPath, imp.XmlDoc, pendingApp: true);
            _ = validator.LogResult(preflight);
        }
        catch (Exception e)
        {
            l.W("Path case preflight failed; reset will continue");
            l.Ex(e);
        }

        // 2. Now we can delete the app before we prepare the import
        var zoneId = parameters.ZoneId;
        var appId = parameters.AppId;
        workAppsRemove.RemoveAppInSiteAndEav(zoneId, appId, false);

        // 3. Optional reset SiteFiles
        if (parameters.WithSiteFiles)
        {
            var sourcePath = Path.Combine(appPaths.PhysicalPath, FolderConstants.DataFolderProtected);

            // Copy app global template files persisted in /App_Data/2sexyGlobal/ back to app [globalTemplatesRoot]
            var globalTemplatesStateFolder = Path.Combine(appDataFolder, FolderConstants.ZipFolderForGlobalAppStuff);
            if (Directory.Exists(globalTemplatesStateFolder))
            {
                zipImport.Init(zoneId, appId, allowCode: true);
                var discard = new List<Message>();
                zipImport.CopyAppGlobalFiles(discard, appId, sourcePath, deleteGlobalTemplates: true, overwriteFiles: true);
            }

            // Copy portal files persisted in /App_Data/SiteFiles/ back to site
            env.TransferFilesToSite(Path.Combine(sourcePath, FolderConstants.ZipFolderForSiteFiles), string.Empty);
        }

        // 4. Now import the App.xml
        result.Success = xmlImport.ImportXml(zoneId, appId, parentAppId: null /* not sure if we never have a parent here */, imp.XmlDoc);
        result.Messages.AddRange(xmlImport.Messages);
        return new(l.Return(result));
    }
}
