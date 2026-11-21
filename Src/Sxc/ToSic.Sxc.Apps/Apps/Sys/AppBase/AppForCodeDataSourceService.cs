using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// Shared underlying code to get data source lookup etc. for Custom Code.
/// </summary>
/// <param name="dsFactory"></param>
/// <param name="dataConfigProvider"></param>
public class AppForCodeDataSourceService(IDataSourcesService dsFactory, IAppDataConfigProvider dataConfigProvider)
    : ServiceWithSetup<AppForCodeDataSourceService.Options>("App.PrvData", connect: [dsFactory, dataConfigProvider])
{
    /// <summary>
    /// Options, setup is required.
    /// </summary>
    /// <param name="App">App, required</param>
    /// <param name="DataConfigSpecs">Specs - optional, but usually provided</param>
    public record Options(IAppForCode App, AppDataConfigSpecs? DataConfigSpecs);

    public ILookUpEngine LookUpEngine => field
        ??= AppDataConfig.LookUpEngine
           ?? throw new("Cannot provide Data for the object App as crucial information is missing. " +
                        "Please call InitData first to provide this data.");

    private IAppDataConfiguration AppDataConfig => field ??= new Func<IAppDataConfiguration>(() =>
    {
        var configSpecs = MyOptions.DataConfigSpecs ?? new AppDataConfigSpecs();
        var config = dataConfigProvider.GetDataConfiguration(MyOptions.App, configSpecs);

        // needed to initialize data - must always happen a bit later because the show-draft info isn't available when creating the first App-object.
        // todo: later this should be moved to initialization of this object
        Log.A($"init data drafts:{config.ShowDrafts}, hasConf:{config.LookUpEngine}");
        return config;

    })();
    
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