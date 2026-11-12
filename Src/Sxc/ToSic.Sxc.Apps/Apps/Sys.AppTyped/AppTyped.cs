using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Services.DataService.Sys;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Apps.Sys.AppTyped;

// Wip, NOT PRODUCTION READY #IAppTyped
// Goal is to use this instead of the App, to be a clean wrapper hiding the old App
// and to ensure only this API works with the new typed data

// It's not complete, because ATM it assumes it's already receiving an AppTyped - which is what this should be for.
// So to complete
// - provide an instance of this on the CodeApiService
// - use that instead

internal class AppTyped(LazySvc<GlobalPaths> globalPaths, Generator<GetQueryMs<TypedQuery>, DataSourceOptionsMs> queryBuilder)
    : ServiceWithContext(SxcLogName + ".AppTyp", connect: [globalPaths, queryBuilder]),
        IAppTyped
{
    [field: AllowNull, MaybeNull]
    protected IAppForCode App => field
        ??= ExCtx.GetApp()
            ?? throw new($"Can't access {nameof(App)} - either null or can't convert");


    /// <inheritdoc />
    int IZoneIdentity.ZoneId => App.ZoneId;

    /// <inheritdoc />
    int IAppIdentityLight.AppId => App.AppId;

    /// <inheritdoc />
    public string Name => App.AppSpecs.Name;

    /// <summary>
    /// The CDF either comes from the ExecutionContext, or self-generated.
    /// This is important for scenarios where the App is used on a theme, where there is no execution context.
    /// </summary>
    [field: AllowNull, MaybeNull]
    protected ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IAppDataTyped Data => field ??= App.DataService
        .BuildData<AppDataTyped, AppDataTyped>(this.PureIdentity())
        .Setup(Cdf);

    /// <inheritdoc />
    public ITypedQuery? GetQuery(string? name = default, NoParamOrder npo = default, IDataSourceLinkable? attach = default, object? parameters = default)
    {
        var opts = new DataSourceOptionsMs(this, () => App.DataService.LookUpEngine);
        var queryMicroService = queryBuilder.New(opts);
        var query = queryMicroService.GetQuery(name, npo, attach, parameters);
        query?.Setup(Cdf);
        return query;
    }

    /// <inheritdoc />
    public IAppConfiguration Configuration => App.AppSpecs.Configuration;

    /// <inheritdoc />
    ITypedItem IAppTyped.Settings => _settings.Get(() => App.SettingsEntity.NullOrGetWith(appS => MakeTyped(appS, propsRequired: true)))!;
    private readonly LazyGet<ITypedItem?> _settings = new();

    /// <inheritdoc />
    ITypedItem IAppTyped.Resources => _resources.Get(() => App.ResourcesEntity.NullOrGetWith(appR => MakeTyped(appR, propsRequired: true)))!;
    private readonly LazyGet<ITypedItem?> _resources = new();
    
    private ITypedItem MakeTyped(ICanBeEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents.Entity, false);
        return Cdf.AsItem(wrapped, new() { ItemIsStrict = propsRequired })!;
    }

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IFolder Folder => field ??= (this as IAppTyped).FolderAdvanced();

    /// <inheritdoc />
    public IFolder FolderAdvanced(NoParamOrder npo = default, string? location = default)
        => new AppAssetFolderMain(App.AppPaths, App.AppSpecs.Folder, AppAssetsHelpers.DetermineShared(location) ?? App.AppReader.IsShared());

    /// <inheritdoc />
    public IFile Thumbnail => field
        ??= new AppAssetThumbnail(App.AppReader, App.AppPaths, globalPaths);
}
