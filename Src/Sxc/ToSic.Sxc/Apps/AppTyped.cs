using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Apps;

// Wip, NOT PRODUCTION READY
// Goal is to use this instead of the App, to be a clean wrapper hiding the old App
// and to ensure only this API works with the new typed data

// It's not complete, because ATM it assumes it's already receiving an AppTyped - which is what this should be for.
// So to complete
// - change so it uses the old IApp
// - put the code / services here needed to create an IAppTyped
// - provide an instance of this on the CodeApiService
// - use that instead

internal class AppTyped() : ServiceForDynamicCode(SxcLogName + ".AppTyp"), IAppTyped
{
    private ICodeApiService CodeApiSvc => _CodeApiSvc ?? throw new($"Can't access {nameof(CodeApiSvc)} - either null or can't convert");

    private IAppTyped App => CodeApiSvc.App as IAppTyped ?? throw new($"Can't access {nameof(App)} - either null or can't convert");

    int IZoneIdentity.ZoneId => App.ZoneId;

    int IAppIdentityLight.AppId => App.AppId;

    string IAppTyped.Name => App.Name;

    IAppDataTyped IAppTyped.Data => App.Data;

    IDataSource IAppTyped.GetQuery(string name, NoParamOrder noParamOrder, IDataSourceLinkable attach, object parameters)
        => App.GetQuery(name, noParamOrder, attach, parameters);

    IAppConfiguration IAppTyped.Configuration => App.Configuration;

    ITypedItem IAppTyped.Settings => _typedSettings.Get(() => (App.Settings as ICanBeEntity).NullOrGetWith(appS => MakeTyped(appS, propsRequired: true)));
    private readonly GetOnce<ITypedItem> _typedSettings = new();

    ITypedItem IAppTyped.Resources => _typedRes.Get(() => (App.Resources as ICanBeEntity).NullOrGetWith(appR => MakeTyped(appR, propsRequired: true)));
    private readonly GetOnce<ITypedItem> _typedRes = new();

    private ITypedItem MakeTyped(ICanBeEntity contents, bool propsRequired)
    {
        var wrapped = CmsEditDecorator.Wrap(contents.Entity, false);
        return CodeApiSvc.Cdf.AsItem(wrapped, propsRequired: propsRequired);
    }

    IFolder IAppTyped.Folder => App.Folder;

    IFolder IAppTyped.FolderAdvanced(NoParamOrder noParamOrder, string location)
        => App.FolderAdvanced(noParamOrder, location);

    IFile IAppTyped.Thumbnail => App.Thumbnail;


}
