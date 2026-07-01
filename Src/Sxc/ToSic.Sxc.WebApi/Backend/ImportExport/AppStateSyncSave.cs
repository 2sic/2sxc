using ToSic.Eav.Data.Processing;
using ISite = ToSic.Eav.Context.ISite;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppStateSyncSave(
    Generator<ZipExport, ZipExport.Options> exportGenerator,
    ISite site,
    Generator<ImpExpHelpers> impExpHelpers,
    ISysFeaturesService features)
    : ServiceBase("Bck.Export", connect: [exportGenerator, site, features, impExpHelpers]),
        ILowCodeAction<AppExportSpecs, bool>
{

    public async Task<ActionData<bool>> Run(LowCodeActionContext context, ActionData<AppExportSpecs> data)
    {
        var specs = data.Data;
        var l = Log.Fn<bool>(specs.Dump());

        if (features.IsEnabled(BuiltInFeatures.AppStateSyncSaveDisabled))
            throw new FeaturesRefusingException(BuiltInFeatures.AppStateSyncSaveDisabled.NameId,
                "App Sync Save Disabled is active, probably as a protective measure.");

        
        // Ensure feature available...
        ExportHelper.SyncWithSiteFilesVerifyFeaturesOrThrow(features, specs.WithSiteFiles);

        var (appRead, appPaths) = impExpHelpers.New().GetReaderAndPathsAfterZoneSwitchPermissionCheck(specs);

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