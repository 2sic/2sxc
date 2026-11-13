using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.ImportExport.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Sys.Ancestors;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.DataProviders;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppStatisticsProvider(
    ICultureStateSvc cultureStateSvc,
    AppWorkChain<WorkEntities> workEntities,
    ExportHelper exportHelper,
    ISite site
) : ServiceWithSetup<IAppWorkContext>("Bck.Export", connect: [workEntities, cultureStateSvc, site, exportHelper]),
    IDataSourceRawDataProviderWithCtx
{
    public const string NameId = "System.AppStatistics";
    public const string NameIdEav = "System.AppStatistics.Eav";

    public IEnumerable<IRawData> Provide(IDataSourceConfiguration configuration)
        => [GetAppInfo()];

    public AppExportInfoRaw GetAppInfo()
    {
        var l = Log.Fn<AppExportInfoRaw>($"get app info for app: {MyOptions.Show()}");
        var (appReader, zipExport) = exportHelper.GetZipExportAndCheckZoneSwitchPermissions(MyOptions);

        var appEntities = workEntities.New(MyOptions);

        var appHasCustomParent = appReader.HasCustomParentApp();

        // Files Count = PortalFilesCount + GlobalFilesCount
        var filesCount = zipExport.CountFiles(!appHasCustomParent, fm => fm.AllFiles());

        // TransferablePortalFilesCount + TransferableGlobalFilesCount
        var transferableFilesCount = zipExport.CountFiles(!appHasCustomParent, fm => fm.GetAllTransferableFiles());

        var appSpecs = appReader.Specs;
        return l.Return(new()
        {
            Id = MyOptions.AppId,
            Name = appSpecs.Name,
            NameId = appSpecs.NameId,
            Version = appSpecs.VersionSafe(),
            EntitiesCount = appEntities.All().Count(e => !e.HasAncestor()),
            LanguagesCount = cultureStateSvc.CulturesEnabledWithState(site).Count,
            TemplatesCount = 0,
            HasRazorTemplates = false,
            HasTokenTemplates = false,
            FilesCount = filesCount,
            TransferableFilesCount = transferableFilesCount,
        });
    }

}