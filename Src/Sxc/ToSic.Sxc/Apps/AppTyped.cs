using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.DataServices;
using ToSic.Sxc.Services.Internal;
using static ToSic.Sxc.Apps.AppAssetFolderMain;

namespace ToSic.Sxc.Apps;

// Wip, NOT PRODUCTION READY #IAppTyped
// Goal is to use this instead of the App, to be a clean wrapper hiding the old App
// and to ensure only this API works with the new typed data

// It's not complete, because ATM it assumes it's already receiving an AppTyped - which is what this should be for.
// So to complete
// - provide an instance of this on the CodeApiService
// - use that instead

internal class AppTyped(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager> queryManager)
    : ServiceForDynamicCode(SxcLogName + ".AppTyp", errorIfNotConnected: true, connect: [globalPaths, queryManager]),
        IAppTyped
{
    protected App App => CodeApiSvc.App as App ?? throw new($"Can't access {nameof(App)} - either null or can't convert");

    /// <inheritdoc />
    int IZoneIdentity.ZoneId => App.ZoneId;

    /// <inheritdoc />
    int IAppIdentityLight.AppId => App.AppId;

    /// <inheritdoc />
    public string Name => App.Name;

    /// <inheritdoc />
    public IAppDataTyped Data => _data ??= new Func<IAppDataTyped>(() =>
    {
        var data = App.BuildDataForTyped<AppDataTyped, AppDataTyped>();
        data.Setup(((ICodeApiServiceInternal)CodeApiSvc).GetKit<ServiceKit16>());
        return data;
    })();
    private IAppDataTyped _data;

    /// <inheritdoc />
    public IDataSource GetQuery(string name = default, NoParamOrder noParamOrder = default, IDataSourceLinkable attach = default, object parameters = default)
    {
        var opts = new DataSourceOptionsMs(this, () => App.ConfigurationProvider);
        return new GetQueryMs(queryManager, opts, Log).GetQuery(name, noParamOrder, attach, parameters);
    }

    /// <inheritdoc />
    public IAppConfiguration Configuration => App.Configuration;

    /// <inheritdoc />
    ITypedItem IAppTyped.Settings => _settings.Get(() => App.AppSettingsForTyped.NullOrGetWith(appS => MakeTyped(appS, propsRequired: true)));
    private readonly GetOnce<ITypedItem> _settings = new();

    /// <inheritdoc />
    ITypedItem IAppTyped.Resources => _resources.Get(() => App.AppResourcesForTyped.NullOrGetWith(appR => MakeTyped(appR, propsRequired: true)));
    private readonly GetOnce<ITypedItem> _resources = new();

    private ITypedItem MakeTyped(ICanBeEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents.Entity, false);
        return CodeApiSvc.Cdf.AsItem(wrapped, propsRequired: propsRequired);
    }

    /// <inheritdoc />
    public IFolder Folder => _folder ??= (this as IAppTyped).FolderAdvanced();
    private IFolder _folder;

    /// <inheritdoc />
    public IFolder FolderAdvanced(NoParamOrder noParamOrder = default, string location = default)
        => new AppAssetFolderMain(App.AppPathsForTyped, App.Folder, DetermineShared(location) ?? App.AppReaderForTyped.IsShared());

    /// <inheritdoc />
    public IFile Thumbnail => _thumbnailFile.Get(() => new AppAssetThumbnail(App.AppReaderForTyped, App.AppPathsForTyped, globalPaths));
    private readonly GetOnce<IFile> _thumbnailFile = new();


}
