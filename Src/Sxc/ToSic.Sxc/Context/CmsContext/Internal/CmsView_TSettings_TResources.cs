using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context.Internal;

internal class CmsView<TSettings, TResources>(CmsContext parent, IBlock block) : CmsView(parent, block), ICmsView<TSettings, TResources>
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    private readonly IView _view = block.View;
    private readonly CmsContext _parent = parent;

    public TSettings Settings => _settings.Get(() => _parent._CodeApiSvc.Cdf.AsCustom<TSettings>(_view.Settings));
    private readonly GetOnce<TSettings> _settings = new();

    public TResources Resources => _resources.Get(() => _parent._CodeApiSvc.Cdf.AsCustom<TResources>(_view.Resources));
    private readonly GetOnce<TResources> _resources = new();
}