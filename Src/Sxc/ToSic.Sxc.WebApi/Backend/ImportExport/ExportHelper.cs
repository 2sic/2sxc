using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.WebApi.Sys.Security;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Capabilities.SysFeatures;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Backend.ImportExport;

public class ExportHelper(
    Generator<ImpExpHelpers> impExpHelpers,
    ISite site,
    IUser user,
    Generator<ZipExport, ZipExport.Options> exportGenerator,
    IAppReaderFactory appReadFac,
    IAppPathsMicroSvc appPathSvc
) : ServiceBase("Sxc.ImExHl", connect: [impExpHelpers, site, user, exportGenerator, appReadFac, appPathSvc])
{

    internal (IAppReader appReader, ZipExport zipExport) GetZipExportAndCheckZoneSwitchPermissions(IAppIdentity appIdentity)
    {
        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        var appReader = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(appIdentity, user, contextZoneId: site.ZoneId);
        var appPaths = appPathSvc.Get(appReader, site);
        var zipExport = exportGenerator.New(new()
        {
            ZoneId = appIdentity.ZoneId,
            AppId = appIdentity.AppId,
            AppFolder = appReader.Specs.Folder,
            PhysicalAppPath = appPaths.PhysicalPath,
            PhysicalPathGlobal = appPaths.PhysicalPathShared
        });
        return (appReader, zipExport);
    }

    internal static void SyncWithSiteFilesVerifyFeaturesOrThrow(ISysFeaturesService features, bool withSiteFiles)
    {
        if (!withSiteFiles)
            return;
        features.ThrowIfNotEnabled("Requires features enabled to sync with site files ",
            BuiltInFeatures.AppSyncWithSiteFiles.Guid);
    }

}