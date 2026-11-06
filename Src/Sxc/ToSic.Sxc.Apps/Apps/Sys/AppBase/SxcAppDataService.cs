using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// Shared underlying code to get data source lookup etc. for Custom Code.
/// </summary>
/// <param name="dsFactory"></param>
/// <param name="dataConfigProvider"></param>
public class SxcAppDataService(IDataSourcesService dsFactory, IAppDataConfigProvider dataConfigProvider)
    : ServiceWithSetup<SxcAppDataService.Options>("App.PrvData", connect: [dsFactory, dataConfigProvider])
{
    public record Options(IAppForCode App, AppDataConfigSpecs? DataConfigSpecs);

    public ILookUpEngine LookUpEngine => field
        ??= AppDataConfig.LookUpEngine
           ?? throw new("Cannot provide Data for the object App as crucial information is missing. " +
                        "Please call InitData first to provide this data.");

    protected internal IAppDataConfiguration AppDataConfig => field ??= new Func<IAppDataConfiguration>(() =>
    {
        // New v17
        var config = dataConfigProvider.GetDataConfiguration(MyOptions.App, DataConfigSpecs);

        // needed to initialize data - must always happen a bit later because the show-draft info isn't available when creating the first App-object.
        // todo: later this should be moved to initialization of this object
        Log.A($"init data drafts:{config.ShowDrafts}, hasConf:{config.LookUpEngine}");
        return config;

    })();

    /// <summary>
    /// for deferred initialization as needed
    /// </summary>
    private AppDataConfigSpecs DataConfigSpecs => field ??= MyOptions.DataConfigSpecs ?? new AppDataConfigSpecs();

    
    [PrivateApi]
    public TResult BuildData<TDataSource, TResult>(IAppIdentity pureIdentity)
        where TDataSource : TResult
        where TResult : class, IDataSource
    {
        var l = Log.Fn<TResult>();
        if (LookUpEngine == null) // note: as of 2025-09, I believe this can never be null.
            throw new("Cannot provide Data for the object App as crucial information is missing. " +
                      "Please call InitData first to provide this data.");

        // Note: ModulePermissionController does not work when indexing, return false for search
        var options = new DataSourceOptions
        {
            AppIdentityOrReader = pureIdentity,
            LookUp = LookUpEngine,
            ShowDrafts = AppDataConfig.ShowDrafts,
        };
        var initialSource = dsFactory.CreateDefault(options);
        var appDataWithCreate = dsFactory.Create<TDataSource>(options with { Attach = initialSource, }) as TResult;

        return l.Return(appDataWithCreate);
    }
}