using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Coding;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Services.DataServices;
using static ToSic.Sxc.Apps.AppAssetFolderMain;

namespace ToSic.Sxc.Apps;

public partial class App: IAppTyped
{
    #region IAppTyped Folder / Thumbnail (replaces Paths)

    IFolder IAppTyped.Folder => _folder ??= (this as IAppTyped).FolderAdvanced();
    private IFolder _folder;

    IFolder IAppTyped.FolderAdvanced(NoParamOrder noParamOrder, string location) 
        => new AppAssetFolderMain(AppPaths, Folder, DetermineShared(location) ?? AppStateInt.IsShared());

    IFile IAppTyped.Thumbnail => _thumbnailFile.Get(() => new AppAssetThumbnail(AppStateInt, AppPaths, _globalPaths));
    private readonly GetOnce<IFile> _thumbnailFile = new();

    #endregion

    #region Data

    IAppDataTyped IAppTyped.Data => _data ??= BuildData<AppDataTyped, IAppDataTyped>();
    private IAppDataTyped _data;

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
        return _cdfLazy.Value.AsItem(wrapped, propsRequired: propsRequired);
    }


}