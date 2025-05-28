using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources.Internal;
using ToSic.Lib.Helpers;
using ToSic.Lib.LookUp.Engines;

namespace ToSic.Eav.Apps.Internal;

partial class EavApp
{
    [PrivateApi]
    public ILookUpEngine ConfigurationProvider => _configurationProvider.Get(() => AppDataConfig.Configuration);
    private readonly GetOnce<ILookUpEngine> _configurationProvider = new();

    private IAppDataConfiguration AppDataConfig => _appDataConfigOnce.Get(() =>
    {
        // New v17
        var config = Services.DataConfigProvider.GetDataConfiguration(this, _dataConfigSpecs ?? new AppDataConfigSpecs());

        // needed to initialize data - must always happen a bit later because the show-draft info isn't available when creating the first App-object.
        // todo: later this should be moved to initialization of this object
        Log.A($"init data drafts:{config.ShowDrafts}, hasConf:{config.Configuration != null}");
        return config;

    });
    private readonly GetOnce<IAppDataConfiguration> _appDataConfigOnce = new();
    private AppDataConfigSpecs _dataConfigSpecs;

    #region Data



    /// <inheritdoc />
    public IAppData Data => field ??= BuildData<AppDataWithCrud, IAppData>();

    [PrivateApi]
    protected TResult BuildData<TDataSource, TResult>() where TDataSource : TResult where TResult : class, IDataSource
    {
        var l = Log.Fn<TResult>();
        if (ConfigurationProvider == null)
            throw new("Cannot provide Data for the object App as crucial information is missing. " +
                      "Please call InitData first to provide this data.");

        // Note: ModulePermissionController does not work when indexing, return false for search
        var initialSource = Services.DataSourceFactory.CreateDefault(
            new DataSourceOptions
            {
                AppIdentityOrReader = this,
                LookUp = ConfigurationProvider,
                ShowDrafts = AppDataConfig?.ShowDrafts
            });
        var appDataWithCreate = Services.DataSourceFactory.Create<TDataSource>(attach: initialSource) as TResult;

        return l.Return(appDataWithCreate);
    }

    #endregion
}