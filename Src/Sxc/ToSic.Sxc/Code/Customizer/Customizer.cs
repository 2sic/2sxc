using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Code.Customizer;

internal class Customizer(): ServiceForDynamicCode(SxcLogName + ".CdeCst"), ICodeCustomizer
{
    public IAppTyped<TSettings, TResources> App<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new()
    {
        _app ??= _CodeApiSvc.GetService<IAppTyped<TSettings, TResources>>(reuse: true);
        return _app as IAppTyped<TSettings, TResources>;
    }
    private object _app;

    public ICmsView<TSettings, TResources> View<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new()
    {
        var cmsContext = _CodeApiSvc.CmsContext as CmsContext;
        _view ??= new CmsView<TSettings, TResources>(cmsContext, cmsContext?.RealBlockOrNull);
        return _view as ICmsView<TSettings, TResources>;
    }

    private ICmsView _view;

}
