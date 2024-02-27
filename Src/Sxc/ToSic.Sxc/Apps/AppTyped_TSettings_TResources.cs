using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Apps;

internal class AppTyped<TSettings, TResources>() : ServiceForDynamicCode(SxcLogName + ".AppTyp"), IAppTyped<TSettings, TResources>
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    private ICodeApiService CodeApiSvc => _CodeApiSvc ?? throw new($"Can't access {nameof(CodeApiSvc)} - either null or can't convert");

    private IAppTyped App => CodeApiSvc.App as IAppTyped ?? throw new($"Can't access {nameof(App)} - either null or can't convert");


    int IZoneIdentity.ZoneId => App.ZoneId;

    int IAppIdentityLight.AppId => App.AppId;

    string IAppTyped<TSettings, TResources>.Name => App.Name;

    IAppDataTyped IAppTyped<TSettings, TResources>.Data => App.Data;

    IDataSource IAppTyped<TSettings, TResources>.GetQuery(string name, NoParamOrder noParamOrder, IDataSourceLinkable attach, object parameters)
        => App.GetQuery(name, noParamOrder, attach, parameters);

    IAppConfiguration IAppTyped<TSettings, TResources>.Configuration => App.Configuration;

    TSettings IAppTyped<TSettings, TResources>.Settings => _settings.Get(() => AsCustom<TSettings>(App.Settings));
    private readonly GetOnce<TSettings> _settings = new();

    TResources IAppTyped<TSettings, TResources>.Resources => _resources.Get(() => AsCustom<TResources>(App.Resources));
    private readonly GetOnce<TResources> _resources = new();

    private T AsCustom<T>(ICanBeEntity original)
        where T : class, ITypedItem, ITypedItemWrapper16, new()
        => CodeApiSvc._Cdf.AsCustom<T>(original, default, mock: false);

    IFolder IAppTyped<TSettings, TResources>.Folder => App.Folder;

    IFolder IAppTyped<TSettings, TResources>.FolderAdvanced(NoParamOrder noParamOrder, string location)
        => App.FolderAdvanced(noParamOrder, location);

    IFile IAppTyped<TSettings, TResources>.Thumbnail => App.Thumbnail;
}
