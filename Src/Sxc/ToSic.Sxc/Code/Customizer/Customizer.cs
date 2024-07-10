using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Code.Customizer;

internal class Customizer(): ServiceForDynamicCode(SxcLogName + ".CdeCst"), ICodeCustomizer
{
    public IAppTyped<TSettings, TResources> App<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new()
    {
        // check if cache exists and was created with the sames specs
        if (_app is IAppTyped<TSettings, TResources> typed) return typed;

        // Get and cache for next time
        var created = _CodeApiSvc.GetService<IAppTyped<TSettings, TResources>>(reuse: true);
        _app = created;
        return created;
    }
    private object _app;

    public ICmsView<TSettings, TResources> MyView<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new()
    {
        // check if cache exists and was created with the sames specs
        if (_view is ICmsView<TSettings, TResources> typed) return typed;

        // Get and cache for reuse
        var cmsContext = _CodeApiSvc.CmsContext as CmsContext;
        var created = new CmsView<TSettings, TResources>(cmsContext, cmsContext?.RealBlockOrNull, false);
        _view = created;
        return created;
    }

    private ICmsView _view;

    public TCustomType MyItem<TCustomType>()
        where TCustomType : class, ITypedItem, ITypedItemWrapper16, new()
    {
        // check if cache exists and was created with the sames specs
        if (_myItem is TCustomType typed) return typed;

        var created = _CodeApiSvc.Cdf.AsCustom<TCustomType>((_CodeApiSvc.Data as ContextData)?.MyItem.FirstOrDefault());
        _myItem = created;
        return created;
    }
    private object _myItem;

    public IEnumerable<TCustomType> MyItems<TCustomType>()
        where TCustomType : class, ITypedItem, ITypedItemWrapper16, new()
    {
        // check if cache exists and was created with the sames specs
        if (_myItems is IEnumerable<TCustomType> typed) return typed;
        
        // Get and cache for reuse
        var created = _CodeApiSvc.Cdf.AsCustomList<TCustomType>((_CodeApiSvc.Data as ContextData)?.MyItem ?? [], default, nullIfNull: false);
        _myItems = created;
        return created;
    }
    private object _myItems;

    public TCustomType MyHeader<TCustomType>()
        where TCustomType : class, ITypedItem, ITypedItemWrapper16, new()
    {
        // check if cache exists and was created with the sames specs
        if (_myHeader is TCustomType typed) return typed;

        // Get and cache for reuse
        var created = _CodeApiSvc.Cdf.AsCustom<TCustomType>((_CodeApiSvc.Data as ContextData)?.MyHeader.FirstOrDefault());
        _myHeader = created;
        return created;
    }
    private object _myHeader;

}
