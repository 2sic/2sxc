using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Coding;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Services.DataServices;
using static ToSic.Sxc.Apps.AppAssetFolderMain;

namespace ToSic.Sxc.Apps;

public partial class App: IAppTyped
{
    #region IAppTyped Folder / Thumbnail (replaces Paths)

    IFolder IAppTyped.Folder => _folder ??= (this as IAppTyped).FolderAdvanced();
    private IFolder _folder;

    IFolder IAppTyped.FolderAdvanced(NoParamOrder noParamOrder, string location)
    {
        //Protect(noParamOrder, nameof(location));
        return new AppAssetFolderMain(AppPaths, Folder, DetermineShared(location) ?? AppState.IsShared());
    }

    IFile IAppTyped.Thumbnail => _thumbnailFile.Get(() => new AppAssetThumbnail(this, AppPaths, _globalPaths));
    private readonly GetOnce<IFile> _thumbnailFile = new();

    #endregion

    #region GetQuery

    IDataSource IAppTyped.GetQuery(string name, NoParamOrder noParamOrder, IDataSourceLinkable attach, object parameters)
    {
        var opts = new DataSourceOptionsMs(this, () => ConfigurationProvider);
        return new GetQueryMs(Services.QueryManager, opts, Log).GetQuery(name, noParamOrder, attach, parameters);
    }

    #endregion

    /// <inheritdoc cref="IAppTyped.Settings"/>
    ITypedItem IAppTyped.Settings => AppSettings == null ? null : _typedSettings.Get(() => MakeTyped(AppSettings, propsRequired: true));
    private readonly GetOnce<ITypedItem> _typedSettings = new();

    /// <inheritdoc cref="IAppTyped.Resources"/>
    ITypedItem IAppTyped.Resources => _typedRes.Get(() => MakeTyped(AppResources, propsRequired: true));
    private readonly GetOnce<ITypedItem> _typedRes = new();

    private ITypedItem MakeTyped(IEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents, false);
        return _cdfLazy.Value.AsItem(wrapped, default, propsRequired: propsRequired);
    }

}