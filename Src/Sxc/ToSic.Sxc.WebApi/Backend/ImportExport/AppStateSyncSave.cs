using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Data.Processing;
using ISite = ToSic.Eav.Context.ISite;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.WebApi.Sys.Security;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppStateSyncSave(
    Generator<ZipExport, ZipExport.Options> exportGenerator,
    ISite site,
    IUser user,
    Generator<ImpExpHelpers> impExpHelpers,
    ISysFeaturesService features,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.Export", connect: [exportGenerator, site, user, features, impExpHelpers, appPathSvc]),
        ILowCodeAction<AppExportSpecs, bool>
{

    public async Task<ActionData<bool>> Run(LowCodeActionContext context, ActionData<AppExportSpecs> data)
    {
        var specs = data.Data;
        var l = Log.Fn<bool>(specs.Dump());
        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        if (features.IsEnabled(BuiltInFeatures.AppStateSyncSaveDisabled))
            throw new FeaturesRefusingException(BuiltInFeatures.AppStateSyncSaveDisabled.NameId,
                "App Sync Save Disabled is active, probably as a protective measure.");

        // Ensure feature available...
        ExportHelper.SyncWithSiteFilesVerifyFeaturesOrThrow(features, specs.WithSiteFiles);

        var contextZoneId = site.ZoneId;
        var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(specs, user, contextZoneId);
        var appPaths = appPathSvc.Get(appRead, site);

        var zipExport = exportGenerator.New(new()
        {
            ZoneId = specs.ZoneId,
            AppId = specs.AppId,
            AppFolder = appRead.Specs.Folder,
            PhysicalAppPath = appPaths.PhysicalPath,
            PhysicalPathGlobal = appPaths.PhysicalPathShared
        });
        zipExport.ExportForSourceControl(specs);

        return new(l.ReturnTrue());
    }

}