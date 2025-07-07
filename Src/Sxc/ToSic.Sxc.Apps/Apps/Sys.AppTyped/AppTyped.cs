using System.Diagnostics.CodeAnalysis;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.DataServices;
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

internal class AppTyped(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager> queryManager)
    : ServiceWithContext(SxcLogName + ".AppTyp", connect: [globalPaths, queryManager]),
        IAppTyped
{
    protected App App => ExCtx.GetApp() as App
                         ?? throw new($"Can't access {nameof(App)} - either null or can't convert");

    /// <inheritdoc />
    int IZoneIdentity.ZoneId => App.ZoneId;

    /// <inheritdoc />
    int IAppIdentityLight.AppId => App.AppId;

    /// <inheritdoc />
    public string Name => App.Name;

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IAppDataTyped Data => field ??= App
        .BuildDataForTyped<AppDataTyped, AppDataTyped>()
        .Setup(Cdf);

    /// <inheritdoc />
    public IDataSource? GetQuery(string? name = default, NoParamOrder noParamOrder = default, IDataSourceLinkable? attach = default, object? parameters = default)
    {
        var opts = new DataSourceOptionsMs(this, () => App.ConfigurationProvider);
        return new GetQueryMs(queryManager, opts, Log).GetQuery(name, noParamOrder, attach, parameters);
    }

    /// <inheritdoc />
    public IAppConfiguration Configuration => App.Configuration;

    /// <inheritdoc />
    ITypedItem IAppTyped.Settings => _settings.Get(() => App.AppSettings.NullOrGetWith(appS => MakeTyped(appS, propsRequired: true))!)!;
    private readonly GetOnce<ITypedItem> _settings = new();

    /// <inheritdoc />
    ITypedItem IAppTyped.Resources => _resources.Get(() => App.AppResources.NullOrGetWith(appR => MakeTyped(appR, propsRequired: true))!)!;
    private readonly GetOnce<ITypedItem> _resources = new();

    private ITypedItem MakeTyped(ICanBeEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents.Entity, false);
        return Cdf.AsItem(wrapped, new() { ItemIsStrict = propsRequired })!;
    }

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IFolder Folder => field ??= (this as IAppTyped).FolderAdvanced();

    /// <inheritdoc />
    public IFolder FolderAdvanced(NoParamOrder noParamOrder = default, string? location = default)
        => new AppAssetFolderMain(App.AppPathsForTyped, App.Folder, AppAssetsHelpers.DetermineShared(location) ?? App.AppReaderForTyped.IsShared());

    /// <inheritdoc />
    public IFile Thumbnail => _thumbnailFile.Get(() => new AppAssetThumbnail(App.AppReaderForTyped, App.AppPathsForTyped, globalPaths))!;
    private readonly GetOnce<IFile> _thumbnailFile = new();


}
