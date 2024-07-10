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

    /// <summary>
    /// Problem: in v16/17 we forgot to make propsRequired for Setting be true.
    /// So they were `false` - and the Content App used such settings which didn't exist! :(
    /// So for now we need to keep this as `true` - but we should change it back to `false` in
    /// the next base class update, probably v18.
    /// </summary>
    private const bool SettingsPropsRequired = false;

    public TSettings Settings => _settings.Get(() => _parent._CodeApiSvc.Cdf.AsCustom<TSettings>(_parent._CodeApiSvc.Cdf.AsItem(_view.Settings, propsRequired: SettingsPropsRequired)));
    private readonly GetOnce<TSettings> _settings = new();

    public TResources Resources => _resources.Get(() => _parent._CodeApiSvc.Cdf.AsCustom<TResources>(_view.Resources));
    private readonly GetOnce<TResources> _resources = new();
}