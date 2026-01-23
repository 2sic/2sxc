using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys.CmsContext;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Customizer;

/// <inheritdoc cref="ICodeCustomizer" />
internal class Customizer(): ServiceWithContext(SxcLogName + ".CdeCst"), ICodeCustomizer
{
    public IAppTyped<TSettings, TResources> App<TSettings, TResources>()
        where TSettings : class, ICanWrapData, new()
        where TResources : class, ICanWrapData, new()
    {
        // check if cache exists and was created with the sames specs
        if (_app is IAppTyped<TSettings, TResources> typed)
            return typed;

        // Get and cache for next time
        var created = ExCtx.GetService<IAppTyped<TSettings, TResources>>(reuse: true);
        _app = created;
        return created;
    }
    private object? _app;

    public ICmsView<TSettings, TResources> MyView<TSettings, TResources>()
        where TSettings : class, ICanWrapData, new()
        where TResources : class, ICanWrapData, new()
    {
        // check if cache exists and was created with the sames specs
        if (_view is ICmsView<TSettings, TResources> typed)
            return typed;

        // Get and cache for reuse
        var cmsContext = (CmsContext)ExCtx.GetCmsContext();
        var created = new CmsView<TSettings, TResources>(cmsContext, cmsContext.BlockInternal, false);
        _view = created;
        return created;
    }

    private ICmsView? _view;

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    public TCustomType? MyItem<TCustomType>()
        where TCustomType : class, ICanWrapData, new()
    {
        // check if cache exists and was created with the sames specs
        if (_myItem is TCustomType typed)
            return typed;

        var firstEntity = ExCtx.GetContextData()?.MyItems.FirstOrDefault();
        var created = Cdf.AsCustom<TCustomType>(firstEntity);
        _myItem = created;
        return created;
    }
    private object? _myItem;

    public IEnumerable<TCustomType> MyItems<TCustomType>()
        where TCustomType : class, ICanWrapData, new()
    {
        // check if cache exists and was created with the sames type as is now requested
        if (_myItems is IEnumerable<TCustomType> typed)
            return typed;
        
        // Get and cache for reuse
        var items = ExCtx.GetContextData()?.MyItems ?? [];
        var created = Cdf.AsCustomList<TCustomType>(items, default, nullIfNull: false);
        _myItems = created;
        return created;
    }
    private object? _myItems; // not typed, since we don't know the type yet, but we know it will be IEnumerable<TCustomType> when used

    public TCustomType? MyHeader<TCustomType>()
        where TCustomType : class, ICanWrapData, new()
    {
        // check if cache exists and was created with the sames specs
        if (_myHeader is TCustomType typed)
            return typed;

        // Get and cache for reuse
        var header = ExCtx.GetContextData()?.MyHeaders.FirstOrDefault();
        var created = Cdf.AsCustom<TCustomType>(header);
        _myHeader = created;
        return created;
    }
    private object? _myHeader;

}
