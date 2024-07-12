using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSettings"></typeparam>
/// <typeparam name="TResources"></typeparam>
/// <param name="parent"></param>
/// <param name="block"></param>
/// <param name="settingsPropsRequired">
/// Problem: in v16/17 we forgot to make propsRequired for Setting be true.
/// So they were `false` - and the Content App used such settings which didn't exist! :(
/// So for now we need to keep this as `true` - but we should change it back to `false` in
/// the next base class update, probably v18.
/// </param>
internal class CmsView<TSettings, TResources>(CmsContext parent, IBlock block, bool settingsPropsRequired = true)
    : CmsView(parent, block), ICmsView<TSettings, TResources>
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    private readonly IView _view = block.View;
    private readonly CmsContext _parent = parent;

    public TSettings Settings => _settings.Get(() => _parent._CodeApiSvc.Cdf.AsCustom<TSettings>(_parent._CodeApiSvc.Cdf.AsItem(_view.Settings, propsRequired: settingsPropsRequired)));
    private readonly GetOnce<TSettings> _settings = new();

    public TResources Resources => _resources.Get(() => _parent._CodeApiSvc.Cdf.AsCustom<TResources>(_view.Resources));
    private readonly GetOnce<TResources> _resources = new();
}